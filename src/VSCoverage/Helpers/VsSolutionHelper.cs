using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using VSCoverage.Model;

namespace VSCoverage.Helpers
{
    public static class VsSolutionHelper
    {
        private static Guid csLib = new Guid("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");

        public static IList<EnvDTE.Project> GetTestProjects()
        {
            var projects = GetEnvDTEProjectsInSolution();
            var componentModel = (IComponentModel)ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel));
            IVsPackageInstallerServices installerServices = componentModel.GetService<IVsPackageInstallerServices>();
            var result = new List<EnvDTE.Project>();

            foreach (var proj in projects)
            {
                if (installerServices.IsPackageInstalled(proj, "Microsoft.NET.Test.Sdk"))
                    result.Add(proj);
            }

            return result;
        }

        public static IList<Item> GetSolutionHierarchy()
        {
            var result = new List<Item>();
            var projects = GetEnvDTEProjectsInSolution();

            foreach (var project in projects)
            {
                var p = new Model.Project { Name = project.Name };
                result.Add(p);

                // Get all classes
                var classes = GetProjectClasses(project);
                foreach (var c in classes)
                {
                    var split = c.Namespace.Split('.');
                    Item current = p;
                    foreach (var nsPart in split)
                    {
                        var ns = current.Items.FirstOrDefault(x => x is Namespace && x.Name == nsPart);
                        if (ns is null)
                        {
                            ns = new Namespace { Name = nsPart };
                            current.Items.Add(ns);
                        }

                        current = ns;
                    }

                    current.Items.Add(new Class { Name = c.Name, FullPath = c.FullPath, ProjectItem = c.ProjectItem });
                }
            }

            FlattenNamespaces(result);
            return result;
        }

        private static void FlattenNamespaces(IList<Item> items)
        {
            foreach (var item in items)
            {
                if (item is Namespace ns)
                {
                    while (ns.Items.Count == 1 && ns.Items[0] is Namespace inner)
                    {
                        item.Name = $"{item.Name}.{inner.Name}";
                        item.Items = inner.Items;
                    }
                }

                FlattenNamespaces(item.Items);
            }
        }

        private struct ClassInfo
        {
            public string FullPath { get; set; }

            public string Namespace { get; set; }

            public string Name { get; set; }

            public ProjectItem ProjectItem { get; set; }
        }

        private static IList<ClassInfo> GetProjectClasses(EnvDTE.Project project)
        {
            var result = new List<ClassInfo>();
            var classes = GetProjectItems(project.ProjectItems).Where(x => x.Name.EndsWith(".cs"));
            foreach (var c in classes)
            {
                var fileCodeModel = c.FileCodeModel;
                if (fileCodeModel is null)
                    continue;

                foreach (var codeElement in fileCodeModel.CodeElements)
                {
                    if (codeElement is CodeNamespace codeNamespace)
                    {
                        foreach (var member in codeNamespace.Members)
                        {
                            if (member is CodeType codeType)
                            {
                                result.Add(new ClassInfo
                                {
                                    FullPath = c.Properties.Item("FullPath").Value.ToString(),
                                    Namespace= codeNamespace.Name,
                                    Name = codeType.Name,
                                    ProjectItem = c
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static IEnumerable<ProjectItem> GetProjectItems(EnvDTE.ProjectItems projectItems)
        {
            foreach (EnvDTE.ProjectItem item in projectItems)
            {
                yield return item;

                if (item.SubProject != null)
                {
                    foreach (EnvDTE.ProjectItem childItem in GetProjectItems(item.SubProject.ProjectItems))
                        yield return childItem;
                }
                else
                {
                    foreach (EnvDTE.ProjectItem childItem in GetProjectItems(item.ProjectItems))
                        yield return childItem;
                }
            }

        }

        private static List<string> GetNamespacesRecursive(CodeElement elem)
        {
            var namespaces = new List<string>();

            if (IsNamespaceable(elem.Kind) && IsEmptyNamespace(elem))
            {
                namespaces.Add(string.Empty);
            }

            if (elem.Kind == vsCMElement.vsCMElementNamespace && !namespaces.Contains(elem.FullName))
            {
                namespaces.Add(elem.FullName);
                if (elem.Children != null)
                {
                    foreach (CodeElement codeElement in elem.Children)
                    {
                        var ns = GetNamespacesRecursive(codeElement);
                        if (ns.Count > 0)
                            namespaces.AddRange(ns);
                    }
                }
            }

            return namespaces;
        }

        private static bool IsEmptyNamespace(CodeElement elem)
        {
            return elem.FullName.IndexOf('.') < 0;
        }

        private static bool IsNamespaceable(vsCMElement kind)
        {
            return (kind == vsCMElement.vsCMElementClass
                    || kind == vsCMElement.vsCMElementEnum
                    || kind == vsCMElement.vsCMElementInterface
                    || kind == vsCMElement.vsCMElementStruct);
        }

        private static List<ProjectItem> GetProjectItemsRecursively(ProjectItems items)
        {
            var ret = new List<EnvDTE.ProjectItem>();
            if (items == null) return ret;
            foreach (ProjectItem item in items)
            {
                ret.Add(item);
                ret.AddRange(GetProjectItemsRecursively(item.ProjectItems));
            }
            return ret;
        }

        private static IEnumerable<EnvDTE.Project> GetEnvDTEProjectsInSolution()
        {
            List<EnvDTE.Project> ret = new List<EnvDTE.Project>();
            EnvDTE80.DTE2 dte = (EnvDTE80.DTE2)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));

            EnvDTE.UIHierarchy hierarchy = dte.ToolWindows.SolutionExplorer;
            foreach (EnvDTE.UIHierarchyItem innerItem in hierarchy.UIHierarchyItems)
            {
                FindProjectsIn(innerItem, ret);
            }
            return ret;
        }

        private static void FindProjectsIn(EnvDTE.ProjectItem item, List<EnvDTE.Project> results)
        {
            if (item.Object is EnvDTE.Project)
            {
                var proj = (EnvDTE.Project)item.Object;
                if (new Guid(proj.Kind) == csLib)
                {
                    results.Add((EnvDTE.Project)item.Object);
                }
                else
                {
                    foreach (EnvDTE.ProjectItem innerItem in proj.ProjectItems)
                    {
                        FindProjectsIn(innerItem, results);
                    }
                }
            }
            else if (item.ProjectItems != null)
            {
                foreach (EnvDTE.ProjectItem innerItem in item.ProjectItems)
                {
                    FindProjectsIn(innerItem, results);
                }
            }
        }

        private static void FindProjectsIn(EnvDTE.UIHierarchyItem item, List<EnvDTE.Project> results)
        {
            if (item.Object is EnvDTE.Project)
            {
                var proj = (EnvDTE.Project)item.Object;
                if (new Guid(proj.Kind) == csLib)
                {
                    results.Add((EnvDTE.Project)item.Object);
                }
                else
                {
                    foreach (EnvDTE.ProjectItem innerItem in proj.ProjectItems)
                    {
                        FindProjectsIn(innerItem, results);
                    }
                }
            }
            else
            {
                foreach (EnvDTE.UIHierarchyItem innerItem in item.UIHierarchyItems)
                {
                    FindProjectsIn(innerItem, results);
                }
            }
        }
    }
}

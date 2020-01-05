using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using VSCoverage.Helpers;
using VSCoverage.UnitTests.LCOV;

namespace VSCoverage.Model
{
    public class CoverageViewModel
    {
        public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();

        private ICommand _updateCommand;
        public ICommand UpdateCommand => _updateCommand ?? (_updateCommand = new RelayCommand(x => UpdateItems()));

        public int Level { get; set; }

        public CoverageViewModel()
        {
        }


        private void UpdateItems()
        {
            var items = VsSolutionHelper.GetSolutionHierarchy();
            UpdateLevel(items);
            Level = items.Max(x => x.Level);

            var coverage = UnitTests.Coverage.GetTestCoverage();
            UpdateCoverage(items, coverage);

            Items.Clear();
            foreach (var p in items)
            {
                Items.Add(p);
            }
        }

        private void UpdateLevel(IList<Item> items)
        {
            var depth = GetMaxDepth(items);
            UpdateLevel(depth, items);
        }

        private void UpdateLevel(int currentLevel, IList<Item> items)
        {
            foreach (var item in items)
            {
                item.Level = currentLevel;
                UpdateLevel(currentLevel - 1, item.Items);
            }
        }

        private int GetMaxDepth(IList<Item> items)
        {
            if (items.Count == 0)
                return 0;
            
            return items.Select(x => GetMaxDepth(x.Items)).Max() + 1;
        }

        private Coverage UpdateCoverage(IList<Item> items, IList<FileCoverage> coverage)
        {
            var total = new Coverage();
            foreach (var item in items)
            {
                if (item is Class classItem)
                {
                    var c = coverage.FirstOrDefault(x => x.FilePath == classItem.FullPath);
                    if (c is null)
                        continue;

                    item.Coverage = new Coverage(c.Lines.Count, c.Lines.Count(x => x.Hit), c.Branches.Count, c.Branches.Count(x => x.Taken > 0));
                    total += item.Coverage;
                }
                else
                {
                    item.Coverage = UpdateCoverage(item.Items, coverage);
                    total += item.Coverage;
                }
            }

            return total;
        }
    }
}

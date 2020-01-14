using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace VSCoverage.Editor
{
    public static class Classifications
    {
        public const string CoverageOk = "Coverage/Ok";
        public const string CoverageWarn = "Coverage/Warn";
        public const string CoverageError = "Coverage/Error";
        private const double Opacity = 0.4;

        [Export]
        [Name(CoverageOk)]
        private static ClassificationTypeDefinition CoverageOkType = null;

        [Export]
        [Name(CoverageWarn)]
        private static ClassificationTypeDefinition CoverageWarnType = null;

        [Export]
        [Name(CoverageError)]
        private static ClassificationTypeDefinition CoverageErrorType = null;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = CoverageOk)]
        [UserVisible(true)]
        [Name(CoverageOk)]
        [Order(After = Priority.Default, Before = Priority.High)]
        public sealed class ArchiveKeyFormatDefinition : ClassificationFormatDefinition
        {
            public ArchiveKeyFormatDefinition()
            {
                BackgroundColor = Colors.Green;
                BackgroundOpacity = Opacity;
                
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = CoverageWarn)]
        [UserVisible(true)]
        [Name(CoverageWarn)]
        [Order(After = Priority.Default, Before = Priority.High)]
        public sealed class ArchiveKeyVarFormatDefinition : ClassificationFormatDefinition
        {
            public ArchiveKeyVarFormatDefinition()
            {
                BackgroundColor = Colors.Orange;
                BackgroundOpacity = Opacity;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = CoverageError)]
        [UserVisible(true)]
        [Name(CoverageError)]
        [Order(After = Priority.Default, Before = Priority.High)]
        public sealed class ArchiveKeyErrFormatDefinition : ClassificationFormatDefinition
        {
            public ArchiveKeyErrFormatDefinition()
            {
                BackgroundColor = Colors.Red;
                BackgroundOpacity = Opacity;
            }
        }
    }
}

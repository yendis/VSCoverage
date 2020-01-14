using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using VSCoverage.Model;

namespace VSCoverage.Editor
{
    /// <summary>
    /// Classifier provider. It adds the classifier to the set of classifiers.
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("text")] // This classifier applies to all text files.
    internal class CoverageClassifierProvider : IClassifierProvider
    {
        // Disable "Field is never assigned to..." compiler's warning. Justification: the field is assigned by MEF.
#pragma warning disable 649

        /// <summary>
        /// Classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        private IClassificationTypeRegistryService classificationRegistry;

#pragma warning restore 649

        #region IClassifierProvider

        /// <summary>
        /// Gets a classifier for the given text buffer.
        /// </summary>
        /// <param name="buffer">The <see cref="ITextBuffer"/> to classify.</param>
        /// <returns>A classifier for the text buffer, or null if the provider cannot do so in its current state.</returns>
        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (!CoverageViewModel.Instance.LastUpdatedUtc.HasValue) 
                return null;

            var filePath = GetDocumentPath(buffer.CurrentSnapshot);
            if (filePath is null)
                return null;

            var fileChanged = System.IO.File.GetLastWriteTimeUtc(filePath);
            if (fileChanged > CoverageViewModel.Instance.LastUpdatedUtc)
                return null;

            var coverage = CoverageViewModel.Instance.FileCoverages.FirstOrDefault(x => x.FilePath == filePath);
            if (coverage is null)
                return null;

            return buffer.Properties.GetOrCreateSingletonProperty<CoverageClassifier>(creator: () => new CoverageClassifier(this.classificationRegistry, coverage));
        }

        public string GetDocumentPath(Microsoft.VisualStudio.Text.ITextSnapshot ts)
        {
            Microsoft.VisualStudio.Text.ITextDocument textDoc;
            bool rc = ts.TextBuffer.Properties.TryGetProperty(
                typeof(Microsoft.VisualStudio.Text.ITextDocument), out textDoc);
            if (rc && textDoc != null)
                return textDoc.FilePath;
            return null;
        }

        #endregion
    }
}

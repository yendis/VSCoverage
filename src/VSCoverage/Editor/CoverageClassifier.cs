using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using VSCoverage.UnitTests.LCOV;

namespace VSCoverage.Editor
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "CoverageClassifier" classification type.
    /// </summary>
    internal class CoverageClassifier : IClassifier
    {
        /// <summary>
        /// Classification type.
        /// </summary>
        private readonly IClassificationType okClassification;
        private readonly IClassificationType warnClassification;
        private readonly IClassificationType errorClassification;

        private readonly FileCoverage _coverage;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverageClassifier"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal CoverageClassifier(IClassificationTypeRegistryService registry, FileCoverage coverage)
        {
            okClassification = registry.GetClassificationType(Classifications.CoverageOk);
            warnClassification = registry.GetClassificationType(Classifications.CoverageWarn);
            errorClassification = registry.GetClassificationType(Classifications.CoverageError);
            _coverage = coverage;
        }

        #region IClassifier

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>();
            if (_coverage is null)
                return result;

            var lineNumber = span.Snapshot.GetLineNumberFromPosition(span.Start.Position) + 1;
            var line = _coverage.Lines.FirstOrDefault(x => x.LineNumber == lineNumber);
            if (line is null)
                return result;

            if (!line.Hit)
            {
                result.Add(new ClassificationSpan(TrimSpan(span), errorClassification));
                return result;
            }

            var branches = _coverage.Branches.Where(x => x.LineNumber == lineNumber).ToList();
            if (branches.Count == 0 || branches.All(x => x.Taken > 0))
                result.Add(new ClassificationSpan(TrimSpan(span), okClassification));
            else
                result.Add(new ClassificationSpan(TrimSpan(span), warnClassification));

            return result;
        }

        private SnapshotSpan TrimSpan(SnapshotSpan span)
        {
            var text = span.GetText();
            var spaceCount = GetSpaceCount(text);
            var start = span.Start + spaceCount;
            return new SnapshotSpan(span.Snapshot, start, span.Length - spaceCount);
        }

        private int GetSpaceCount(string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] != ' ')
                    return i;
            }

            return text.Length;
        }


        #endregion
    }
}

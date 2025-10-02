namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.HtmlWriter
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.Results;

    internal class ValidatorResultTreeNode(ValidatorResult result) : ValidatorResultTreeItem(result)
    {
        private int suppressedCount;
        private int nonSuppressedCount;

        /// <summary>
        /// Gets the sub results.
        /// </summary>
        /// <value>The sub results.</value>
        public List<ValidatorResultTreeItem> SubResults { get; } = [];

        public override int SuppressedCount => suppressedCount;

        public override int NonSuppressedCount => nonSuppressedCount;

        private void AddSubResults(StringBuilder stringBuilder, bool includeSuppressed, int depth = 2)
        {
            foreach (var result in SubResults)
            {
                result.WriteHtml(stringBuilder, includeSuppressed, depth);
            }
        }

        public void UpdateCounts()
        {
            suppressedCount = 0;
            nonSuppressedCount = 0;

            foreach (var result in SubResults)
            {
                suppressedCount += result.SuppressedCount;
                nonSuppressedCount += result.NonSuppressedCount;
            }
        }

        public override void WriteHtml(StringBuilder stringBuilder, bool includeSuppressed, int depth = 2)
        {
            // Add row start
            stringBuilder.Append($"<tr data-depth=\"{depth}\" class=\"collapse level{depth}{(Suppressed ? " suppressed" : String.Empty)}\">");

            // Add description (with expand/collapse toggle and colored rectangle)
            stringBuilder.Append("<td><span class=\"toggle collapse\"></span>");
            stringBuilder.Append($"&nbsp;<span class=\"{Severity.ToString().ToLower()}\">&nbsp;</span>&nbsp;{Description}&nbsp;");

            if (includeSuppressed)
            {
                stringBuilder.Append($"({NonSuppressedCount} active, {SuppressedCount} suppressed)");
            }
            else
            {
                stringBuilder.Append($"({NonSuppressedCount} active)");
            }

            stringBuilder.Append("</td>");

            // Add other columns
            string linkToDocId = $"<a href=\"https://skylinecommunications.github.io/Skyline.DataMiner.CICD.Validators/checks/Check_{Id.Replace('.', '_')}.html\" target=\"_blank\" rel=\"noopener noreferrer\">{Id}</a>";
            stringBuilder.Append($"<td>{GetState()}</td><td>{Certainty}</td><td>{FixImpact}</td><td>{Category}</td><td>{linkToDocId}</td><td>{Line}</td><td>{Column}</td><td>{Dve}</td>");

            // Add row end
            stringBuilder.Append("</tr>");

            AddSubResults(stringBuilder, includeSuppressed, depth + 1);
        }
    }
}

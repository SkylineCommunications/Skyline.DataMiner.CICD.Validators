namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.HtmlWriter
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class ValidatorResultTreeNode : ValidatorResultTreeItem
    {
        private int suppressedCount;
        private int nonSuppressedCount;

        public ValidatorResultTreeNode(ValidatorResult validatorResult) : base(validatorResult)
        {
            SubResults = new List<ValidatorResultTreeItem>();
        }

        /// <summary>
        /// Gets the sub results.
        /// </summary>
        /// <value>The sub results.</value>
        public List<ValidatorResultTreeItem> SubResults { get; }

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
            stringBuilder.AppendFormat("        <tr data-depth=\"{0}\" class=\"collapse level{0}{2}\">{1}            <td>", depth, Environment.NewLine, Suppressed ? " suppressed" : String.Empty);
            stringBuilder.Append("<span class=\"toggle collapse\"></span>");
            //stringBuilder.AppendFormat("&nbsp;<span class=\"{1}\" >&nbsp;</span>&nbsp;{2}&nbsp;({3} active, {4} suppressed)</td>{0}            <td>{5}</td>{0}            <td>{6}</td>{0}            <td>{7}</td>{0}            <td>{8}</td>{0}            <td>{9}</td>{0}            <td>{10}</td>{0}            <td>{11}</td>{0}            <td>{12}</td>{0}        </tr>", Environment.NewLine, Severity.ToString().ToLower(), Description, NonSuppressedCount, SuppressedCount, GetState(), Certainty, FixImpact, Category, Id, Line, Column, Dve);

            stringBuilder.AppendFormat("&nbsp;<span class=\"{1}\" >&nbsp;</span>&nbsp;{2}&nbsp;", Environment.NewLine, Severity.ToString().ToLower(), Description, NonSuppressedCount, SuppressedCount);

            if (includeSuppressed)
            {
                stringBuilder.AppendFormat("({0} active, {1} suppressed)", NonSuppressedCount, SuppressedCount);
            }
            else
            {
                stringBuilder.AppendFormat("({0} active)", NonSuppressedCount);
            }

            stringBuilder.AppendFormat("</td>{0}            <td>{1}</td>{0}            <td>{2}</td>{0}            <td>{3}</td>{0}            <td>{4}</td>{0}            <td>{5}</td>{0}            <td>{6}</td>{0}            <td>{7}</td>{0}            <td>{8}</td>{0}        </tr>", Environment.NewLine, GetState(), Certainty, FixImpact, Category, Id, Line, Column, Dve);

            AddSubResults(stringBuilder, includeSuppressed, depth + 1);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.HtmlWriter
{
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

        public override int SuppressedCount { get { return suppressedCount; } }

        public override int NonSuppressedCount { get { return nonSuppressedCount; } }

        private void AddSubResults(StringBuilder stringBuilder, int depth = 2)
        {
            foreach (var result in SubResults)
            {
                result.WriteHtml(stringBuilder, depth);
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

        public override void WriteHtml(StringBuilder stringBuilder, int depth = 2)
        {
            stringBuilder.AppendFormat("        <tr data-depth=\"{0}\" class=\"collapse level{0}{2}\">{1}            <td>", depth, Environment.NewLine, Suppressed ? " suppressed" : String.Empty);
            stringBuilder.Append("<span class=\"toggle collapse\"></span>");
            stringBuilder.AppendFormat("&nbsp;<span class=\"{1}\" >&nbsp;</span>&nbsp;{2}&nbsp;({3} active, {4} suppressed)</td>{0}            <td>{5}</td>{0}            <td>{6}</td>{0}            <td>{7}</td>{0}            <td>{8}</td>{0}            <td>{9}</td>{0}            <td>{10}</td>{0}            <td>{11}</td>{0}            <td>{12}</td>{0}        </tr>", Environment.NewLine, Severity.ToString().ToLower(), Description, NonSuppressedCount, SuppressedCount, GetState(), Certainty, FixImpact, Category, Id, Line, Column, Dve);

            AddSubResults(stringBuilder, depth + 1);
        }
    }
}

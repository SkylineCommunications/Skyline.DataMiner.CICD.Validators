namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.HtmlWriter
{
    using System;
    using System.Text;

    using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.Results;

    internal class ValidatorResultTreeLeaf(ValidatorResult validatorResult) : ValidatorResultTreeItem(validatorResult)
    {
        public override int SuppressedCount => Suppressed ? 1 : 0;

        public override int NonSuppressedCount => Suppressed ? 0 : 1;

        public override void WriteHtml(StringBuilder stringBuilder, bool includeSuppressed, int depth = 2)
        {
            stringBuilder.AppendFormat("        <tr data-depth=\"{0}\" class=\"collapse level{0}{2}\">{1}            <td>", depth, Environment.NewLine, Suppressed ? " suppressed" : String.Empty);
            stringBuilder.AppendFormat("<span class=\"notoggle\"></span>");
            stringBuilder.AppendFormat("&nbsp;<span class=\"{1}\" >&nbsp;</span>&nbsp;{2}</td>{0}            <td>{3}</td>{0}            <td>{4}</td>{0}            <td>{5}</td>{0}            <td>{6}</td>{0}            <td>{7}</td>{0}            <td>{8}</td>{0}            <td>{9}</td>{0}            <td>{10}</td>{0}        </tr>", Environment.NewLine, Severity.ToString().ToLower(), Description, GetState(), Certainty, FixImpact, Category, Id, Line, Column, Dve);
        }
    }
}

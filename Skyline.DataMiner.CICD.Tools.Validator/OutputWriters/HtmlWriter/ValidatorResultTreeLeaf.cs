namespace Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.HtmlWriter
{
    using System;
    using System.Text;

    using Skyline.DataMiner.CICD.Tools.Validator.OutputWriters.Results;

    internal class ValidatorResultTreeLeaf(ValidatorResult result) : ValidatorResultTreeItem(result)
    {
        public override int SuppressedCount => Suppressed ? 1 : 0;

        public override int NonSuppressedCount => Suppressed ? 0 : 1;

        public override void WriteHtml(StringBuilder stringBuilder, bool includeSuppressed, int depth = 2)
        {
            // Add row start
            stringBuilder.Append($"<tr data-depth=\"{depth}\" class=\"collapse level{depth}{(Suppressed ? " suppressed" : String.Empty)}\">");

            // Add description (with colored rectangle and no expand/collapse toggle)
            stringBuilder.Append("<td><span class=\"notoggle\"></span>");
            stringBuilder.Append($"&nbsp;<span class=\"{Severity.ToString().ToLower()}\">&nbsp;</span>&nbsp;{Description}</td>");

            // Add other columns
            string linkToDocId = $"<a href=\"https://skylinecommunications.github.io/Skyline.DataMiner.CICD.Validators/checks/Check_{Id.Replace('.', '_')}.html\" target=\"_blank\" rel=\"noopener noreferrer\">{Id}</a>";
            stringBuilder.Append($"<td>{GetState()}</td><td>{Certainty}</td><td>{FixImpact}</td><td>{Category}</td><td>{linkToDocId}</td><td>{Line}</td><td>{Column}</td><td>{Dve}</td>");

            // Add row end
            stringBuilder.Append("</tr>");
        }
    }
}

namespace Skyline.DataMiner.CICD.Validators.Protocol.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Serializable]
    [DataContract]
    internal class ValidationResult : IValidationResult, ISeverityBubbleUpResult
    {
        public Dictionary<Enum, object> ExtraData { get; set; }

        public List<IValidationResult> SubResults { get; set; }

        public IValidate Test { get; set; }

        public uint ErrorId { get; set; }

        public uint CheckId { get; set; }

        [DataMember(Order = 1)]
        public Category Category { get; set; }

        public Source Source { get; set; }

        [DataMember(Order = 3)]
        public Severity Severity { get; set; }

        [DataMember(Order = 4)]
        public Certainty Certainty { get; set; }

        [DataMember(Order = 5)]
        public FixImpact FixImpact { get; set; }

        public string GroupDescription { get; set; }

        [DataMember(Order = 2)]
        public string Description { get; set; }

        public string HowToFix { get; set; }

        [Obsolete("Has been moved to the dedicated markdown file about the error. Will be removed in the next major change update.")]
        public string ExampleCode { get; set; }

        [Obsolete("Has been moved to the dedicated markdown file about the error. Will be removed in the next major change update.")]
        public string Details { get; set; }

        public IReadable ReferenceNode { get; set; }

        public bool HasCodeFix { get; set; }

        [DataMember(Order = 0)]
        public string FullId { get; set; }

        [DataMember(Order = 6)]
        public int Line { get; set; }

        public int Position => PositionNode?.ReadNode?.FirstCharOffset ?? -1;

        public IReadable PositionNode { get; set; }

        public string DescriptionFormat { get; set; }

        public object[] DescriptionParameters { get; set; }

        public List<(string Message, bool AutoFixPopup)> AutoFixWarnings { get; set; }

        [DataMember(Order = 7)]
        public (int TablePid, string Name)? DveExport { get; set; }

        public ValidationResult()
        {
            Category = Category.Undefined;
            FixImpact = FixImpact.Undefined;
            Severity = Severity.Undefined;
            Source = Source.Undefined;
            Certainty = Certainty.Undefined;
            SubResults = new List<IValidationResult>();
            GroupDescription = String.Empty;
            ExtraData = new Dictionary<Enum, object>();
            AutoFixWarnings = new List<(string, bool)>();
        }

        void ISeverityBubbleUpResult.DoSeverityBubbleUp()
        {
            if (SubResults == null || SubResults.Count <= 0)
            {
                return;
            }

            Severity maxSeverity = Severity;
            foreach (IValidationResult subResult in SubResults)
            {
                if (subResult is ISeverityBubbleUpResult bubbleUp)
                {
                    bubbleUp.DoSeverityBubbleUp();
                }

                if (subResult.Severity > maxSeverity)
                {
                    maxSeverity = subResult.Severity;
                }
            }

            Severity = maxSeverity;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("{")
                .AppendLine($"\tSource = {Source}")
                .AppendLine($"\tCategory = {Category}")
                .AppendLine($"\tErrorId = {ErrorId}")
                .AppendLine($"\tDescription = {Description}")
                .AppendLine($"\tSeverity = {Severity}")
                .AppendLine($"\tCertainty = {Certainty}")
                .AppendLine($"\tFixImpact = {FixImpact}")
                .AppendLine($"\tHasCodeFix = {HasCodeFix}")
                .AppendLine($"\tHowToFix = {HowToFix}")
                .AppendLine("\tSubResults = ");

            if (SubResults != null)
            {
                foreach (IValidationResult subResult in SubResults)
                {
                    sb.AppendLine("\t\t" + subResult);
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
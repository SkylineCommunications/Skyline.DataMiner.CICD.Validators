namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Ports.PortSettings.CheckNameAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckNameAttribute, Category.Ports)]
    internal class CheckNameAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var ports = context.ProtocolModel?.Protocol?.Ports;
            if (ports == null)
            {
                return results;
            }

            for (int i = 0; i < ports.Count; i++)
            {
                var portSettings = ports[i];
                string connectionId = (i + 1).ToString();
                var name = portSettings.Name;
                (GenericStatus status, string rawValue, string _) = GenericTests.CheckBasics(name, isRequired: true);

                if (status.HasFlag(GenericStatus.Missing))
                {
                    results.Add(Error.MissingAttribute(this, portSettings, portSettings, connectionId));
                    continue;
                }

                if (status.HasFlag(GenericStatus.Empty))
                {
                    results.Add(Error.EmptyAttribute(this, portSettings, name, connectionId));
                    continue;
                }

                if (status.HasFlag(GenericStatus.Untrimmed))
                {
                    results.Add(Error.UntrimmedAttribute(this, portSettings, name, connectionId, rawValue));
                    continue;
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            if (context.Protocol.Ports == null)
            {
                result.Message = "No Ports found!";
                return result;
            }

            var readNode = (IPortSettings)context.Result.ReferenceNode;
            var editNode = context.Protocol.Ports.Get(readNode);

            switch (context.Result.ErrorId)
            {
                case ErrorIds.EmptyAttribute:
                case ErrorIds.MissingAttribute:
                    editNode.Name = ConnectionHelper.CreateConnectionName(context.Protocol.Read, (IPortSettingsBase)readNode);
                    result.Success = true;
                    break;

                case ErrorIds.UntrimmedAttribute:
                    editNode.Name.Value = readNode.Name.Value.Trim();
                    result.Success = true;
                    break;

                default:
                    result.Message = $"This error ({context.Result.ErrorId}) isn't implemented.";
                    break;
            }

            return result;
        }

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }
}
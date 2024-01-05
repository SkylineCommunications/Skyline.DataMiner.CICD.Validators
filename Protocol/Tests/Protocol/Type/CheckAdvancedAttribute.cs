namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Type.CheckAdvancedAttribute
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckAdvancedAttribute, Category.Protocol)]
    internal class CheckAdvancedAttribute : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            var type = context?.ProtocolModel?.Protocol?.Type;
            var advanced = type?.Advanced;
            if (advanced == null)
            {
                return results;
            }

            (GenericStatus status, string _, string _) = Generic.GenericTests.CheckBasics(advanced, isRequired: false);

            // Empty
            if (status.HasFlag(GenericStatus.Empty))
            {
                results.Add(Error.EmptyAttribute(this, advanced, advanced));
                return results;
            }

            ProtocolTypeAdvanced connections = type.GetAdvanced();

            List<IValidationResult> untrimmedSubResults = new List<IValidationResult>();
            foreach (AdvancedConnection connection in connections.Connections)
            {
                string[] thingsToCheck =
                {
                    connection.Name,
                    connection.TypeString
                };

                foreach (string thing in thingsToCheck)
                {
                    (GenericStatus valueStatus, string _) = Generic.GenericTests.CheckBasics<string>(thing);

                    if (valueStatus.HasFlag(GenericStatus.Untrimmed))
                    {
                        untrimmedSubResults.Add(Error.UntrimmedValueInAttribute_Sub(this, advanced, advanced, thing));
                    }
                }

                if (connection.Type == null)
                {
                    results.Add(Error.UnknownConnection(this, advanced, advanced, connection.TypeString, connection.ConnectionId.ToString()));
                }
            }

            if (untrimmedSubResults.Count > 0)
            {
                IValidationResult error = Error.UntrimmedAttribute(this, advanced, advanced, connections.ToString());
                error.WithSubResults(untrimmedSubResults.ToArray());
                results.Add(error);
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.EmptyAttribute:
                    context.Protocol.Type.Advanced = null;
                    result.Success = true;
                    break;

                case ErrorIds.UntrimmedAttribute:
                    {
                        var editNode = context.Protocol.Type.Advanced;

                        string[] connections = editNode.Value.Split(';');

                        List<string> newConnections = new List<string>(connections.Length);
                        foreach (string connection in connections)
                        {
                            string[] parts = connection.Split(':');

                            List<string> newParts = new List<string>(parts.Length);
                            foreach (string part in parts)
                            {
                                newParts.Add(part.Trim());
                            }

                            newConnections.Add(String.Join(":", newParts));
                        }

                        editNode.Value = String.Join(";", newConnections);
                        result.Success = true;
                        break;
                    }

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
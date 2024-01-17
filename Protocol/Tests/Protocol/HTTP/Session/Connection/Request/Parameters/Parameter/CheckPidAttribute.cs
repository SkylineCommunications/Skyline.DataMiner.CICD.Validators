namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.HTTP.Session.Connection.Request.Parameters.Parameter.CheckPidAttribute
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Linking;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Generic;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckPidAttribute, Category.HTTP)]
    internal class CheckPidAttribute : IValidate/*, ICodeFix, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach (IHTTPSession session in context.EachHttpSessionWithValidId())
            {
                foreach (IHTTPSessionConnection connection in context.EachHttpConnectionWithValidId(session))
                {
                    if (connection.Request?.Parameters == null)
                    {
                        continue;
                    }

                    foreach (var parameter in connection.Request.Parameters)
                    {
                        (GenericStatus status, string rawValue, uint? _) = GenericTests.CheckBasics(parameter.Pid, isRequired: true);

                        if (status.HasFlag(GenericStatus.Missing))
                        {
                            continue;
                        }

                        if (status.HasFlag(GenericStatus.Empty))
                        {
                            results.Add(Error.EmptyAttribute(this, parameter, parameter, session.Id.RawValue, connection.Id.RawValue));
                            continue;
                        }

                        if (status.HasFlag(GenericStatus.Invalid))
                        {
                            results.Add(Error.InvalidAttribute(this, parameter, parameter, rawValue, session.Id.RawValue, connection.Id.RawValue));
                            continue;
                        }

                        if (!context.ProtocolModel.TryGetObjectByKey<IParamsParam>(Mappings.ParamsById, rawValue, out _))
                        {
                            results.Add(Error.NonExistingId(this, parameter, parameter, rawValue, session.Id.RawValue, connection.Id.RawValue));
                            continue;
                        }
                    }
                }
            }

            return results;
        }

        ////public ICodeFixResult Fix(CodeFixContext context)
        ////{
        ////    CodeFixResult result = new CodeFixResult();

        ////    switch (context.Result.ErrorId)
        ////    {

        ////        default:
        ////            result.Message = String.Format("This error ({0}) isn't implemented.", context.Result.ErrorId.ToString());
        ////            break;
        ////    }

        ////    return result;
        ////}

        ////public List<IValidationResult> Compare(MajorChangeCheckContext context)
        ////{
        ////    List<IValidationResult> results = new List<IValidationResult>();

        ////    return results;
        ////}
    }
}
//#define DebugValidator
namespace Skyline.DataMiner.CICD.Validators.Protocol.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using XmlDocument = System.Xml.XmlDocument;
    using XmlEdit = Skyline.DataMiner.CICD.Parsers.Common.XmlEdit;

    /// <summary>
    /// Methods for validating a DataMiner Protocol.
    /// </summary>
    public class Validator : IValidator
    {
        /// <summary>
        /// Runs the validator tests
        /// </summary>
        /// <returns>List of results.</returns>
        public IList<IValidationResult> RunValidate(IProtocolInputData input, ValidatorSettings validatorSettings, CancellationToken cancellationToken)
        {
            var results = new List<IValidationResult>();

            // validate main protocol
            results.AddRange(ValidateProtocol(input));

            // validate exported protocols, if any
            foreach (var ep in input.Model.GetAllExportedProtocols())
            {
                var exportInput = new ProtocolInputData(ep.Model, ep.Document, null);
                var exportResults = ValidateProtocol(exportInput);

                // indicate for which DVE these results were created
                foreach (IValidationResult exportResult in exportResults)
                {
                    exportResult.WithDveExport(ep.TablePid, ep.Name);
                }

                results.AddRange(exportResults);
            }

            return results;
        }

        private IList<IValidationResult> ValidateProtocol(IProtocolInputData input)
        {
            var validationResult = new List<IValidationResult>();

            var protocolChecks = new ProtocolChecks();

            // Add line numbers to XML file
            bool linesLoaded;
            XmlDocument xDoc;
            try
            {
                xDoc = protocolChecks.LoadWithLineNums(input.Document.GetXml());
                linesLoaded = true;
            }
            catch (Exception ex)
            {
                xDoc = null;
                linesLoaded = false;

                validationResult.Add(new InternalError
                {
                    Line = Convert.ToInt32(protocolChecks.LineNum),
                    TestName = "LoadLineNums",
                    DescriptionParameters = new object[] { "loading line numbers", ex.ToString() },
                });
            }

            // Run tests
            if (linesLoaded)
            {
                try
                {
                    protocolChecks.CollectParameterInfo(xDoc);
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Collecting Parameter Info", ex.ToString() },
                        TestName = "collectParameterInfo",
                    });
                }

                try
                {
                    protocolChecks.GetDuplicatedParams(xDoc);
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Get duplicated parameters.", ex.ToString() },
                        TestName = "getDuplicatedParams"
                    });
                }
#if DebugValidator
                sw.Stop();
                log.WriteLog("collect parameter info done in " + sw.Elapsed.ToString());
                sw.Reset();
                sw.Start();
#endif

                try
                {
                    validationResult.AddRange(protocolChecks.CheckTableColumnParams(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Table Column Parameters", ex.ToString() },
                        TestName = "CheckTableColumnParams"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckPortSettings(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Port Settings", ex.ToString() },
                        TestName = "CheckPortSettings"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckGroupSettings(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Group Settings", ex.ToString() },
                        TestName = "CheckGroupSettings"
                    });
                }

                try
                {
                    if (input.Model.MainProtocolModel == null)
                    {
                        validationResult.AddRange(protocolChecks.CheckDveColumnOptionElement(xDoc));
                    }
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check DVE Column Option Element", ex.ToString() },
                        TestName = "CheckDVEColumnOptionElement"
                    });
                }

#if DebugValidator
                sw.Stop();
                log.WriteLog("CheckDVEColumnOptionElement done in " + sw.Elapsed.ToString());
                sw.Reset();
                sw.Start();
#endif

                try
                {
                    validationResult.AddRange(protocolChecks.CheckPositions(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Positions", ex.ToString() },
                        TestName = "CheckPositions"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckTrendAlarm(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Trend Alarm", ex.ToString() },
                        TestName = "CheckTrendAlarm"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckRawTypeDouble(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Raw Type Double", ex.ToString() },
                        TestName = "CheckRawTypeDouble"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckAttributesContent(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Attributes Content", ex.ToString() },
                        TestName = "CheckAttributesContent"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckCopyAction(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Copy Action", ex.ToString() },
                        TestName = "CheckCopyAction"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckTimers(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Timers", ex.ToString() },
                        TestName = "CheckTimers"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckRecursivePageButtons(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Recursive Page Buttons", ex.ToString() },
                        TestName = "CheckRecursivePageButtons"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckTableIndexSequence(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Table Index Sequence", ex.ToString() },
                        TestName = "CheckTableIndexSequence"
                    });
                }

#if DebugValidator
                sw.Stop();
                log.WriteLog("CheckTableIndexSequence done in " + sw.Elapsed.ToString());
                sw.Reset();
                sw.Start();
#endif

                try
                {
                    validationResult.AddRange(protocolChecks.CheckRTDisplayTrue(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check RTDisplay", ex.ToString() },
                        TestName = "CheckRTDisplayTrue"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckResponseContent(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Response Content", ex.ToString() },
                        TestName = "CheckResponseContent"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckTableColumnExports(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Table Column Exports", ex.ToString() },
                        TestName = "CheckTableColumnExports"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckProtocolNames(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Protocol Names", ex.ToString() },
                        TestName = "CheckProtocolNames"
                    });
                }

                try
                {
                    validationResult.AddRange(protocolChecks.CheckInterpreteMeasurement(xDoc));
                }
                catch (Exception ex)
                {
                    validationResult.Add(new InternalError
                    {
                        Line = Convert.ToInt32(protocolChecks.LineNum),
                        DescriptionParameters = new object[] { "Check Interprete Measurement", ex.ToString() },
                        TestName = "CheckProtocolNames"
                    });
                }
            }

            return validationResult;
        }

        /// <inheritdoc cref="IValidator.ExecuteCodeFix"/>
        public ICodeFixResult ExecuteCodeFix(XmlEdit.XmlDocument document, Skyline.DataMiner.CICD.Models.Protocol.Edit.Protocol protocol, IValidationResult result, ValidatorSettings validatorSettings)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="IValidator.RunCompare"/>
        public IList<IValidationResult> RunCompare(IProtocolInputData newCode, IProtocolInputData previousCode, ValidatorSettings validatorSettings, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
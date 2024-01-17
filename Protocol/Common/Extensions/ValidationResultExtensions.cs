namespace Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions
{
    using System;
    using System.Linq;

    using AutoMapper;

    using Microsoft.CodeAnalysis;

    using Skyline.DataMiner.CICD.CSharpAnalysis.Interfaces;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    internal static class ValidationResultExtensions
    {
        private static readonly MapperConfiguration _csharpValidationResultMapperConfig
            = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;
                cfg.CreateMap<IValidationResult, CSharpValidationResult>();
            });

        public static ICSharpValidationResult WithCSharp<T>(this IValidationResult result, ICSharpObject<T> csharp) where T : SyntaxNode
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (csharp == null)
            {
                throw new ArgumentNullException(nameof(csharp));
            }

            var csharpResult = ConvertToCSharpValidationResult(result);

            if (csharpResult is CSharpValidationResult csharpResultImpl)
            {
                csharpResultImpl.CSharpLocation = csharp.GetLocation();
            }

            return csharpResult;
        }

        public static ICSharpValidationResult WithCSharp(this IValidationResult result, Location location)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            var csharpResult = ConvertToCSharpValidationResult(result);

            if (csharpResult is CSharpValidationResult csharpResultImpl)
            {
                csharpResultImpl.CSharpLocation = location;
            }

            return csharpResult;
        }

        public static ICSharpValidationResult WithCSharp(this IValidationResult result, SyntaxNode csharp)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (csharp == null)
            {
                throw new ArgumentNullException(nameof(csharp));
            }

            var csharpResult = ConvertToCSharpValidationResult(result);

            if (csharpResult is CSharpValidationResult csharpResultImpl)
            {
                csharpResultImpl.CSharpLocation = csharp.GetLocation();
            }

            return csharpResult;
        }

        public static ICSharpValidationResult WithCSharp(this IValidationResult result, SyntaxTrivia trivia)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var csharpResult = ConvertToCSharpValidationResult(result);

            if (csharpResult is CSharpValidationResult csharpResultImpl)
            {
                csharpResultImpl.CSharpLocation = trivia.GetLocation();
            }

            return csharpResult;
        }

        public static IValidationResult WithSubResults(this IValidationResult result, params IValidationResult[] subResults)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (subResults == null)
            {
                throw new ArgumentNullException(nameof(subResults));
            }

            result.SubResults.AddRange(subResults);
            return result;
        }

        public static IValidationResult WithExtraData(this IValidationResult result, Enum key, object value)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (result is ValidationResult classResult)
            {
                classResult.ExtraData.Add(key, value);
            }

            return result;
        }

        public static IValidationResult WithWarning(this IValidationResult result, string warning, bool autoFixPopup = true)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (String.IsNullOrWhiteSpace(warning))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(warning));
            }

            if (result is ValidationResult classResult)
            {
                classResult.AutoFixWarnings.Add((warning, autoFixPopup));
            }

            return result;
        }

        public static IValidationResult WithWarnings(this IValidationResult result, (string Message, bool AutoFixPopup)[] warnings, bool overwriteWarnings = false)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (warnings == null)
            {
                throw new ArgumentNullException(nameof(warnings));
            }

            if (result is ValidationResult classResult)
            {
                if (overwriteWarnings)
                {
                    classResult.AutoFixWarnings = warnings.ToList();
                }
                else
                {
                    classResult.AutoFixWarnings.AddRange(warnings);
                }
            }

            return result;
        }

        public static IValidationResult WithDveExport(this IValidationResult result, int tablePid, string name)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result is ValidationResult classResult)
            {
                classResult.DveExport = (tablePid, name);

                if (classResult.SubResults != null)
                {
                    foreach (var sr in classResult.SubResults)
                    {
                        // apply recursive to subresults
                        sr.WithDveExport(tablePid, name);
                    }
                }
            }

            return result;
        }

        private static ICSharpValidationResult ConvertToCSharpValidationResult(this IValidationResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (!(result is ICSharpValidationResult csharpResult))
            {
                var mapper = _csharpValidationResultMapperConfig.CreateMapper();
                csharpResult = mapper.Map<CSharpValidationResult>(result);
            }

            return csharpResult;
        }
    }
}
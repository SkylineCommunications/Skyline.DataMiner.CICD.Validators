namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckFileEncoding
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Skyline.DataMiner.CICD.FileSystem;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common.Extensions;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;

    [Test(CheckId.CheckFileEncoding, Category.QAction)]
    internal class CheckFileEncoding : IValidate, ICodeFix/*, ICompare*/
    {
        public List<IValidationResult> Validate(ValidatorContext context)
        {
            List<IValidationResult> results = new List<IValidationResult>();

            foreach ((CompiledQActionProject projectData, IQActionsQAction qaction) in context.EachQActionProject(true))
            {
                var project = projectData.Project;
                string projectDirectory = FileSystem.Instance.Path.GetDirectoryName(project.FilePath);

                string[] csharpFiles = FileSystem.Instance.Directory.GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories);

                foreach (string csharpFile in csharpFiles)
                {
                    using (StreamReader sr = new StreamReader(csharpFile))
                    {
                        // Have a look in the file, so it can detect the encoding.
                        sr.Peek();

                        if (!sr.CurrentEncoding.Equals(Encoding.UTF8))
                        {
                            results.Add(Error.InvalidFileEncoding(this, qaction, qaction, sr.CurrentEncoding.EncodingName, qaction.Id.RawValue)
                                             .WithExtraData(ExtraData.InvalidFileEncoding, csharpFile));
                        }
                    }
                }
            }

            return results;
        }

        public ICodeFixResult Fix(CodeFixContext context)
        {
            CodeFixResult result = new CodeFixResult();

            switch (context.Result.ErrorId)
            {
                case ErrorIds.InvalidFileEncoding:
                    if (context.Result.ExtraData.TryGetValue(ExtraData.InvalidFileEncoding, out object csharpFile) && csharpFile is string filePath)
                    {
                        string tempPath = Path.GetTempFileName();
                        using (StreamReader sr = new StreamReader(filePath))
                        using (StreamWriter sw = new StreamWriter(tempPath, false, Encoding.UTF8))
                        {
                            // Have a look in the file, so it can detect the encoding.
                            sr.Peek();

                            int charsRead;
                            char[] buffer = new char[128 * 1024];
                            while ((charsRead = sr.ReadBlock(buffer, 0, buffer.Length)) > 0)
                            {
                                sw.Write(buffer, 0, charsRead);
                            }
                        }
                        File.Delete(filePath);
                        File.Move(tempPath, filePath);
                    }
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

    internal enum ExtraData
    {
        InvalidFileEncoding,
    }
}
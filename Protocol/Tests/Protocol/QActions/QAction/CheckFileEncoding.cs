namespace Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.QActions.QAction.CheckFileEncoding
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Skyline.DataMiner.CICD.FileSystem;
    using Skyline.DataMiner.CICD.Models.Protocol;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio.Projects;
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

            if (!context.HasQActionsAndIsSolution)
            {
                // Early skip when no QActions are present or when it is not solution based.
                return results;
            }

            foreach ((CompiledQActionProject projectData, IQActionsQAction qaction) in context.EachQActionProject(true))
            {
                Project project = Project.Load(projectData.Project.FilePath, projectData.Project.Name);
                string projectDirectory = FileSystem.Instance.Path.GetDirectoryName(project.Path);
                foreach (ProjectFile file in project.Files)
                {
                    string csharpFile = FileSystem.Instance.Path.Combine(projectDirectory, file.Name);
                    using (StreamReader sr = new StreamReader(csharpFile))
                    {
                        // Have a look in the file, so it can detect the encoding.
                        sr.Peek();

                        if (!sr.CurrentEncoding.Equals(Encoding.UTF8))
                        {
                            string fileName = FileSystem.Instance.Path.GetFileName(csharpFile);
                            results.Add(Error.InvalidFileEncoding(this, qaction, qaction, sr.CurrentEncoding.EncodingName, fileName, qaction.Id.RawValue)
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
            var fs = FileSystem.Instance;

            switch (context.Result.ErrorId)
            {
                case ErrorIds.InvalidFileEncoding:
                    if (context.Result.ExtraData.TryGetValue(ExtraData.InvalidFileEncoding, out object csharpFile) &&
                        csharpFile is string filePath &&
                        fs.File.Exists(filePath))
                    {
                        string tempPath = fs.Path.Combine(fs.Path.GetTempPath(), fs.Path.GetRandomFileName());
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

                        fs.File.Delete(filePath);
                        fs.File.Move(tempPath, filePath);

                        result.Success = true;
                    }
                    else
                    {
                        result.Message = "Unable to locate file.";
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
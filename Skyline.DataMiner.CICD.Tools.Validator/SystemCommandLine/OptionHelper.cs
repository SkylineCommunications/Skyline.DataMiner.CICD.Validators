// ReSharper disable UnusedMember.Global
namespace Skyline.DataMiner.CICD.Tools.Validator.SystemCommandLine
{
    using Skyline.DataMiner.CICD.FileSystem;

    /// <summary>
    /// Helper methods so that System.CommandLine can deal with CICD.FileSystem classes.
    /// </summary>
    internal static class OptionHelper
    {
        public static DirectoryInfo? ParseDirectoryInfo(ArgumentResult result)
        {
            if (result.Tokens.Count != 1)
            {
                result.ErrorMessage = $"--{result.Argument.Name} requires exactly one argument.";
                return null;
            }

            return new DirectoryInfo(result.Tokens[0].Value);
        }

        public static FileInfo? ParseFileInfo(ArgumentResult result)
        {
            if (result.Tokens.Count != 1)
            {
                result.ErrorMessage = $"--{result.Argument.Name} requires exactly one argument.";
                return null;
            }

            return new FileInfo(result.Tokens[0].Value);
        }

        public static IFileSystemInfoIO? ParseFileSystemInfo(ArgumentResult result)
        {
            if (result.Tokens.Count != 1)
            {
                result.ErrorMessage = $"--{result.Argument.Name} requires exactly one argument.";
                return null;
            }

            string tokenValue = result.Tokens[0].Value;
            if (FileSystem.Instance.File.GetAttributes(tokenValue).HasFlag(System.IO.FileAttributes.Directory))
            {
                return new DirectoryInfo(tokenValue);
            }

            return new FileInfo(tokenValue);
        }
    }
}
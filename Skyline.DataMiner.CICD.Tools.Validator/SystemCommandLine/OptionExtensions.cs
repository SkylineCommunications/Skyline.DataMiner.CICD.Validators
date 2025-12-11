// ReSharper disable UnusedMember.Global
namespace Skyline.DataMiner.CICD.Tools.Validator.SystemCommandLine
{
    using Skyline.DataMiner.CICD.FileSystem;

    internal static class OptionExtensions
    {
        /// <summary>
        /// Configures an option to accept only values corresponding to an existing file.
        /// </summary>
        /// <param name="option">The option to configure.</param>
        /// <returns>The option being extended.</returns>
        public static Option<IFileSystemInfoIO?> ExistingOnly(this Option<IFileSystemInfoIO?> option)
        {
            option.AddValidator(FileOrDirectoryExists);
            return option;
        }

        /// <summary>
        /// Configures an option to accept only values corresponding to an existing file.
        /// </summary>
        /// <param name="option">The option to configure.</param>
        /// <returns>The option being extended.</returns>
        public static Option<IFileInfoIO?> ExistingOnly(this Option<IFileInfoIO?> option)
        {
            option.AddValidator(FileExists);
            return option;
        }

        /// <summary>
        /// Configures an option to accept only values corresponding to an existing directory.
        /// </summary>
        /// <param name="option">The option to configure.</param>
        /// <returns>The option being extended.</returns>
        public static Option<IDirectoryInfoIO?> ExistingOnly(this Option<IDirectoryInfoIO?> option)
        {
            option.AddValidator(DirectoryExists);
            return option;
        }

        private static void DirectoryExists(OptionResult result)
        {
            foreach (Token token in result.Tokens)
            {
                if (!FileSystem.Instance.Directory.Exists(token.Value))
                {
                    result.ErrorMessage = result.LocalizationResources.DirectoryDoesNotExist(token.Value);
                    return;
                }
            }
        }

        private static void FileExists(OptionResult result)
        {
            foreach (Token token in result.Tokens)
            {
                if (!FileSystem.Instance.File.Exists(token.Value))
                {
                    result.ErrorMessage = result.LocalizationResources.FileDoesNotExist(token.Value);
                    return;
                }
            }
        }

        private static void FileOrDirectoryExists(OptionResult result)
        {
            foreach (Token token in result.Tokens)
            {
                if (FileSystem.Instance.File.Exists(token.Value))
                {
                    continue;
                }

                if (FileSystem.Instance.Directory.Exists(token.Value))
                {
                    continue;
                }

                result.ErrorMessage = result.LocalizationResources.FileOrDirectoryDoesNotExist(token.Value);
                return;
            }
        }
    }
}
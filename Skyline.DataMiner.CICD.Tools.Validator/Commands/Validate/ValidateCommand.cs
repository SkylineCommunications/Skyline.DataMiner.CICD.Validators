namespace Skyline.DataMiner.CICD.Tools.Validator.Commands.Validate
{
    internal class ValidateCommand : Command
    {
        public ValidateCommand() : base(name: "validate", description: "Validates a DataMiner solution.")
        {
            AddCommand(new ValidateProtocolSolutionCommand());
        }
    }
}
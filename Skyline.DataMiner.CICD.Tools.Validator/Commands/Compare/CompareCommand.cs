namespace Skyline.DataMiner.CICD.Tools.Validator.Commands.Compare
{
    internal class CompareCommand : Command
    {
        public CompareCommand() : base(name: "compare", description: "Compares a DataMiner solution with the previous version.")
        {
            AddCommand(new CompareProtocolSolutionCommand());
        }
    }
}
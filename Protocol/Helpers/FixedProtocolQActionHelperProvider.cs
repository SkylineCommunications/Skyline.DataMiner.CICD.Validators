namespace SLDisValidator2.Helpers
{
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;

    public class FixedProtocolQActionHelperProvider : IProtocolQActionHelperProvider
    {
        private readonly string _qactionHelper;

        public FixedProtocolQActionHelperProvider(string qactionHelper)
        {
            _qactionHelper = qactionHelper;
        }

        public string GetProtocolQActionHelper(string protocolCode, bool ignoreErrors = false)
        {
            return _qactionHelper;
        }
    }
}

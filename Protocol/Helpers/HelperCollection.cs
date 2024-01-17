namespace Skyline.DataMiner.CICD.Validators.Protocol.Helpers
{
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests;

    internal class HelperCollection
    {
        public HelperCollection(ValidatorSettings settings)
        {
            TitleCasing = new TitleCasing(settings);
        }

        public TitleCasing TitleCasing { get; }
    }
}
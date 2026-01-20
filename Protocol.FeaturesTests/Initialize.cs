namespace Protocol.FeaturesTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis.Host.Mef;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class Initialize
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            try
            {
                // warm up some Roslyn classes, to avoid that unit test fail
                var defaultServices = MSBuildMefHostServices.DefaultServices;
                var defaultHost = MefHostServices.DefaultHost;
            }
            catch (ReflectionTypeLoadException tle)
            {
                string text = String.Join(";", tle.LoaderExceptions.Select(x => x.Message));
                throw new Exception($"ReflectionTypeLoadException with these LoaderExceptions:{Environment.NewLine}{text}");
            }
        }
    }
}

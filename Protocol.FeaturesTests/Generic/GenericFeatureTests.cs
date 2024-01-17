namespace Protocol.FeaturesTests.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Attributes;
    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;

    [TestClass]
    public class GenericFeatureTests
    {
        private static List<(IFeatureCheck feature, MinDataMinerVersionsAttribute minVersion, MaxDataMinerVersionsAttribute maxVersion)> features;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            features = new List<(IFeatureCheck feature, MinDataMinerVersionsAttribute minVersion, MaxDataMinerVersionsAttribute maxVersion)>();

            // TODO: Can be tweaked later, maybe with inheritance?
            var minFeatures = MinDataMinerVersionsAttribute.GetFeatures();
            var maxFeatures = MaxDataMinerVersionsAttribute.GetFeatures();

            foreach ((IFeatureCheck feature, MinDataMinerVersionsAttribute version) in minFeatures)
            {
                // Not ideal, but we'll need to see if we implement a comparer or something that makes it unique.
                var temp = maxFeatures.FirstOrDefault(x => x.feature?.Title == feature.Title);

                // temp will not be null, but the inner 'fields' will be default
                features.Add((feature, version, temp.version));
            }
        }

        [TestMethod]
        public void CheckValidVersion()
        {
            List<IFeature> invalidFeatures = new List<IFeature>();
            foreach ((IFeatureCheck feature, MinDataMinerVersionsAttribute minVersions, MaxDataMinerVersionsAttribute maxVersions) in features)
            {
                if (minVersions.MainRelease?.Version == null || minVersions.FeatureRelease?.Version == null)
                {
                    invalidFeatures.Add(feature);
                    continue;
                }

                if (maxVersions != null && (maxVersions.MainRelease?.Version == null || maxVersions.FeatureRelease?.Version == null))
                {
                    invalidFeatures.Add(feature);
                }
            }

            if (invalidFeatures.Count <= 0)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Environment.NewLine + "Features with invalid DataMiner version(s):");

            foreach (IFeature f in invalidFeatures)
            {
                sb.AppendLine("\t" + f.GetType().Name);
            }

            Assert.Fail(sb.ToString());
        }

        [TestMethod]
        public void CheckValidBuildNumber()
        {
            List<IFeature> missingBuildNumbers = new List<IFeature>();
            foreach ((IFeatureCheck feature, MinDataMinerVersionsAttribute minVersions, MaxDataMinerVersionsAttribute maxVersions) in features)
            {
                if (minVersions.MainRelease != null && minVersions.MainRelease.Iteration == 0)
                {
                    missingBuildNumbers.Add(feature);
                    continue;
                }

                if (minVersions.FeatureRelease != null && minVersions.FeatureRelease.Iteration == 0)
                {
                    missingBuildNumbers.Add(feature);
                    continue;
                }

                if (maxVersions == null)
                {
                    continue;
                }

                if (maxVersions.MainRelease != null && maxVersions.MainRelease.Iteration == 0)
                {
                    missingBuildNumbers.Add(feature);
                    continue;
                }

                if (maxVersions.FeatureRelease != null && maxVersions.FeatureRelease.Iteration == 0)
                {
                    missingBuildNumbers.Add(feature);
                }
            }

            if (missingBuildNumbers.Count <= 0)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Environment.NewLine + "Features with missing build number(s):");

            foreach (IFeature f in missingBuildNumbers)
            {
                sb.AppendLine("\t" + f.GetType().Name);
            }

            Assert.Inconclusive(sb.ToString());
        }

        [TestMethod]
        public void CheckDescription()
        {
            List<IFeature> invalidFeatures = new List<IFeature>();
            foreach ((IFeatureCheck feature, _, _) in features)
            {
                try
                {
                    // Will check if null, throwing exception or just empty
                    if (feature.Description.Equals(String.Empty))
                    {
                        invalidFeatures.Add(feature);
                    }
                }
                catch (Exception e) when (e is NullReferenceException || e is NotImplementedException)
                {
                    invalidFeatures.Add(feature);
                }
            }

            if (invalidFeatures.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(Environment.NewLine + "Features with not implemented descriptions:");

                foreach (IFeature f in invalidFeatures)
                {
                    sb.AppendLine("\t" + f.GetType().Name);
                }

                Assert.Fail(sb.ToString());
            }
        }

        [TestMethod]
        public void CheckReleaseNotes()
        {
            List<IFeature> invalidFeatures = new List<IFeature>();
            foreach ((IFeatureCheck feature, _, _) in features)
            {
                // We currently didn't manage to find RNs for Class Library Ranges.
                // JanS should be able to provide us with those but didn't fine the time of it yet.
                if (feature.Title?.StartsWith("Class Library Range") == true)
                {
                    continue;
                }

                try
                {
                    // Will check if null, throwing exception or just empty
                    if (feature.ReleaseNotes.Count == 0)
                    {
                        invalidFeatures.Add(feature);
                    }
                }
                catch (Exception e) when (e is NullReferenceException || e is NotImplementedException)
                {
                    invalidFeatures.Add(feature);
                }
            }

            if (invalidFeatures.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(Environment.NewLine + "Features with not implemented release notes:");

                foreach (IFeature f in invalidFeatures)
                {
                    sb.AppendLine("\t" + f.GetType().Name);
                }

                Assert.Fail(sb.ToString());
            }
        }
    }
}
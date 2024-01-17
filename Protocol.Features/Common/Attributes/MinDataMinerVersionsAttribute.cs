namespace Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Skyline.DataMiner.CICD.Common;

    using Skyline.DataMiner.CICD.Validators.Protocol.Features.Common.Interfaces;

    /// <summary>
    /// This attribute exists so that we can later on pre-filter on the releases so that we don't run all the features if not needed.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class MinDataMinerVersionsAttribute : Attribute
    {
        public MinDataMinerVersionsAttribute(string mainRelease, string featureRelease)
        {
            if (!String.IsNullOrWhiteSpace(mainRelease))
            {
                MainRelease = DataMinerVersion.Parse(mainRelease);
            }

            if (!String.IsNullOrWhiteSpace(featureRelease))
            {
                FeatureRelease = DataMinerVersion.Parse(featureRelease);
            }
        }

        public DataMinerVersion MainRelease { get; }

        public DataMinerVersion FeatureRelease { get; }

        internal static IEnumerable<(IFeatureCheck feature, MinDataMinerVersionsAttribute version)> GetFeatures()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (!typeof(IFeatureCheck).IsAssignableFrom(type) || !type.IsClass)
                {
                    continue;
                }

                var attr = GetAttribute(type);
                if (attr != null)
                {
                    var feature = (IFeatureCheck)Activator.CreateInstance(type);

                    yield return (feature, attr);
                }
            }
        }

        internal static MinDataMinerVersionsAttribute GetAttribute(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetCustomAttribute(typeof(MinDataMinerVersionsAttribute)) as MinDataMinerVersionsAttribute;
        }

        internal static MinDataMinerVersionsAttribute GetAttribute(IFeatureCheck feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            return GetAttribute(feature.GetType());
        }
    }
}
namespace ProtocolTests.SchemaGenerator
{
    using System;
    using System.IO;
    using System.Reflection;

    internal class Program
    {
        private static void Main(string[] args)
        {
            string solutionDir = String.Join(" ", args);

            var assembly = typeof(SchemaGenerator).Assembly;
            var buildConfigurationName = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;

            var projectName = assembly.GetName().Name;
            string schema;

            try
            {
                string path = Path.Combine(solutionDir, projectName, "bin", buildConfigurationName, "net472", "Skyline", "XSD", "protocol.xsd");
                SchemaGenerator generator = new SchemaGenerator(path);
                schema = generator.CreateSchema();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(-1);
                return;
            }

            try
            {
                File.WriteAllText(Path.Combine(solutionDir, "ProtocolTests", "ValidatorUnitTestProtocols.g.xsd"), schema);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(-2);
            }
        }
    }
}
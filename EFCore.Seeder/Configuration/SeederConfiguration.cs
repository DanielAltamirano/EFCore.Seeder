using System.Reflection;
using CsvHelper.Configuration;
using CsvConfiguration = CsvHelper.Configuration.Configuration;
using EFCore.Seeder.Helpers;
using EFCore.Seeder.Helpers.Interfaces;

namespace EFCore.Seeder.Configuration
{
    public static class SeederConfiguration
    {
        public static IDbSetHelper DbSetHelper = new DbSetHelper();

        public static Assembly Assembly { get; private set; }

        public static CsvConfiguration CsvConfiguration { get; private set; }

        public static ManifestConfiguration ManifestConfiguration { get; private set; }

        public static void ResetConfiguration(CsvConfiguration csvConfiguration = null,
            ManifestConfiguration manifestConfiguration = null, Assembly assembly = null)
        {
            Assembly = assembly;

            CsvConfiguration = csvConfiguration ?? new CsvConfiguration
            {
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true
            };

            ManifestConfiguration = manifestConfiguration ?? new ManifestConfiguration
            {
                Delimiter = ".",
                DelimiterFieldName = "{delimiter}",
                Extension = "csv",
                ExtensionFieldName = "{extension}",
                ResourceFieldName = "{resource}",
                Format = "{delimiter}{resource}{delimiter}{extension}"
            };
        }
    }
}

using System.Globalization;
using System.Reflection;
using EFCore.Seeder.Configuration;
using EFCore.Seeder.Console.Models;
using EFCore.Seeder.Extensions;

namespace EFCore.Seeder.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");

            var csvConfiguration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = ",",
                PrepareHeaderForMatch = (s, i) => s.TrimStart().TrimEnd(),
                HasHeaderRecord = true,
                SanitizeForInjection = true,
                MissingFieldFound = null
            };

            var manifestConfiguration = new ManifestConfiguration
            {
                Format = "{delimiter}{resource}{delimiter}{extension}",
                Delimiter = ".",
                Extension = "csv",
                ExtensionFieldName = "{extension}",
                ResourceFieldName = "{resource}",
                DelimiterFieldName = "{delimiter}"
            };

            SeederConfiguration.ResetConfiguration(csvConfiguration, manifestConfiguration, assembly: typeof(Program).GetTypeInfo().Assembly);

            using (var context = new ProductsDbContext())
            {
                //context.Products.SeedDbSetIfEmpty(nameof(context.Products));
                context.Suppliers.SeedFromResource(nameof(context.Suppliers));
                context.SaveChanges();
            }
        }
    }
}

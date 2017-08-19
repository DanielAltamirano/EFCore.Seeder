using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using EFCore.Seeder.Configuration;
using EFCore.Seeder.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Seeder.Extensions
{
    public static class DbSetExtensions
    {
        /// <summary>
        /// Checks if a DbSet is empty. If so, it seeds it from the manifest.
        /// </summary>
        /// <typeparam name="T">The type of entity to load</typeparam>
        /// <param name="dbSet">The DbSet to populate</param>
        /// <param name="manifestResourceName">Full or partial name of the manifest in the assembly.
        /// Must follow the format specified in the manifest configuration</param>
        public static List<T> SeedDbSetIfEmpty<T>(this DbSet<T> dbSet, string manifestResourceName) where T : class
        {
            return !SeederConfiguration.DbSetHelper.Any(dbSet) ? dbSet.SeedFromResource(manifestResourceName) : null;
        }

        /// <summary>
        /// Seeds a DBSet from a CSV file from the configured assembly.
        /// If there's no configured assembly to retrieve the resource manifests, an exception will be thrown
        /// </summary>
        /// <typeparam name="T">The type of entity to load</typeparam>
        /// <param name="dbSet">The DbSet to populate</param>
        /// <param name="manifestResourceName">Full or partial name of the manifest in the assembly.
        /// Must follow the format specified in the configuration</param>
        /// <param name="additionalMapping">Any additonal complex mappings required</param>
        public static List<T> SeedFromResource<T>(this DbSet<T> dbSet, string manifestResourceName, params CsvColumnMapping<T>[] additionalMapping)
            where T : class
        {
            var manifest = GetResourceManifest(manifestResourceName, SeederConfiguration.Assembly);
            if (string.IsNullOrEmpty(manifest))
            {
                throw new Exception($"Invalid assembly or manifest resource name: {manifestResourceName}");
            }

            using (var stream = SeederConfiguration.Assembly.GetManifestResourceStream(manifest))
            {
                return SeedFromStream(dbSet, stream, additionalMapping);
            }
        }

        /// <summary>
        /// Seeds a DBSet from a CSV file that will be read from the specified stream
        /// </summary>
        /// <typeparam name="T">The type of entity to load</typeparam>
        /// <param name="dbSet">The DbSet to populate</param>
        /// <param name="stream">The stream containing the CSV file contents</param>
        /// <param name="additionalMapping">Any additonal complex mappings required</param>
        public static List<T> SeedFromStream<T>(this DbSet<T> dbSet, Stream stream, params CsvColumnMapping<T>[] additionalMapping)
            where T : class
        {
            try
            {
                using (var reader = new StreamReader(stream))
                {
                    var results = new List<T>();

                    var csvReader = new CsvReader(reader, SeederConfiguration.CsvConfiguration);
                    var map = csvReader.Configuration.AutoMap<T>();
                    map.ReferenceMaps.Clear();
                    csvReader.Configuration.RegisterClassMap(map);

                    while (csvReader.Read())
                    {
                        var csvEntity = csvReader.GetRecord<T>();
                        foreach (var csvColumnMapping in additionalMapping)
                        {
                            csvColumnMapping.Execute(csvEntity, csvReader.GetField(csvColumnMapping.CsvColumnName));
                        }

                        SeederConfiguration.DbSetHelper.AddOrUpdate(dbSet, csvEntity);

                        results.Add(csvEntity);
                    }

                    return results;
                }
            }
            catch (Exception exception)
            {
                var message = $"Error Seeding DbSet<{typeof(T).Name}>: {exception}";
                throw new Exception(message, exception);
            }
        }

        private static string GetResourceManifest(string manifestResourceName, Assembly assembly)
        {
            if (assembly == null)
            {
                return null;
            }

            // Create the corresponding manifest name to search for
            var manifestFormatted = SeederConfiguration.ManifestConfiguration.Format
                .Replace(SeederConfiguration.ManifestConfiguration.DelimiterFieldName, SeederConfiguration.ManifestConfiguration.Delimiter)
                .Replace(SeederConfiguration.ManifestConfiguration.ResourceFieldName, manifestResourceName)
                .Replace(SeederConfiguration.ManifestConfiguration.ExtensionFieldName, SeederConfiguration.ManifestConfiguration.Extension);

            return assembly.GetManifestResourceNames().FirstOrDefault(s => s.Contains(manifestFormatted));
        }
    }
}

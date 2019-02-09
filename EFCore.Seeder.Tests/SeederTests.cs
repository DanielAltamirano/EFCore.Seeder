using System;
using System.Reflection;
using EFCore.Seeder.Configuration;
using EFCore.Seeder.Entities.Entities;
using EFCore.Seeder.Extensions;
using EFCore.Seeder.Helpers;
using EFCore.Seeder.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EFCore.Seeder.Tests
{
    public class SeederTests
    {
        [Fact]
        public void EntitySeederSeedFromResource_RunsExtensionMethod_WhenCalled()
        {
            #region Setup

            var dbSetHelperMock = new Mock<IDbSetHelper>();
            dbSetHelperMock.Setup(helper => helper.AddOrUpdate(It.IsAny<DbSet<Product>>(), It.IsAny<Product>()));
            var dbSetMock = new Mock<DbSet<Product>>();

            SeederConfiguration.ResetConfiguration(assembly: typeof(SeederTests).GetTypeInfo().Assembly);
            SeederConfiguration.DbSetHelper = dbSetHelperMock.Object;

            #endregion

            var results = dbSetMock.Object.SeedFromResource(nameof(Product));

            dbSetHelperMock.Verify(helper => helper.AddOrUpdate(It.IsAny<DbSet<Product>>(), It.IsAny<Product>()), Times.Once);

            Assert.True(results.Exists(product => product.Code == "ABCDE"));

            var additionalMapping = new[]
            {
                new CsvColumnMapping<Product>("ExtraInformation", (product, extraInfo) => { product.Description = (string) extraInfo; })
            };

            results = dbSetMock.Object.SeedFromResource(nameof(Product), additionalMapping);

            dbSetHelperMock.Verify(helper => helper.AddOrUpdate(It.IsAny<DbSet<Product>>(), It.IsAny<Product>()), Times.Exactly(2));

            Assert.True(results.Exists(product => product.Description == "Extra Information"));
        }

        [Fact]
        public void EntitySeederSeedDbSetIfEmpty_RunsSeedFromResource_WhenCalled()
        {
            #region Setup

            var dbSetHelperMock = new Mock<IDbSetHelper>();
            dbSetHelperMock.Setup(helper => helper.AddOrUpdate(It.IsAny<DbSet<Product>>(), It.IsAny<Product>()));
            dbSetHelperMock.Setup(helper => helper.Any(It.IsAny<DbSet<Product>>())).Returns(false);
            var dbSetMock = new Mock<DbSet<Product>>();

            const string verifiableCode = "ABCDE";

            SeederConfiguration.ResetConfiguration(assembly: typeof(SeederTests).GetTypeInfo().Assembly);
            SeederConfiguration.DbSetHelper = dbSetHelperMock.Object;

            #endregion

            var results = dbSetMock.Object.SeedDbSetIfEmpty(nameof(Product));

            Assert.True(results.Exists(product => product.Code == verifiableCode));
        }

        [Fact]
        public void EntitySeederSeedFromResource_ThrowsException_WhenCalledWithInvalidManifestName()
        {
            #region Setup

            var dbSetHelperMock = new Mock<IDbSetHelper>();
            dbSetHelperMock.Setup(helper => helper.AddOrUpdate(It.IsAny<DbSet<Product>>(), It.IsAny<Product>()));
            var dbSetMock = new Mock<DbSet<Product>>();

            SeederConfiguration.ResetConfiguration(assembly: typeof(SeederTests).GetTypeInfo().Assembly);
            SeederConfiguration.DbSetHelper = dbSetHelperMock.Object;

            #endregion

            const string invalidManifestResourceName = "InvalidManifestName";

            var exception = Assert.Throws<Exception>(() => dbSetMock.Object.SeedFromResource(invalidManifestResourceName));

            dbSetHelperMock.Verify(helper => helper.AddOrUpdate(It.IsAny<DbSet<Product>>(), It.IsAny<Product>()), Times.Never);

            Assert.Contains($"Invalid assembly or manifest resource name: {invalidManifestResourceName}", exception.Message);
        }

        [Fact]
        public void EntitySeederSeedFromResource_ThrowsException_WhenCalledWithNullAssembly()
        {
            #region Setup

            var dbSetHelperMock = new Mock<IDbSetHelper>();
            dbSetHelperMock.Setup(helper => helper.AddOrUpdate(It.IsAny<DbSet<Product>>(), It.IsAny<Product>()));
            var dbSetMock = new Mock<DbSet<Product>>();

            SeederConfiguration.ResetConfiguration();
            SeederConfiguration.DbSetHelper = dbSetHelperMock.Object;

            #endregion

            const string invalidManifestResourceName = "InvalidManifestName";

            var exception = Assert.Throws<Exception>(() => dbSetMock.Object.SeedFromResource(invalidManifestResourceName));

            dbSetHelperMock.Verify(helper => helper.AddOrUpdate(It.IsAny<DbSet<Product>>(), It.IsAny<Product>()), Times.Never);

            Assert.Contains($"Invalid assembly or manifest resource name: {invalidManifestResourceName}", exception.Message);
        }

        [Fact]
        public void EntitySeederSeedFromResource_ThrowsException_WhenCalled()
        {
            #region Setup
            var dbSetHelperMock = new Mock<IDbSetHelper>();
            dbSetHelperMock.Setup(helper => helper.AddOrUpdate(It.IsAny<DbSet<Product>>(), It.IsAny<Product>()))
                .Throws(new Exception());
            var dbSetMock = new Mock<DbSet<Product>>();

            SeederConfiguration.ResetConfiguration(assembly: typeof(SeederTests).GetTypeInfo().Assembly);
            SeederConfiguration.DbSetHelper = dbSetHelperMock.Object;

            #endregion

            var exception = Assert.Throws<Exception>(() => dbSetMock.Object.SeedFromResource(nameof(Product)));

            dbSetHelperMock.Verify(helper => helper.AddOrUpdate(It.IsAny<DbSet<Product>>(), It.IsAny<Product>()), Times.Once);

            Assert.Contains("Error Seeding DbSet<Product>", exception.Message);
        }
    }
}

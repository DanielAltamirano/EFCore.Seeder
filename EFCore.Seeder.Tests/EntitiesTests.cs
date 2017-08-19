using EFCore.Seeder.Entities.Entities;
using Xunit;

namespace EFCore.Seeder.Tests
{
    public class EntitiesTests
    {
        [Fact]
        public void EntitiesBank_ComparesCorrectly_WhenCalled()
        {
            const string code = "Code";
            const string name = "Product!";
            const string description = "Nice product!";

            var bank = new Product
            {
                Code = code,
                Name = name,
                Id = 1,
                Description = description
            };

            Assert.True(bank.Equals(new Product
            {
                Code = code
            }));
        }
    }
}

using EFCore.Seeder.Helpers;
using Xunit;

namespace EFCore.Seeder.Tests
{
    public class CsvColumnMappingTests
    {
        [Fact]
        public void CsvColumnMappingExecute_ExecutesAction_WhenCalled()
        {
            // Define variables and constants
            var actionWasInvoked = false;
            const string columnName = "CsvColumnName";
            void Action(string s, object o) => actionWasInvoked = true;

            // Create instance
            var mapping = new CsvColumnMapping<string>(columnName, Action);

            // Run method
            mapping.Execute(string.Empty, string.Empty);

            // Verify test conditions
            Assert.True(actionWasInvoked);
            Assert.Equal(mapping.CsvColumnName, columnName);
        }
    }
}

using System;
using CsvHelper.Configuration.Attributes;

namespace EFCore.Seeder.Console.Models
{
    public class Product : IEquatable<Product>
    {
        [Ignore]
        public int Id { get; set; }
        public string Name { get; }
        public string Description { get; set; }

        public bool Equals(Product other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Product) obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}

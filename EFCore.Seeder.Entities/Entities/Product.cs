using System;

namespace EFCore.Seeder.Entities.Entities
{
    public class Product : IEquatable<Product>
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Equals(Product other)
        {
            return Code == other.Code;
        }
    }
}

# EFCore.Seeder
![alt text][logo]

A library to seed databases from CSV files, using .NET Core 2.0 and Entity Framework Core 2.2.1

A project based on [dpaquette/EntityFramework.Seeder](https://github.com/dpaquette/EntityFramework.Seeder), using EntityFramework Core instead of EF6, plus some improvements on how to handle resource files.

## How to install

`Install-Package EFCore.Seeder`

## How to use

### Configuration and setup

This library requires all resource files (CSV files) to be added as `Embedded Resource` in a runtime available assembly. Once that requirement is met, the seeder needs to be configured using:

`SeederConfiguration.ResetConfiguration(assembly: ResourceAssembly);`

In order to make it easier, you can reference a type in that assembly and let Reflection take care of retrieving the assembly instance

`SeederConfiguration.ResetConfiguration(assembly: typeof(<Assembly Class or Type>).GetTypeInfo().Assembly);`

Also, if the format for the CSV files needs to be changed, or the CsvHelper configuration needs some tweaking, this is where we need to pass in the new parameters:

`SeederConfiguration.ResetConfiguration(csvConfiguration: csvConfiguration, manifestConfiguration: manifestConfiguration, assembly: typeof(PayrollContext).GetTypeInfo().Assembly);`

Where `csvConfiguration` is an instance of `CsvConfiguration`, from the `CsvHelper` library, and `manifestConfiguration` is an instance of `ManifestConfiguration`. The last one is just a way to format the embedded resource file names to make them easy to find in the assembly. You can store the configuration in a json file and load it when setting everything up.

```
"manifestConfiguration": {
        "delimiterFieldName": "{delimiter}",
        "resourceFieldName": "{resource}",
        "extensionFieldName": "{extension}",
        "format": "{delimiter}{resource}{delimiter}{extension}",
        "delimiter": ".",
        "extension": "csv"
    }
```

### Usage

Once the Seeder is configured, all we need to do is call the appropiate extension method when accessing a DbSet for a particular type.

Let's say we have a DbContext with a `Products` DbSet. We could do the following:

`dbContext.Products.SeedDbSetIfEmpty(nameof(context.Products));`

This would assume that we have a `Products.csv` file in the configured assembly, with all the required information to load into the Products entity.

Also, please note that if we're going to update information as well as insert it, the `Products` entity must implement the `IEquatable<T>` interface, so we can use `product.Equal(other)` when finding the right entity to update. This is due to Entity Framework Core not having an `AddOrUpdate` method (as to this moment, maybe this has changed in EF Core 2.0).

Please check out the test projects for more information on how to use the library.

[logo]: https://github.com/DanielAltamirano/EFCore.Seeder/raw/master/EFSeederIcon.png "EFCore.Seeder"

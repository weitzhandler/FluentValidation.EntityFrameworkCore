[![](https://img.shields.io/nuget/v/Weitzhandler.FluentValidaiton.EntityFrameworkCore)](https://www.nuget.org/packages/Weitzhandler.FluentValidaiton.EntityFrameworkCore)

# FluentValidation.EntityFrameworkCore

Loads EF Core column configuration from matching FluentValidation validators registered in assembly.

Usage:

```c#
services
  .AddDbContext<EntityContext>(options => ...)
  .AddValidatorsFromAssemblyContaining<EntityValidator>();
``` 

The following validators are currently supported:

- `INotNullValidator`/`INotEmptyValidator` (e.g. `RuleFor(entity => entity.Property.NotNull())`)  
Annotates the Db column as not-nullable (e.g. `Column (nvarchar(MAX), not null`)
- `ILengthValidator` (e.g. `RuleFor(entity => entity.Property.MaximumLength(m))`)  
Annotates the Db column with a max length annotation (e.g. `varchar(m)`).
- `ExactLengthValidator`  
Annotates the Db column with a fixed length annotation (e.g. `varchar(f)`).
- `ScalePrecisionValidator` (`RuleFor(entity => entity.Property.ScalePrecision(scale: s, precision: p))`)  
Annotates the Db column (if decimal) with the precision and scale
This is only supported in EF Core 5+

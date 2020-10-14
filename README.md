[![](https://img.shields.io/nuget/v/Weitzhandler.FluentValidaiton.EntityFrameworkCore)](https://www.nuget.org/packages/Weitzhandler.FluentValidaiton.EntityFrameworkCore)

# FluentValidation.EntityFrameworkCore

Loads EF Core column configuration from matching FluentValidation validators registered in assembly.

Usage:

Install the [`Weitzhandler.FluentValidation.EntityFrameworkCore`](https://www.nuget.org/packages/Weitzhandler.FluentValidaiton.EntityFrameworkCore) package, and set up `FluentValidation.DependencyInjection` to register all validators:

```c#
services
  .AddDbContext<EntityContext>(options => ...)
  .AddValidatorsFromAssemblyContaining<EntityValidator>();
``` 

Then, in your `DbContext`'s `OnModelCreating` method, call `modelBuilder.ApplyConfigurationFromFluentValdations(serviceProvider)`:

```c#
public class EntityContext : DbContext
{    
    private readonly IServiceProvider serviceProvider;

    public EntityContext(DbContextOptions<EntityContext> options, IServiceProvider serviceProvider)
        : base(options)
    {
        this.serviceProvider = serviceProvider;            
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationFromFluentValidations(serviceProvider);
    }
}
```
Note: we need a reference to the service provider so that we obtain the registered `IValidator<>`s from it.

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

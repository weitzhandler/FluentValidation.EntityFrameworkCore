namespace FluentValidation.EntityFrameworkCore.Tests.Models
{
    public class EntityValidator : AbstractValidator<Entity>
    {
        public EntityValidator()
        {
            RuleFor(c => c.NotNull)
                .NotNull();

            RuleFor(c => c.MaxLength10)
                .MaximumLength(maximumLength: 10);

            RuleFor(e => e.FixedLength10)
                .Length(10);

            RuleFor(entity => entity.Precision5_Scale4)
                .ScalePrecision(scale: 4, precision: 5);
        }
    }
}
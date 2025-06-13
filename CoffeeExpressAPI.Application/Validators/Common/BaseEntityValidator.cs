using FluentValidation;


namespace CoffeeExpressAPI.Application.Validators.Common
{
    /// <summary>
    /// Validador base común para entidades con ID
    /// </summary>
    public abstract class BaseEntityValidator<T> : AbstractValidator<T> where T : class
    {
        /// <summary>
        /// Regla común para validar IDs numéricos
        /// </summary>
        protected void ConfigureIdValidation<TProperty>(System.Linq.Expressions.Expression<Func<T, TProperty>> expression)
            where TProperty : struct, IComparable<TProperty>
        {
            RuleFor(expression)
                .Must(id => id.CompareTo(default(TProperty)) > 0)
                .WithMessage("El ID debe ser mayor a 0");
        }

        /// <summary>
        /// Regla común para validar nombres
        /// </summary>
        protected void ConfigureNameValidation(System.Linq.Expressions.Expression<Func<T, string>> expression)
        {
            RuleFor(expression)
                .NotEmpty()
                .WithMessage("El nombre es obligatorio")
                .MinimumLength(2)
                .WithMessage("El nombre debe tener al menos 2 caracteres")
                .MaximumLength(200)
                .WithMessage("El nombre no puede exceder 200 caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
                .WithMessage("El nombre solo puede contener letras y espacios");
        }

        /// <summary>
        /// Regla común para validar precios
        /// </summary>
        protected void ConfigurePriceValidation(System.Linq.Expressions.Expression<Func<T, decimal>> expression)
        {
            RuleFor(expression)
                .GreaterThan(0)
                .WithMessage("El precio debe ser mayor a 0")
                .LessThanOrEqualTo(10000)
                .WithMessage("El precio no puede exceder $10,000")
                .Must(price => price.ToString("F2").Length <= 10)
                .WithMessage("El precio tiene un formato inválido");
        }

        /// <summary>
        /// Regla común para validar emails
        /// </summary>
        protected void ConfigureEmailValidation(System.Linq.Expressions.Expression<Func<T, string>> expression)
        {
            RuleFor(expression)
                .NotEmpty()
                .WithMessage("El email es obligatorio")
                .EmailAddress()
                .WithMessage("El formato del email no es válido")
                .MaximumLength(254)
                .WithMessage("El email no puede exceder 254 caracteres");
        }
    }
}

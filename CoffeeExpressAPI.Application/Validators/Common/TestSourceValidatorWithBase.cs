using FluentValidation;
using CoffeeExpressAPI.Application.Validators.Common;
using CoffeeExpressAPI.Application.Mappings;
using static CoffeeExpressAPI.Application.Mappings.ApplicationMappingProfile;

namespace CoffeeExpressAPI.Application.Validators.Common;

/// <summary>
/// Ejemplo de cómo usar el BaseEntityValidator
/// Este validador hereda funciones comunes del validador base
/// </summary>
public class TestSourceValidatorWithBase : BaseEntityValidator<TestSource>
{
    public TestSourceValidatorWithBase()
    {
        // Usar las validaciones comunes del validador base
        ConfigureIdValidation(x => x.Id);
        ConfigureNameValidation(x => x.Name);
        ConfigurePriceValidation(x => x.Price);

        // Validaciones específicas para TestSource
        RuleFor(x => x.CreatedAt)
            .NotEmpty()
            .WithMessage("La fecha de creación es obligatoria")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("La fecha de creación no puede ser futura");
    }
}

/// <summary>
/// Ejemplo de validador personalizado para un DTO específico
/// </summary>
public class ProductRequestValidator : BaseEntityValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        // Usar validaciones comunes
        ConfigureNameValidation(x => x.ProductName);
        ConfigurePriceValidation(x => x.Price);

        // Validaciones específicas
        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("La categoría es obligatoria")
            .Must(BeAValidCategory)
            .WithMessage("La categoría debe ser: Coffee, Tea, Snack, o Dessert");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("La descripción no puede exceder 1000 caracteres");
    }

    private bool BeAValidCategory(string category)
    {
        var validCategories = new[] { "Coffee", "Tea", "Snack", "Dessert" };
        return validCategories.Contains(category, StringComparer.OrdinalIgnoreCase);
    }
}

// Clase de ejemplo para demostrar el validador
public class ProductRequest
{
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
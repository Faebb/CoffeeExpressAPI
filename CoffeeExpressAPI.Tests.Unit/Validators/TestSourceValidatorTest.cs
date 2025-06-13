using CoffeeExpressAPI.Application.Validators.Common;
using CoffeeExpressAPI.Application.Mappings;
using FluentValidation.TestHelper;
using System.Security.Cryptography.X509Certificates;
using static CoffeeExpressAPI.Application.Mappings.ApplicationMappingProfile;

namespace CoffeeExpressAPI.Tests.Unit.Validators
{
    public class TestSourceValidatorTest
    {
        private readonly TestSourceValidator _validator;

        public TestSourceValidatorTest()
        {
            _validator = new TestSourceValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            //Arrange
            var TestSource = new ApplicationMappingProfile.TestSource
            {
                Id = 1,
                Name = "",
                Price = 10.50m,
                CreatedAt = DateTime.UtcNow,
            };

            //Act
            var result = _validator.TestValidate(TestSource);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("El nombre del producto es obligatorio");
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Short()
        {
            // Arrange
            var testSource = new TestSource
            {
                Id = 1,
                Name = "AB", 
                Price = 10.50m,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(testSource);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("El nombre debe tener al menos 3 caracteres");
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_Zero_Or_Negative()
        {
            // Arrange
            var testSource = new TestSource
            {
                Id = 1,
                Name = "Café Válido",
                Price = -5.00m, 
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(testSource);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Price)
                  .WithErrorMessage("El precio debe ser mayor a 0");
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Zero_Or_Negative()
        {
            // Arrange
            var testSource = new TestSource
            {
                Id = 0, 
                Name = "Café Válido",
                Price = 10.50m,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(testSource);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("El ID debe ser mayor a 0");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_All_Properties_Are_Valid()
        {
            // Arrange
            var testSource = new TestSource
            {
                Id = 1,
                Name = "Café Espresso Colombiano",
                Price = 4.50m,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(testSource);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("ABC")]
        [InlineData("Café Americano")]
        [InlineData("Cappuccino con Leche de Almendras")]
        public void Should_Accept_Valid_Names(string validName)
        {
            // Arrange
            var testSource = new TestSource
            {
                Id = 1,
                Name = validName,
                Price = 5.00m,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(testSource);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [InlineData(0.01)] // Mínimo válido
        [InlineData(4.50)]
        [InlineData(999.99)] // Máximo válido
        public void Should_Accept_Valid_Prices(decimal validPrice)
        {
            // Arrange
            var testSource = new TestSource
            {
                Id = 1,
                Name = "Café Válido",
                Price = validPrice,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(testSource);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Price);
        }
    }
}

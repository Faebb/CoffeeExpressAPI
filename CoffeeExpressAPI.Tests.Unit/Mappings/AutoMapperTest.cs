using AutoMapper;
using FluentAssertions;
using CoffeeExpressAPI.Application.Mappings;
using static CoffeeExpressAPI.Application.Mappings.ApplicationMappingProfile;

namespace CoffeeExpressAPI.Tests.Unit.Mappings;

public class AutoMapperTests
{
    private readonly IMapper _mapper;
    private readonly MapperConfiguration _configuration;

    public AutoMapperTests()
    {
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ApplicationMappingProfile>();
        });

        _mapper = _configuration.CreateMapper();
    }

    [Fact]
    public void AutoMapper_Configuration_Should_Be_Valid()
    {
        // Act & Assert
        _configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void Should_Map_TestSource_To_TestDestination_Correctly()
    {
        // Arrange
        var source = new TestSource
        {
            Id = 123,
            Name = "Café Espresso Premium",
            Price = 5.75m,
            CreatedAt = new DateTime(2025, 6, 7, 15, 30, 0)
        };

        // Act
        var destination = _mapper.Map<TestDestination>(source);

        // Assert
        destination.Should().NotBeNull();
        destination.Id.Should().Be(source.Id);
        destination.ProductName.Should().Be(source.Name);
        destination.Price.Should().Be(source.Price);
        destination.CreatedDate.Should().Be("2025-06-07 15:30:00");
    }

    [Fact]
    public void Should_Map_Multiple_Objects_Correctly()
    {
        // Arrange
        var sources = new List<TestSource>
        {
            new TestSource
            {
                Id = 1,
                Name = "Americano",
                Price = 3.50m,
                CreatedAt = DateTime.UtcNow
            },
            new TestSource
            {
                Id = 2,
                Name = "Cappuccino",
                Price = 4.25m,
                CreatedAt = DateTime.UtcNow
            }
        };

        // Act
        var destinations = _mapper.Map<List<TestDestination>>(sources);

        // Assert
        destinations.Should().HaveCount(2);
        destinations[0].ProductName.Should().Be("Americano");
        destinations[1].ProductName.Should().Be("Cappuccino");
    }

    [Theory]
    [InlineData("Café Simple", "Café Simple")]
    [InlineData("Latte Macchiato", "Latte Macchiato")]
    [InlineData("", "")]
    public void Should_Map_Name_To_ProductName_Correctly(string inputName, string expectedProductName)
    {
        // Arrange
        var source = new TestSource
        {
            Id = 1,
            Name = inputName,
            Price = 4.00m,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var destination = _mapper.Map<TestDestination>(source);

        // Assert
        destination.ProductName.Should().Be(expectedProductName);
    }

    [Fact]
    public void Should_Handle_Null_Source()
    {
        // Arrange
        TestSource? source = null;

        // Act
        var destination = _mapper.Map<TestDestination>(source);

        // Assert
        destination.Should().BeNull();
    }

    [Fact]
    public void Should_Format_DateTime_Correctly()
    {
        // Arrange
        var specificDate = new DateTime(2025, 12, 25, 10, 30, 45);
        var source = new TestSource
        {
            Id = 1,
            Name = "Test Coffee",
            Price = 4.00m,
            CreatedAt = specificDate
        };

        // Act
        var destination = _mapper.Map<TestDestination>(source);

        // Assert
        destination.CreatedDate.Should().Be("2025-12-25 10:30:45");
    }
}
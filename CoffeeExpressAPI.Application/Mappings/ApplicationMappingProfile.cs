using AutoMapper;

namespace CoffeeExpressAPI.Application.Mappings
{
    /// <summary>
    /// Perfil principal de AutoMapper para la aplicación.
    /// Define los mapeos entre entidades del dominio y DTOs.
    /// </summary>
    public class ApplicationMappingProfile : Profile
    {

        public ApplicationMappingProfile()
        {
            CreateMaps();
            CreateTestMaps();
        }

        private void CreateMaps()
        {
            // TODO: Mapeos se agregarán en Sprint 1.2 cuando creemos las entidades

            // Ejemplo de mapeos que implementaremos:
            // CreateMap<Coffee, CoffeeDto>();
            // CreateMap<CreateCoffeeCommand, Coffee>();
            // CreateMap<Order, OrderDto>()
            //     .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            //     .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            // CreateMap<Customer, CustomerDto>();
            // CreateMap<OrderItem, OrderItemDto>();
        }

        private void CreateTestMaps()
        {
            // Mapeos de prueba para verificar que AutoMapper funciona
            CreateMap<TestSource, TestDestination>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")));
        }

        // Clases de prueba para demostrar AutoMapper (temporales - se eliminarán en Sprint 1.2)
        public class TestSource
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class TestDestination
        {
            public int Id { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string CreatedDate { get; set; } = string.Empty;
        }
    }
}

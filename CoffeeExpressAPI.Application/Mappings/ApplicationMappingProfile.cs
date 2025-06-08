using AutoMapper;

namespace CoffeeExpressAPI.Application.Mappings
{
    internal class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {

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
    }
}

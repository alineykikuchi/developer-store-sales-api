using Ambev.DeveloperEvaluation.Application.Sales.AddItemToSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddItemToSale
{
    /// <summary>
    /// Profile for mapping between AddItemToSale-related requests, commands, results and responses
    /// </summary>
    public class AddItemToSaleProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for AddItemToSale operations
        /// </summary>
        public AddItemToSaleProfile()
        {
            // Request to Command mappings
            CreateMap<AddItemToSaleRequest, AddItemToSaleCommand>()
                .ForMember(dest => dest.SaleId, opt => opt.Ignore()); 

            CreateMap<AddItemToSaleProductRequest, AddItemToSaleProduct>();

            // Result to Response mappings
            CreateMap<AddItemToSaleResult, AddItemToSaleResponse>();

            CreateMap<AddItemToSaleItemResult, AddItemToSaleItemResponse>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => new AddItemToSaleProductResponse
                {
                    Id = src.ProductId,
                    Name = src.ProductName,
                    Description = src.ProductDescription
                }));

        }
    }
}

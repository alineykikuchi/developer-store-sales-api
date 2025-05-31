using Ambev.DeveloperEvaluation.Application.Sales.AddItemToSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddItemToSale
{
    /// <summary>
    /// Profile for mapping between AddItemToSale-related requests, commands, results and responses
    /// </summary>
    public class AddItemToSaleRequestProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for AddItemToSale operations
        /// </summary>
        public AddItemToSaleRequestProfile()
        {
            // Request to Command mappings
            CreateMap<AddItemToSaleRequest, AddItemToSaleCommand>()
                .ForMember(dest => dest.SaleId, opt => opt.Ignore()); // Will be set manually in controller

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

            // Domain Entity to Result mappings
            CreateMap<SaleItem, AddItemToSaleItemResult>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
                .ForMember(dest => dest.UnitPriceCurrency, opt => opt.MapFrom(src => src.UnitPrice.Currency))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
                .ForMember(dest => dest.TotalAmountCurrency, opt => opt.MapFrom(src => src.TotalAmount.Currency));
        }
    }
}

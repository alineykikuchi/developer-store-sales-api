using Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.RemoveSaleItem
{
    /// <summary>
    /// Profile for mapping between RemoveSaleItem-related results and responses
    /// </summary>
    public class RemoveSaleItemRequestProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for RemoveSaleItem operations
        /// </summary>
        public RemoveSaleItemRequestProfile()
        {
            // Result to Response mappings
            CreateMap<RemoveSaleItemResult, RemoveSaleItemResponse>();

            CreateMap<RemoveSaleItemDetails, RemoveSaleItemDetailsResponse>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => new RemoveSaleItemProductResponse
                {
                    Id = src.ProductId,
                    Name = src.ProductName,
                    Description = src.ProductDescription
                }))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => new RemoveSaleItemSummaryResponse
                {
                    RemovedQuantity = src.RemovedQuantity,
                    RemovedUnitPrice = src.RemovedUnitPrice,
                    UnitPriceCurrency = src.UnitPriceCurrency,
                    RemovedDiscountPercentage = src.RemovedDiscountPercentage,
                    RemovedTotalAmount = src.RemovedTotalAmount,
                    TotalAmountCurrency = src.TotalAmountCurrency
                }));
        }
    }
}

using Ambev.DeveloperEvaluation.Application.Sales.ModifySaleItem;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySaleItem
{
    /// <summary>
    /// Profile for mapping between ModifySaleItem-related requests, commands, results and responses
    /// </summary>
    public class ModifySaleItemRequestProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for ModifySaleItem operations
        /// </summary>
        public ModifySaleItemRequestProfile()
        {
            // Request to Command mappings
            CreateMap<ModifySaleItemRequest, ModifySaleItemCommand>()
                .ForMember(dest => dest.SaleId, opt => opt.Ignore()) // Will be set manually in controller
                .ForMember(dest => dest.ItemId, opt => opt.Ignore()); // Will be set manually in controller

            // Result to Response mappings
            CreateMap<ModifySaleItemResult, ModifySaleItemResponse>();

            CreateMap<ModifySaleItemDetails, ModifySaleItemDetailsResponse>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => new ModifySaleItemProductResponse
                {
                    Id = src.ProductId,
                    Name = src.ProductName,
                    Description = src.ProductDescription
                }))
                .ForMember(dest => dest.Changes, opt => opt.MapFrom(src => new ModifySaleItemChangesResponse
                {
                    Quantity = new FieldChangeResponse<int>
                    {
                        Previous = src.PreviousQuantity,
                        New = src.NewQuantity,
                        Changed = src.QuantityChanged
                    },
                    UnitPrice = new FieldChangeResponse<decimal>
                    {
                        Previous = src.PreviousUnitPrice,
                        New = src.NewUnitPrice,
                        Changed = src.PriceChanged
                    },
                    DiscountPercentage = new FieldChangeResponse<decimal>
                    {
                        Previous = src.PreviousDiscountPercentage,
                        New = src.NewDiscountPercentage,
                        Changed = src.DiscountChanged
                    },
                    TotalAmount = new FieldChangeResponse<decimal>
                    {
                        Previous = src.PreviousTotalAmount,
                        New = src.NewTotalAmount,
                        Changed = src.PreviousTotalAmount != src.NewTotalAmount
                    }
                }))
                .ForMember(dest => dest.CurrentState, opt => opt.MapFrom(src => new ModifySaleItemCurrentStateResponse
                {
                    Quantity = src.NewQuantity,
                    UnitPrice = src.NewUnitPrice,
                    UnitPriceCurrency = src.UnitPriceCurrency,
                    DiscountPercentage = src.NewDiscountPercentage,
                    TotalAmount = src.NewTotalAmount,
                    TotalAmountCurrency = src.TotalAmountCurrency
                }));
        }
    }

}

using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;
namespace Ambev.DeveloperEvaluation.Application.Sales.ModifySaleItem;

public class ModifySaleItemProfile : Profile
{
    public ModifySaleItemProfile()
    {
        CreateMap<SaleItem, ModifySaleItemDetails>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
            .ForMember(dest => dest.NewQuantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.NewUnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
            .ForMember(dest => dest.UnitPriceCurrency, opt => opt.MapFrom(src => src.UnitPrice.Currency))
            .ForMember(dest => dest.NewDiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
            .ForMember(dest => dest.NewTotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
            .ForMember(dest => dest.TotalAmountCurrency, opt => opt.MapFrom(src => src.TotalAmount.Currency))
            
            .ForMember(dest => dest.PreviousQuantity, opt => opt.Ignore())
            .ForMember(dest => dest.PreviousUnitPrice, opt => opt.Ignore())
            .ForMember(dest => dest.PreviousDiscountPercentage, opt => opt.Ignore())
            .ForMember(dest => dest.PreviousTotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.QuantityChanged, opt => opt.Ignore())
            .ForMember(dest => dest.PriceChanged, opt => opt.Ignore())
            .ForMember(dest => dest.DiscountChanged, opt => opt.Ignore());


        CreateMap<Sale, ModifySaleItemResult>()
                .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SaleNumber, opt => opt.MapFrom(src => src.SaleNumber))
                .ForMember(dest => dest.NewSaleTotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.TotalAmount.Currency))
                .ForMember(dest => dest.TotalItemsCount, opt => opt.MapFrom(src => src.GetTotalItemsCount()))
                .ForMember(dest => dest.HasDiscountedItems, opt => opt.MapFrom(src => src.HasDiscountedItems()))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                
                .ForMember(dest => dest.ModifiedItem, opt => opt.Ignore());
    }
}


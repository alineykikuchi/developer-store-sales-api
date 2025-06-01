using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddItemToSale
{
    public class AddItemToSaleProfile : Profile
    {
        public AddItemToSaleProfile() 
        {
            CreateMap<SaleItem, AddItemToSaleItemResult>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
                .ForMember(dest => dest.UnitPriceCurrency, opt => opt.MapFrom(src => src.UnitPrice.Currency))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
                .ForMember(dest => dest.TotalAmountCurrency, opt => opt.MapFrom(src => src.TotalAmount.Currency));

            CreateMap<Sale, AddItemToSaleResult>()
                .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SaleNumber, opt => opt.MapFrom(src => src.SaleNumber))
                .ForMember(dest => dest.NewSaleTotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.TotalAmount.Currency))
                .ForMember(dest => dest.TotalItemsCount, opt => opt.MapFrom(src => src.GetTotalItemsCount()))
                .ForMember(dest => dest.HasDiscountedItems, opt => opt.MapFrom(src => src.HasDiscountedItems()))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                
                .ForMember(dest => dest.AddedItem, opt => opt.Ignore());
        }
    }
}

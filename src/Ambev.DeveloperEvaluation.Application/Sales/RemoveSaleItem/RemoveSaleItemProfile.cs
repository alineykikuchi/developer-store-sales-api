using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem
{
    public class RemoveSaleItemProfile : Profile
    {
        public RemoveSaleItemProfile() 
        {
            CreateSaleItemToRemoveSaleItemDetailsMap();
            CreateSaleToRemoveSaleItemResultMap();
        }

        /// <summary>
        /// Creates mapping from SaleItem to RemoveSaleItemDetails
        /// </summary>
        private void CreateSaleItemToRemoveSaleItemDetailsMap()
        {
            CreateMap<SaleItem, RemoveSaleItemDetails>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.RemovedQuantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.RemovedUnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
                .ForMember(dest => dest.UnitPriceCurrency, opt => opt.MapFrom(src => src.UnitPrice.Currency))
                .ForMember(dest => dest.RemovedDiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
                .ForMember(dest => dest.RemovedTotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
                .ForMember(dest => dest.TotalAmountCurrency, opt => opt.MapFrom(src => src.TotalAmount.Currency))
                .ForMember(dest => dest.WasSuccessfullyRemoved, opt => opt.MapFrom(src => false)); 
        }

        /// <summary>
        /// Creates mapping from Sale to RemoveSaleItemResult
        /// </summary>
        private void CreateSaleToRemoveSaleItemResultMap()
        {
            CreateMap<Sale, RemoveSaleItemResult>()
                .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SaleNumber, opt => opt.MapFrom(src => src.SaleNumber))
                .ForMember(dest => dest.NewSaleTotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.TotalAmount.Currency))
                .ForMember(dest => dest.TotalItemsCount, opt => opt.MapFrom(src => src.GetTotalItemsCount()))
                .ForMember(dest => dest.HasDiscountedItems, opt => opt.MapFrom(src => src.HasDiscountedItems()))
                .ForMember(dest => dest.SaleIsEmpty, opt => opt.MapFrom(src => !src.Items.Any()))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.RemovedItem, opt => opt.Ignore()); // This will be mapped separately from the removed SaleItem
        }
    }
}

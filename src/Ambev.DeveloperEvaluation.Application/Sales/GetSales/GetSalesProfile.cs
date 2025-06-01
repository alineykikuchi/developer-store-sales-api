using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
    public class GetSalesProfile : Profile
    {
        public GetSalesProfile() 
        {
            CreateMap<Sale, GetSaleResult>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer.Email))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.Branch.Id))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name))
                .ForMember(dest => dest.BranchAddress, opt => opt.MapFrom(src => src.Branch.Address))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.TotalAmount.Currency))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<SaleItem, GetSaleItemResult>()
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

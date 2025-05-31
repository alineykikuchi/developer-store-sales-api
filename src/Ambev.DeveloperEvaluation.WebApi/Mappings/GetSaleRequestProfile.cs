using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings
{
    /// <summary>
    /// Profile for mapping between GetSale-related results and responses
    /// </summary>
    public class GetSaleRequestProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for GetSale operations
        /// </summary>
        public GetSaleRequestProfile()
        {
            // Result to Response mappings
            CreateMap<GetSaleResult, GetSaleResponse>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => new GetSaleCustomerResponse
                {
                    Id = src.CustomerId,
                    Name = src.CustomerName,
                    Email = src.CustomerEmail
                }))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => new GetSaleBranchResponse
                {
                    Id = src.BranchId,
                    Name = src.BranchName,
                    Address = src.BranchAddress
                }))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<GetSaleItemResult, GetSaleItemResponse>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => new GetSaleProductResponse
                {
                    Id = src.ProductId,
                    Name = src.ProductName,
                    Description = src.ProductDescription
                }));

            // Domain Entity to Result mappings
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

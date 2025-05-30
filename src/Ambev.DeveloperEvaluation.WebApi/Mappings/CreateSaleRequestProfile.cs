using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings
{
    /// <summary>
    /// Profile for mapping between Sale-related requests, commands, results and responses
    /// </summary>
    public class CreateSaleRequestProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for CreateSale operations
        /// </summary>
        public CreateSaleRequestProfile()
        {
            // Request to Command mappings
            CreateMap<CreateSaleRequest, CreateSaleCommand>();
            CreateMap<CreateSaleCustomerRequest, CreateSaleCustomer>();
            CreateMap<CreateSaleBranchRequest, CreateSaleBranch>();
            CreateMap<CreateSaleItemRequest, CreateSaleItem>();

            // Result to Response mappings
            CreateMap<CreateSaleResult, CreateSaleResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateSaleItemResult, CreateSaleItemResponse>();

            // Domain Entity to Result mappings
            CreateMap<Sale, CreateSaleResult>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.TotalAmount.Currency))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<SaleItem, CreateSaleItemResult>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount));
        }
    }
}

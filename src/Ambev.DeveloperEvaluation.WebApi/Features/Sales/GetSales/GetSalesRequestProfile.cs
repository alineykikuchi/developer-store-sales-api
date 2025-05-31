using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales
{
    /// <summary>
    /// Profile for mapping between GetSales-related requests, commands, results and responses
    /// </summary>
    public class GetSalesRequestProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for GetSales operations
        /// </summary>
        public GetSalesRequestProfile()
        {
            // Request to Command mappings
            CreateMap<GetSalesRequest, GetSalesCommand>();

            // Result to Response mappings
            CreateMap<GetSalesResult, GetSalesResponse>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => new GetSalesCustomerResponse
                {
                    Id = src.CustomerId,
                    Name = src.CustomerName,
                    Email = src.CustomerEmail
                }))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => new GetSalesBranchResponse
                {
                    Id = src.BranchId,
                    Name = src.BranchName
                }))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Domain Entity to Result mappings
            CreateMap<Sale, GetSalesResult>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer.Email))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.Branch.Id))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.TotalAmount.Currency));
        }
    }
}

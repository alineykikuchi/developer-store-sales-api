using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
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
        }
    }
}

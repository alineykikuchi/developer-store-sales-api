using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
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
        }
    }
}

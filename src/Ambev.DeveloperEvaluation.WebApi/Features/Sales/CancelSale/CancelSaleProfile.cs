using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale
{
    /// <summary>
    /// Profile for mapping between CancelSale-related results and responses
    /// </summary>
    public class CancelSaleProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for CancelSale operations
        /// </summary>
        public CancelSaleProfile()
        {
            // Result to Response mappings
            CreateMap<CancelSaleResult, CancelSaleResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Domain Entity to Result mappings
            CreateMap<Sale, CancelSaleResult>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.TotalAmount.Currency))
                .ForMember(dest => dest.CancelledAt, opt => opt.MapFrom(src => src.CancelledAt ?? DateTime.UtcNow))
                .ForMember(dest => dest.CancellationReason, opt => opt.Ignore()) // Will be set manually in handler
                .ForMember(dest => dest.WasSuccessfullyCancelled, opt => opt.Ignore()); // Will be set manually in handler
        }
    }
}

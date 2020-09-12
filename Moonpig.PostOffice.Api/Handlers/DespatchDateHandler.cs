namespace Moonpig.PostOffice.Api.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Moonpig.PostOffice.Api.Models;
    using Moonpig.PostOffice.Api.Repositories.Contracts;
    using Moonpig.PostOffice.Api.Services.Contracts;

    public class DespatchDateHandler : IRequestHandler<Order, DespatchDate>
    {
        private readonly IProductRepository productRepository;
        private readonly ISupplierRepository supplierRepository;
        private readonly IDespatchDateService service;

        public DespatchDateHandler(IProductRepository productRepository, ISupplierRepository supplierRepository, IDespatchDateService service)
        {
            this.productRepository = productRepository;
            this.supplierRepository = supplierRepository;
            this.service = service;
        }

        public Task<DespatchDate> Handle(Order order, CancellationToken cancellationToken)
        {
            var doProductsExist = productRepository.DoProductsExist(order.ProductIds);
            if(!doProductsExist)
            {
                return Task.FromResult((DespatchDate)null);
            }

            DateTime baselineDate = service.GetSoonestNonWeekendDate(order.OrderDate);
            DateTime maxLeadTime = baselineDate;

            foreach (var productId in order.ProductIds)
            {
                var supplierId = productRepository.GetSupplierId(productId);
                var supplierLeadTime = supplierRepository.GetLeadTime(supplierId);
                if (baselineDate.AddDays(supplierLeadTime) > maxLeadTime)
                {
                    maxLeadTime = baselineDate.AddDays(supplierLeadTime);
                }
            }

            maxLeadTime = service.GetDateWithWeekendsNotIncludedAsProcessingDays(baselineDate, maxLeadTime);
            
            return Task.FromResult(new DespatchDate { Date = service.GetSoonestNonWeekendDate(maxLeadTime) });
        }
    }
}
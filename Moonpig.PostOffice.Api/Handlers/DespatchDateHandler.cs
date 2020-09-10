namespace Moonpig.PostOffice.Api.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Moonpig.PostOffice.Api.Models;
    using Moonpig.PostOffice.Api.Repositories.Contracts;

    public class DespatchDateHandler : IRequestHandler<Order, DespatchDate>
    {
        private readonly IProductRepository productRepository;
        private readonly ISupplierRepository supplierRepository;

        public DespatchDateHandler(IProductRepository productRepository, ISupplierRepository supplierRepository)
        {
            this.productRepository = productRepository;
            this.supplierRepository = supplierRepository;
        }

        public Task<DespatchDate> Handle(Order order, CancellationToken cancellationToken)
        {
            var doProductsExist = productRepository.DoProductsExist(order.ProductIds);
            if(!doProductsExist)
            {
                return Task.FromResult((DespatchDate)null);
            }
            DateTime maxLeadTime = order.OrderDate;
            foreach (var productId in order.ProductIds)
            {
                var supplierId = productRepository.GetSupplierId(productId);
                var supplierLeadTime = supplierRepository.GetLeadTime(supplierId);
                if (order.OrderDate.AddDays(supplierLeadTime) > maxLeadTime)
                {
                    maxLeadTime = order.OrderDate.AddDays(supplierLeadTime);
                }
            }
            return Task.FromResult(new DespatchDate { Date = AccountForWeekend(maxLeadTime) });
        }

        private DateTime AccountForWeekend(DateTime maxLeadTime)
        {
            if (maxLeadTime.DayOfWeek == DayOfWeek.Saturday)
            {
                maxLeadTime = maxLeadTime.AddDays(2);
            }
            else if (maxLeadTime.DayOfWeek == DayOfWeek.Sunday)
            {
                maxLeadTime = maxLeadTime.AddDays(1);
            }
            return maxLeadTime;
        }
    }
}
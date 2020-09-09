namespace Moonpig.PostOffice.Api.Repositories.Concretes
{
    using System.Linq;
    using Moonpig.PostOffice.Api.Repositories.Contracts;
    using Moonpig.PostOffice.Data;

    public class SupplierRepository : ISupplierRepository
    {
        private readonly IDbContext dbContext;
        public SupplierRepository(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public int GetLeadTime(int supplierId)
        {
            return dbContext.Suppliers.Single(x => x.SupplierId == supplierId).LeadTime;
        }
    }
}
namespace Moonpig.PostOffice.Api.Repositories.Concretes
{
    using System.Linq;
    using Moonpig.PostOffice.Api.Repositories.Contracts;
    using Moonpig.PostOffice.Data;

    public class ProductRepository : IProductRepository
    {
        private readonly IDbContext dbContext;
        public ProductRepository(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public int GetSupplierId(int productId)
        {
            return dbContext.Products.Single(x => x.ProductId == productId).SupplierId;
        }
    }
}
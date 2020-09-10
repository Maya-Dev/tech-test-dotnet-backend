namespace Moonpig.PostOffice.Api.Repositories.Concretes
{
    using System.Collections.Generic;
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

        public bool DoProductsExist(List<int> productIds)
        {
            bool doesProductExist = false;
            foreach(var productId in productIds)
            {
                doesProductExist = dbContext.Products.Any(x => x.ProductId == productId);
                if(!doesProductExist)
                {
                    break;
                }
            }
            return doesProductExist;
        }
    }
}
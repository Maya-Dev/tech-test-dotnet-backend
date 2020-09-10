namespace Moonpig.PostOffice.Api.Repositories.Contracts
{
    using System.Collections.Generic;
    
    public interface IProductRepository
    {
        int GetSupplierId(int productId);
        bool DoProductsExist(List<int> productIds);
    }
}
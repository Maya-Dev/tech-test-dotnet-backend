namespace Moonpig.PostOffice.Api.Repositories.Contracts
{
    public interface ISupplierRepository
    {
        int GetLeadTime(int supplierId);
    }
}
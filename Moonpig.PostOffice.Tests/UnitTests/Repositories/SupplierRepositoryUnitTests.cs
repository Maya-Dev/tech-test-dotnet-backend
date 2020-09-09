namespace Moonpig.PostOffice.Tests.UnitTests.Repositories
{
    using System.Collections.Generic;
    using Shouldly;
    using Xunit;
    using Moq;
    using Moonpig.PostOffice.Data;
    using Moonpig.PostOffice.Api.Repositories.Concretes;
    using System.Linq;

    public class SupplierRepositoryUnitTests
    {
        private readonly Mock<IDbContext> mockDbContext;
        private readonly SupplierRepository repository;
        private IQueryable<Supplier> suppliers;

        public SupplierRepositoryUnitTests()
        {
            mockDbContext = new Mock<IDbContext>();
            repository = new SupplierRepository(mockDbContext.Object);

            suppliers = new List<Supplier>
            {
                new Supplier
                {
                    SupplierId = 1,
                    Name = "SupplierA",
                    LeadTime = 3
                },
                new Supplier
                {
                    SupplierId = 2,
                    Name = "SupplierB",
                    LeadTime = 4
                }
            }.AsQueryable();
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 4)]
        public void Test_GetLeadTime_ReturnsTheCorrectLeadTime_WhenCalled(int supplierId, int expectedLeadTime)
        {
            //Arrange
            mockDbContext.Setup(x => x.Suppliers).Returns(suppliers);

            //Act
            var leadTime = repository.GetLeadTime(supplierId);

            //Assert
            leadTime.ShouldBe<int>(expectedLeadTime);
        }
    }
}
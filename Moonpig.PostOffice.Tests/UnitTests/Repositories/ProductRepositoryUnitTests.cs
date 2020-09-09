namespace Moonpig.PostOffice.Tests.UnitTests.Repositories
{
    using System.Collections.Generic;
    using Shouldly;
    using Xunit;
    using Moq;
    using Moonpig.PostOffice.Data;
    using Moonpig.PostOffice.Api.Repositories.Concretes;
    using System.Linq;

    public class ProductRepositoryUnitTests
    {
        private readonly Mock<IDbContext> mockDbContext;
        private readonly ProductRepository repository;
        private IQueryable<Product> products;

        public ProductRepositoryUnitTests()
        {
            mockDbContext = new Mock<IDbContext>();
            repository = new ProductRepository(mockDbContext.Object);

            products = new List<Product>
            {
                new Product { ProductId = 1, Name = "Greetings Card", SupplierId = 3 },
                new Product { ProductId = 2, Name = "Flowers", SupplierId = 4 }
            }.AsQueryable();
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 4)]
        public void Test_GetSupplierId_ReturnsTheCorrectSupplierId_WhenCalled(int productId, int expectedSupplierId)
        {
            //Arrange
            mockDbContext.Setup(x => x.Products).Returns(products);

            //Act
            var supplierId = repository.GetSupplierId(productId);

            //Assert
            supplierId.ShouldBe<int>(expectedSupplierId);
        }
    }
}
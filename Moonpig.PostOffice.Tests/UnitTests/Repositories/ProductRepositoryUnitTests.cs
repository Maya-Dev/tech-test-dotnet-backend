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

            
            mockDbContext.Setup(x => x.Products).Returns(products);
        }

        // ------------------------------------
        //  GetSupplierId
        // ------------------------------------

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 4)]
        public void Test_GetSupplierId_ReturnsTheCorrectSupplierId_WhenCalled(int productId, int expectedSupplierId)
        {
            //Arrange

            //Act
            var supplierId = repository.GetSupplierId(productId);

            //Assert
            supplierId.ShouldBe<int>(expectedSupplierId);
        }

        // ------------------------------------
        //  DoProductsExist
        // ------------------------------------

        [Fact]
        public void Test_DoProductsExist_ReturnsFalse_WhenCalledWithNoProducts()
        {
            //Arrange

            //Act
            var doProductsExist = repository.DoProductsExist(new List<int>());

            //Assert
            doProductsExist.ShouldBeFalse();
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(18, false)]
        public void Test_DoProductsExist_ReturnsExpectedResponse_WhenCalledWithOneProduct(int productId, bool expectedResponse)
        {
            //Arrange

            //Act
            var doProductsExist = repository.DoProductsExist(new List<int>(){productId});

            //Assert
            doProductsExist.ShouldBe<bool>(expectedResponse);
        }

        [Theory]
        [InlineData(1, 2, true)]
        [InlineData(18, 19, false)]
        [InlineData(18, 1, false)]
        [InlineData(1, 18, false)]
        public void Test_DoProductsExist_ReturnsExpectedResponse_WhenCalledWithTwoProducts(int productIdOne, int productIdTwo, bool expectedResponse)
        {
            //Arrange

            //Act
            var doProductsExist = repository.DoProductsExist(new List<int>(){productIdOne, productIdTwo});

            //Assert
            doProductsExist.ShouldBe<bool>(expectedResponse);
        }
    }
}
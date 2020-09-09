namespace Moonpig.PostOffice.Tests.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using Shouldly;
    using Xunit;
    using Moq;
    using Moonpig.PostOffice.Api.Models;
    using System.Threading;
    using Moonpig.PostOffice.Api.Repositories.Contracts;
    using Moonpig.PostOffice.Api.Handlers;

    public class DespatchDateHandlerUnitTests
    {
        private readonly Mock<IProductRepository> mockProductRepository;
        private readonly Mock<ISupplierRepository> mockSupplierRepository;
        private readonly DespatchDateHandler handler;
        private Order order;
        private DespatchDate expectedDespatchDate;
        private int supplierId;
        private int productId;

        public DespatchDateHandlerUnitTests()
        {
            expectedDespatchDate = new DespatchDate()
            {
                Date = new DateTime(2020, 09, 10)
            };
            mockProductRepository = new Mock<IProductRepository>();
            mockSupplierRepository = new Mock<ISupplierRepository>();
            handler = new DespatchDateHandler(mockProductRepository.Object, mockSupplierRepository.Object);
            
            supplierId = 1;
            productId = 1;
            mockProductRepository.Setup(x => x.GetSupplierId(productId)).Returns(supplierId);
            mockSupplierRepository.Setup(x => x.GetLeadTime(supplierId)).Returns(3);
        }

        [Fact]
        public void Test_Handle_CallsGetSupplierIdOnce_WhenCalledWithOneProductId()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            
            //Act
            handler.Handle(order, default(CancellationToken));

            //Assert
            mockProductRepository.Verify(x => x.GetSupplierId(productId), Times.Once);
        }

        [Fact]
        public void Test_Handle_CallsGetLeadTimeOnce_WhenCalledWithOneProductId()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            
            //Act
            handler.Handle(order, default(CancellationToken));

            //Assert
            mockSupplierRepository.Verify(x => x.GetLeadTime(supplierId), Times.Once);
        }

        [Theory]
        [InlineData("2020-01-01", "2020-01-06")]
        [InlineData("2020-01-02", "2020-01-06")]
        [InlineData("2020-01-03", "2020-01-06")]
        [InlineData("2020-01-04", "2020-01-07")]
        [InlineData("2020-01-05", "2020-01-08")]
        [InlineData("2020-01-06", "2020-01-09")]
        [InlineData("2020-01-07", "2020-01-10")]
        public void Test_Handle_ReturnsTheCorrectDespatchDate_ForEachDayOfTheWeekTheLeadTimeCanBe(DateTime orderDate, DateTime despatchDate)
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = orderDate
            };
            expectedDespatchDate = new DespatchDate()
            {
                Date = despatchDate
            };

            //Act
            var actualDespatchDate = handler.Handle(order, default(CancellationToken)).Result;

            //Assert
            actualDespatchDate.Date.ShouldBe(expectedDespatchDate.Date);
        }

        [Fact]
        public void Test_Handle_CallsGetSupplierIdTwice_WhenCalledWithTwoProductIds()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId, productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            
            //Act
            handler.Handle(order, default(CancellationToken));

            //Assert
            mockProductRepository.Verify(x => x.GetSupplierId(productId), Times.Exactly(2));
        }

        [Fact]
        public void Test_Handle_CallsGetLeadTimeTwice_WhenCalledWithTwoProductIds()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId, productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            
            //Act
            handler.Handle(order, default(CancellationToken));

            //Assert
            mockSupplierRepository.Verify(x => x.GetLeadTime(supplierId), Times.Exactly(2));
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        public void Test_Handle_ReturnsTheCorrectDespatchDate_WhenCalledWithTwoProductIds(int productIdOne, int productIdTwo)
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productIdOne, productIdTwo},
                OrderDate = new DateTime(2020, 09, 09)
            };
            expectedDespatchDate = new DespatchDate()
            {
                Date = new DateTime(2020, 09, 14)
            };
            mockProductRepository.Setup(x => x.GetSupplierId(2)).Returns(2);
            mockSupplierRepository.Setup(x => x.GetLeadTime(2)).Returns(1);

            //Act
            var actualDespatchDate = handler.Handle(order, default(CancellationToken)).Result;

            //Assert
            actualDespatchDate.Date.ShouldBe(expectedDespatchDate.Date);
        }
    }
}
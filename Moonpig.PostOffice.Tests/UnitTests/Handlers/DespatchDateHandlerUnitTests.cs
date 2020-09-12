namespace Moonpig.PostOffice.Tests.UnitTests.Handlers
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
    using Moonpig.PostOffice.Api.Services.Contracts;

    public class DespatchDateHandlerUnitTests
    {
        private readonly Mock<IProductRepository> mockProductRepository;
        private readonly Mock<ISupplierRepository> mockSupplierRepository;
        private readonly Mock<IDespatchDateService> mockService;
        private readonly DespatchDateHandler handler;
        private Order order;
        private DespatchDate expectedDespatchDate;
        private int supplierId;
        private int productId;
        private int supplierLeadTime;

        public DespatchDateHandlerUnitTests()
        {
            expectedDespatchDate = new DespatchDate()
            {
                Date = new DateTime(2020, 09, 10)
            };
            mockProductRepository = new Mock<IProductRepository>();
            mockSupplierRepository = new Mock<ISupplierRepository>();
            mockService = new Mock<IDespatchDateService>();
            handler = new DespatchDateHandler(mockProductRepository.Object, mockSupplierRepository.Object, mockService.Object);
            
            supplierId = 1;
            productId = 1;
            supplierLeadTime = 3;
            mockProductRepository.Setup(x => x.GetSupplierId(productId)).Returns(supplierId);
            mockSupplierRepository.Setup(x => x.GetLeadTime(supplierId)).Returns(supplierLeadTime);
        }

        [Fact]
        public void Test_Handle_CallsGetSupplierIdOnce_WhenCalledWithOneProductId()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(true);
            
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
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(true);
            
            //Act
            handler.Handle(order, default(CancellationToken));

            //Assert
            mockSupplierRepository.Verify(x => x.GetLeadTime(supplierId), Times.Once);
        }

        [Fact]
        public void Test_Handle_CallsGetSupplierIdTwice_WhenCalledWithTwoProductIds()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId, productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(true);
            
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
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(true);
            
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
                OrderDate = new DateTime(2020, 09, 07)
            };
            expectedDespatchDate = new DespatchDate()
            {
                Date = order.OrderDate.AddDays(supplierLeadTime)
            };
            mockProductRepository.Setup(x => x.GetSupplierId(2)).Returns(2);
            mockSupplierRepository.Setup(x => x.GetLeadTime(2)).Returns(1);
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(true);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(order.OrderDate)).Returns(order.OrderDate);
            mockService.Setup(x => x.GetDateWithWeekendsNotIncludedAsProcessingDays(order.OrderDate, expectedDespatchDate.Date)).Returns(expectedDespatchDate.Date);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(expectedDespatchDate.Date)).Returns(expectedDespatchDate.Date);

            //Act
            var actualDespatchDate = handler.Handle(order, default(CancellationToken)).Result;

            //Assert
            actualDespatchDate.Date.ShouldBe(expectedDespatchDate.Date);
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 1)]
        public void Test_Handle_ReturnsTheCorrectDespatchDate_WhenCalledWithOneProductId(int productIdParam, int supplierLeadTimeParam)
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productIdParam},
                OrderDate = new DateTime(2020, 09, 07)
            };
            expectedDespatchDate = new DespatchDate()
            {
                Date = order.OrderDate.AddDays(supplierLeadTimeParam)
            };
            mockProductRepository.Setup(x => x.GetSupplierId(2)).Returns(2);
            mockSupplierRepository.Setup(x => x.GetLeadTime(2)).Returns(1);
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(true);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(order.OrderDate)).Returns(order.OrderDate);
            mockService.Setup(x => x.GetDateWithWeekendsNotIncludedAsProcessingDays(order.OrderDate, expectedDespatchDate.Date)).Returns(expectedDespatchDate.Date);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(expectedDespatchDate.Date)).Returns(expectedDespatchDate.Date);

            //Act
            var actualDespatchDate = handler.Handle(order, default(CancellationToken)).Result;

            //Assert
            actualDespatchDate.Date.ShouldBe(expectedDespatchDate.Date);
        }

        [Fact]
        public void Test_Handle_CallsDoProductsExistOnce_WhenCalled()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(false);
            
            //Act
            handler.Handle(order, default(CancellationToken));

            //Assert
            mockProductRepository.Verify(x => x.DoProductsExist(order.ProductIds), Times.Once);
        }

        [Fact]
        public void Test_Handle_DoesNotCallGetSupplierId_WhenDoProductsExistsReturnsFalse()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(false);
            
            //Act
            handler.Handle(order, default(CancellationToken));

            //Assert
            mockProductRepository.Verify(x => x.GetSupplierId(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Test_Handle_DoesNotCallGetLeadTime_WhenDoProductsExistsReturnsFalse()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(false);
            
            //Act
            handler.Handle(order, default(CancellationToken));

            //Assert
            mockSupplierRepository.Verify(x => x.GetLeadTime(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Test_Handle_DoesNotCallGetSoonestNonWeekendDate_WhenDoProductsExistsReturnsFalse()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(false);
            
            //Act
            handler.Handle(order, default(CancellationToken));

            //Assert
            mockService.Verify(x => x.GetSoonestNonWeekendDate(It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public void Test_Handle_ReturnsNull_WhenCalledWithNonExistentProductIds()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(false);
            
            //Act
            var actualDespatchDate = handler.Handle(order, default(CancellationToken)).Result;

            //Assert
            actualDespatchDate.ShouldBeNull();
        }

        [Fact]
        public void Test_Handle_CallsGetSoonestNonWeekendDateTwice_WhenDoProductsExistsReturnsTrue()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            expectedDespatchDate.Date = order.OrderDate.AddDays(supplierLeadTime);
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(true);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(order.OrderDate)).Returns(order.OrderDate);
            mockService.Setup(x => x.GetDateWithWeekendsNotIncludedAsProcessingDays(order.OrderDate, expectedDespatchDate.Date)).Returns(expectedDespatchDate.Date);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(expectedDespatchDate.Date)).Returns(expectedDespatchDate.Date);
            
            //Act
            handler.Handle(order, default(CancellationToken));

            //Assert
            mockService.Verify(x => x.GetSoonestNonWeekendDate(order.OrderDate), Times.Once);
            mockService.Verify(x => x.GetSoonestNonWeekendDate(expectedDespatchDate.Date), Times.Once);
        }

        [Fact]
        public void Test_Handle_CallsGetDateWithWeekendsNotIncludedAsProcessingDaysOnce_WhenDoProductsExistsReturnsTrue()
        {
            //Arrange
            var productIdTwo = 2;
            order = new Order(){
                ProductIds = new List<int>() {productId, productIdTwo},
                OrderDate = new DateTime(2020, 09, 09)
            };
            expectedDespatchDate.Date = order.OrderDate.AddDays(supplierLeadTime);
            mockProductRepository.Setup(x => x.GetSupplierId(productIdTwo)).Returns(2);
            mockSupplierRepository.Setup(x => x.GetLeadTime(2)).Returns(1);
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(true);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(order.OrderDate)).Returns(order.OrderDate);
            mockService.Setup(x => x.GetDateWithWeekendsNotIncludedAsProcessingDays(order.OrderDate, expectedDespatchDate.Date)).Returns(expectedDespatchDate.Date);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(expectedDespatchDate.Date)).Returns(expectedDespatchDate.Date);
            
            //Act
            handler.Handle(order, default(CancellationToken));

            //Assert
            mockService.Verify(x => x.GetDateWithWeekendsNotIncludedAsProcessingDays(order.OrderDate, expectedDespatchDate.Date), Times.Once);
        }

        [Fact]
        public void Test_Handle_ReturnsCorrectDate_WhenGetSoonestNonWeekendDateWithOrderDateReturnsDifferentDate()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            var alteredDate = order.OrderDate.AddDays(1);
            expectedDespatchDate.Date = alteredDate.AddDays(supplierLeadTime);
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(true);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(order.OrderDate)).Returns(alteredDate);
            mockService.Setup(x => x.GetDateWithWeekendsNotIncludedAsProcessingDays(alteredDate, expectedDespatchDate.Date)).Returns(expectedDespatchDate.Date);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(expectedDespatchDate.Date)).Returns(expectedDespatchDate.Date);
            
            //Act
            var despatchDate = handler.Handle(order, default(CancellationToken)).Result;

            //Assert
            despatchDate.Date.ShouldBe<DateTime>(expectedDespatchDate.Date);
        }

        [Fact]
        public void Test_Handle_ReturnsCorrectDate_WhenGetDateWithWeekendsNotIncludedAsProcessingDaysWithOrderDateReturnsDifferentDate()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            var alteredDate = order.OrderDate.AddDays(1);
            expectedDespatchDate.Date = alteredDate.AddDays(supplierLeadTime);
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(true);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(order.OrderDate)).Returns(order.OrderDate);
            mockService.Setup(x => x.GetDateWithWeekendsNotIncludedAsProcessingDays(order.OrderDate, order.OrderDate.AddDays(supplierLeadTime))).Returns(expectedDespatchDate.Date);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(expectedDespatchDate.Date)).Returns(expectedDespatchDate.Date);
            
            //Act
            var despatchDate = handler.Handle(order, default(CancellationToken)).Result;

            //Assert
            despatchDate.Date.ShouldBe<DateTime>(expectedDespatchDate.Date);
        }

        [Fact]
        public void Test_Handle_ReturnsCorrectDate_WhenGetSoonestNonWeekendDateWithOrderDatePlusLeadTimeReturnsDifferentDate()
        {
            //Arrange
            order = new Order(){
                ProductIds = new List<int>() {productId},
                OrderDate = new DateTime(2020, 09, 09)
            };
            var alteredDate = order.OrderDate.AddDays(1);
            expectedDespatchDate.Date = alteredDate.AddDays(supplierLeadTime);
            var orderDatePlusLeadTime = order.OrderDate.AddDays(supplierLeadTime);
            mockProductRepository.Setup(x => x.DoProductsExist(order.ProductIds)).Returns(true);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(order.OrderDate)).Returns(order.OrderDate);
            mockService.Setup(x => x.GetDateWithWeekendsNotIncludedAsProcessingDays(order.OrderDate, orderDatePlusLeadTime)).Returns(orderDatePlusLeadTime);
            mockService.Setup(x => x.GetSoonestNonWeekendDate(orderDatePlusLeadTime)).Returns(expectedDespatchDate.Date);
            
            //Act
            var despatchDate = handler.Handle(order, default(CancellationToken)).Result;

            //Assert
            despatchDate.Date.ShouldBe<DateTime>(expectedDespatchDate.Date);
        }
    }
}
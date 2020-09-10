namespace Moonpig.PostOffice.Tests.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using Api.Controllers;
    using Shouldly;
    using Xunit;
    using Moq;
    using MediatR;
    using Moonpig.PostOffice.Api.Models;
    using System.Threading.Tasks;
    using System.Threading;
    using Microsoft.AspNetCore.Mvc;

    public class DespatchDateControllerUnitTests
    {
        private readonly Mock<IMediator> mockMediator;
        private readonly DespatchDateController controller;
        private List<int> productIds;
        private DateTime orderDate;
        private DespatchDate expectedDespatchDate;

        public DespatchDateControllerUnitTests()
        {
            productIds = new List<int>() {1};
            orderDate = new DateTime(2020, 09, 09);
            expectedDespatchDate = new DespatchDate()
            {
                Date = new DateTime(2020, 09, 10)
            };
            mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.Is<Order>(x => x.ProductIds == productIds && x.OrderDate == orderDate), default(CancellationToken))).Returns(Task.FromResult(expectedDespatchDate));
            controller = new DespatchDateController(mockMediator.Object);
        }

        [Fact]
        public void Test_GetDespatchDate_CallsSend_WhenCalled()
        {
            //Arrange

            //Act
            controller.GetDespatchDate(productIds, orderDate);

            //Assert
            mockMediator.Verify(x => x.Send(It.Is<Order>(x => x.ProductIds == productIds && x.OrderDate == orderDate), default(CancellationToken)), Times.Once);
        }

        [Fact]
        public void Test_GetDespatchDate_ReturnsResponseFromSend_WhenCalled()
        {
            //Arrange

            //Act
            var actualDespatchDateActionResult = controller.GetDespatchDate(productIds, orderDate);

            //Assert
            var actualDespatchDate = actualDespatchDateActionResult.Value as DespatchDate;
            actualDespatchDate.ShouldBe<DespatchDate>(expectedDespatchDate);
        }

        [Fact]
        public void Test_GetDespatchDate_ReturnsNotFoundResult_WhenCalledWithNonExistentProductId()
        {
            //Arrange
            var nonExistentProductIds = new List<int>() {18};
            mockMediator.Setup(x => x.Send(It.Is<Order>(x => x.ProductIds == nonExistentProductIds && x.OrderDate == orderDate), default(CancellationToken))).Returns(Task.FromResult((DespatchDate)null));

            //Act
            var actualDespatchDateActionResult = controller.GetDespatchDate(nonExistentProductIds, orderDate);

            //Assert
            actualDespatchDateActionResult.Result.ShouldBeOfType<NotFoundResult>();
        }
    }
}
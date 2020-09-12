namespace Moonpig.PostOffice.Tests.UnitTests.Services
{
    using Moonpig.PostOffice.Api.Services.Concretes;
    using Shouldly;
    using System;
    using Xunit;
    
    public class DespatchDateServiceUnitTests
    {
        DespatchDateService service;
        public DespatchDateServiceUnitTests()
        {
            service = new DespatchDateService();
        }

        // ------------------------------------
        //  GetSoonestNonWeekendDate
        // ------------------------------------

        [Theory]
        [InlineData("2020-01-01", "2020-01-01")]
        [InlineData("2020-01-02", "2020-01-02")]
        [InlineData("2020-01-03", "2020-01-03")]
        [InlineData("2020-01-04", "2020-01-06")]
        [InlineData("2020-01-05", "2020-01-06")]
        [InlineData("2020-01-06", "2020-01-06")]
        [InlineData("2020-01-07", "2020-01-07")]
        public void Test_GetSoonestNonWeekendDate_ReturnsTheCorrectDate_ForEachDayOfTheWeekTheLeadTimeCanBe(DateTime orderDate, DateTime expectedDespatchDate)
        {
            //Arrange

            //Act
            var actualDespatchDate = service.GetSoonestNonWeekendDate(orderDate);

            //Assert
            actualDespatchDate.Date.ShouldBe(expectedDespatchDate);
        }

        // ------------------------------------------------------
        //  GetDateWithWeekendsNotIncludedAsProcessingDays
        // ------------------------------------------------------

        [Theory]
        [InlineData("2020-01-01", "2020-01-02", "2020-01-02")]
        [InlineData("2020-01-01", "2020-01-06", "2020-01-08")]
        [InlineData("2020-01-01", "2020-01-13", "2020-01-17")]
        [InlineData("2020-01-03", "2020-01-17", "2020-01-23")]
        public void Test_GetDateWithWeekendsNotIncludedAsProcessingDays_ReturnsCorrectDate_WhenCalled(DateTime orderDate, DateTime orderDateWithLeadTime, DateTime expectedMaxLeadTime)
        {
            //Arrange

            //Act
            var actualMaxLeadTime = service.GetDateWithWeekendsNotIncludedAsProcessingDays(orderDate, orderDateWithLeadTime);

            //Assert
            actualMaxLeadTime.ShouldBe<DateTime>(expectedMaxLeadTime);
        }
    }
}
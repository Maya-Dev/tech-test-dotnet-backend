namespace Moonpig.PostOffice.Api.Services.Contracts
{
    using System;
    public interface IDespatchDateService
    {
        DateTime GetSoonestNonWeekendDate(DateTime maxLeadTime);
        DateTime GetDateWithWeekendsNotIncludedAsProcessingDays(DateTime baselineDate, DateTime maxLeadTime);
    }
}
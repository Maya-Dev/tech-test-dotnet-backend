namespace Moonpig.PostOffice.Api.Services.Concretes
{
    using System;
    using System.Globalization;
    using System.Threading;
    using Moonpig.PostOffice.Api.Services.Contracts;

    public class DespatchDateService : IDespatchDateService
    {
        public DateTime GetSoonestNonWeekendDate(DateTime maxLeadTime)
        {
            if (maxLeadTime.DayOfWeek == DayOfWeek.Saturday)
            {
                maxLeadTime = maxLeadTime.AddDays(2);
            }
            else if (maxLeadTime.DayOfWeek == DayOfWeek.Sunday)
            {
                maxLeadTime = maxLeadTime.AddDays(1);
            }
            return maxLeadTime;
        }

        public DateTime GetDateWithWeekendsNotIncludedAsProcessingDays(DateTime baselineDate, DateTime maxLeadTime)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            CalendarWeekRule calendarWeekRule = culture.DateTimeFormat.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
            Calendar calendar = culture.Calendar;

            var baselineWeekNumber = calendar.GetWeekOfYear(baselineDate, calendarWeekRule, firstDayOfWeek);
            var maxLeadTimeWeekNumber = calendar.GetWeekOfYear(maxLeadTime, calendarWeekRule, firstDayOfWeek);

            while(maxLeadTimeWeekNumber != baselineWeekNumber)
            {
                var weekNumberDifference = maxLeadTimeWeekNumber - baselineWeekNumber;
                baselineDate = maxLeadTime;
                maxLeadTime = maxLeadTime.AddDays(2 * weekNumberDifference);
                
                baselineWeekNumber = calendar.GetWeekOfYear(baselineDate, calendarWeekRule, firstDayOfWeek);
                maxLeadTimeWeekNumber = calendar.GetWeekOfYear(maxLeadTime, calendarWeekRule, firstDayOfWeek);
            }
            
            return maxLeadTime;
        }
    }
}
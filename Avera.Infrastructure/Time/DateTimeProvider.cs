using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avera.Infrastructure.Time
{
    internal sealed class DateTimeProvider : IDateTimeProvider
    {
        private static readonly TimeZoneInfo PhTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");

        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime PhilippineNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PhTimeZone);
    }
}

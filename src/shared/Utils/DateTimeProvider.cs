using System;

namespace Shared.Utils
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}

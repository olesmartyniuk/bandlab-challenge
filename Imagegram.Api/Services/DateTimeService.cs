using System;

namespace Imagegram.Api.Services
{
    public class DateTimeService
    {
        public virtual DateTime Now()
        {
            return DateTime.UtcNow;
        }
    }

}

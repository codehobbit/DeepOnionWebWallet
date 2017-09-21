using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WWWOnionWallet.Helper
{
    public static class DateTimeHelper
    {
        public static DateTime ConvertTimestamp(double timestamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
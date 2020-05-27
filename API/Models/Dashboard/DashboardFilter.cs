using System;
using System.Text.Json.Serialization;

namespace API.Models.Dashboard
{
    public class DashboardFilter
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string GroupBy { get; set; }
    }
}

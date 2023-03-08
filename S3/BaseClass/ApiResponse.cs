using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3.BaseClass
{
    public class ApiResponse<T>
    {
        public string message { get; set; }
        public string responseCode { get; set; } = "00000";
        public DateTime timestamp { get; set; } = DateTime.UtcNow.AddHours(3);
        public T container { get; set; }
    }
}

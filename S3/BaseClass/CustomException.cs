using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace S3.BaseClass
{
    public class CustomException : Exception
    {
        public string message { get; set; }
        private HttpStatusCode httpStatusCode { get; set; }
        public CustomException(HttpStatusCode httpStatusCode, string _message = "") : base(JsonConvert.SerializeObject(new
        {
            httpStatusCode,
            _message
        }))
        {
            message = _message;
        }
    }
}

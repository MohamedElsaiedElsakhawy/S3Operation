using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3.BaseClass
{
    public class S3Model
    {
        public string accessKey { get; set; }
        public string secertKey { get; set; }
        public string endpoint { get; set; }
        public string bucket { get; set; }
    }
}

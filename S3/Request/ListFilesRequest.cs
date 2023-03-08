using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3.Request
{
    public class ListFilesRequest
    {
        public string fullPath { get; set; }
        public string bucketName { get; set; }
    }
}

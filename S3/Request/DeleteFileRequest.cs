using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3.Request
{
    public class DeleteFileRequest
    {
        public string filePath { get; set; }
        public string bucketName { get; set; }
    }
}

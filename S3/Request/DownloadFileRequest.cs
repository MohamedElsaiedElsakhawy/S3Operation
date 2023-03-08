using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3.Request
{
    public class DownloadFileRequest
    {
        public string bucketName { get; set; }
        public string filePath { get; set; }
        public string fileName { get; set; }
    }
}

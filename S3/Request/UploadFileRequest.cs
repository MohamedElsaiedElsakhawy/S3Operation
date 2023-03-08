using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace S3.Request
{
    public class UploadFileRequest
    {
        public List<IFormFile> files { get; set; }
        public string filePath { get; set; }
        public string bucketName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3.BaseClass
{
    public class FileBase
    {
        public string folderName { get; set; }
        public List<string> SuccessFilesName { get; set; }
        public List<string> FailedFilesName { get; set; }
    }
}

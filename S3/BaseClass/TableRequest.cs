using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace S3.BaseClass
{
    public class TableRequest
    {
        /// <summary>
        /// Description
        /// </summary>
        public int? page { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public int? pageSize { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [JsonConstructor]
        public TableRequest()
        {

        }
        /// <summary>
        /// Description
        /// </summary>
        [JsonConstructor]
        public TableRequest(int? page, int pageSize)
        {
            this.page = page;
            this.pageSize = pageSize;
        }
    }
}

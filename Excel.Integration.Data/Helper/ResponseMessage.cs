using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel.Integration.Data.Helper
{
    public class ResponseMessage
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public object Data { get; set; }
    }

}

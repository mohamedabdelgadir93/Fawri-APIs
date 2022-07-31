using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fawry.APIStantderd
{
    public class ResponseClass
    {
        public int Code { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Model.ViewModel
{
    public class Result
    {
        public Header Header { get; set; }
        public object Data { get; set; }

        public Result()
        {
            this.Header = new Header();
            this.Data = new object();
        }
    }

    public class Header
    {
        public string ResultCode { get; set; }
        public string ResultDescription { get; set; }

        public Header()
        {
            this.ResultCode = string.Empty;
            this.ResultDescription = string.Empty;
        }
    }
}

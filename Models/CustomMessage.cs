using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace producer_service.Models
{
    class CustomMessage
    {
        public CustomTime time { get; set; }
        public string disclaimer { get; set; }
        public string chartName { get; set; }
        public CustomBpi bpi { get; set; }
    }
}

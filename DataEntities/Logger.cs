using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kestavar.DataEntities
{
    public class Logger
    {
        public string Id { get; set; }
        public DateTime date { get; set; }
        public string MerchantFk { get; set; }
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public string InstanceId { get; set; }

    }
}

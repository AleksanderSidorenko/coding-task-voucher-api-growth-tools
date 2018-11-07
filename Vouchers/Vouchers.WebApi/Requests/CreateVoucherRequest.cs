using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vouchers.WebApi.Requests
{
    public class Money
    {
        public int Amount { get; set; }
        public string Currency { get; set; }
    }

    public class CreateVoucherRequest
    {
        public Money Money { get; set; } 
    }
}

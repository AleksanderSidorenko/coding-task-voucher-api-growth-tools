using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vouchers.WebApi.Requests
{
    public class PaymentRequest
    {
        public string VoucherCode { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
    }
}

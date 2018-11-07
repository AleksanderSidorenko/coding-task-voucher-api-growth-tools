using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace Vouchers.IntegrationTests
{
    public class PaymentWithInvalidVoucherFixture
    {
        public HttpResponseMessage Response { get; }
        public string VoucherCode { get; }

        public PaymentWithInvalidVoucherFixture(TestHarness harness)
        {
            Response = harness.JohnDoe.CreateVoucher("{'money': {'amount': 10,'currency': 'GBP'} }").Result;
            var resultJson = Response.Content.ReadAsStringAsync().Result;
            VoucherCode = (string)JsonConvert.DeserializeObject<dynamic>(resultJson).voucherCode;
            Response = harness.JohnDoe.PayWithVoucher("{'VoucherCode': '" + VoucherCode + "', 'Amount': 10, 'Currency': 'RUR'}").Result;
        }
    }

    [Collection("Test Harness")]
    public class PaymentWithInvalidVoucher : OsdrWebTest, IClassFixture<PaymentWithInvalidVoucherFixture>
    {
        public HttpResponseMessage Response { get; }

        public PaymentWithInvalidVoucher(TestHarness fixture, ITestOutputHelper output, PaymentWithInvalidVoucherFixture initFixture)
            : base(fixture, output)
        {
            Response = initFixture.Response;

        }

        [Fact]
        public void InvalidPaymentWithVoucher_ShouldBeEndedWithError()
        {
            Response.IsSuccessStatusCode.Should().BeFalse();
        }
    }
}

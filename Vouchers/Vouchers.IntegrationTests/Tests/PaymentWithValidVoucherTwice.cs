using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Vouchers.IntegrationTests
{
    public class PaymentWithValidVoucherTwiceFixture
    {
        public HttpResponseMessage Response { get; }
        public string VoucherCode { get; }

        public PaymentWithValidVoucherTwiceFixture(TestHarness harness)
        {
            Response = harness.JohnDoe.CreateVoucher("{'money': {'amount': 10,'currency': 'GBP'} }").Result;
            var resultJson = Response.Content.ReadAsStringAsync().Result;
            VoucherCode = (string)JsonConvert.DeserializeObject<dynamic>(resultJson).voucherCode;
            harness.JohnDoe.PayWithVoucher("{'VoucherCode': '" + VoucherCode + "', 'Amount': 10, 'Currency': 'GBP'}").Wait();
            Response = harness.JohnDoe.PayWithVoucher("{'VoucherCode': '" + VoucherCode + "', 'Amount': 10, 'Currency': 'GBP'}").Result;
        }
    }

    [Collection("Test Harness")]
    public class PaymentWithValidVoucherTwice : OsdrWebTest, IClassFixture<PaymentWithValidVoucherTwiceFixture>
    {
        public HttpResponseMessage Response { get; }
        public string VoucherCode { get; }
        public IMongoCollection<BsonDocument> Vouchers { get; }

        public PaymentWithValidVoucherTwice(TestHarness fixture, ITestOutputHelper output, PaymentWithValidVoucherTwiceFixture initFixture)
            : base(fixture, output)
        {
            Response = initFixture.Response;
            VoucherCode = initFixture.VoucherCode;
            Vouchers = fixture.MongoDb.GetCollection<BsonDocument>("Vouchers");
        }

        [Fact]
        public void PaymentWithVoucherTwice_ShouldBeEndedWithError()
        {
            Response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task PaymentWithVoucherTwice_ShouldContainAppropriateData()
        {
            var filter = new BsonDocument("VoucherCode", VoucherCode);
            var result = await Vouchers.Find(filter).SingleOrDefaultAsync();
            result.GetValue("IsUsed").ToJson().Trim().Should().BeEquivalentTo("true");
        }
    }
}

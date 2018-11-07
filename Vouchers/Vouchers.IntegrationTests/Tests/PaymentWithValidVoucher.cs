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
    public class PaymentWithValidVoucherFixture
    {
        public HttpResponseMessage Response { get; }
        public string VoucherCode { get; }

        public PaymentWithValidVoucherFixture(TestHarness harness)
        {
            Response = harness.JohnDoe.CreateVoucher("{'money': {'amount': 10,'currency': 'GBP'} }").Result;
            var resultJson = Response.Content.ReadAsStringAsync().Result;
            VoucherCode = (string)JsonConvert.DeserializeObject<dynamic>(resultJson).voucherCode;
            Response = harness.JohnDoe.PayWithVoucher("{'VoucherCode': '" + VoucherCode + "', 'Amount': 10, 'Currency': 'GBP'}").Result;
        }
    }

    [Collection("Test Harness")]
    public class PaymentWithValidVoucher : OsdrWebTest, IClassFixture<PaymentWithValidVoucherFixture>
    {
        public HttpResponseMessage Response { get; }
        public string VoucherCode { get; }
        public IMongoCollection<BsonDocument> Vouchers { get; }

        public PaymentWithValidVoucher(TestHarness fixture, ITestOutputHelper output, PaymentWithValidVoucherFixture initFixture)
            : base(fixture, output)
        {
            Response = initFixture.Response;
            VoucherCode = initFixture.VoucherCode;
            Vouchers = fixture.MongoDb.GetCollection<BsonDocument>("Vouchers");
        }

        [Fact]
        public async Task PaymentWithVoucher_VoucherShouldExist()
        {
            Response.IsSuccessStatusCode.Should().BeTrue();
            var filter = new BsonDocument("VoucherCode", VoucherCode);
            var result = await Vouchers.Find(filter).SingleOrDefaultAsync();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task PaymentWithVoucher_ShouldMarkVoucherAsUsed()
        {
            var filter = new BsonDocument("VoucherCode", VoucherCode);
            var result = await Vouchers.Find(filter).SingleOrDefaultAsync();
            result.GetValue("IsUsed").ToJson().Trim().Should().BeEquivalentTo("true");
        }
    }
}

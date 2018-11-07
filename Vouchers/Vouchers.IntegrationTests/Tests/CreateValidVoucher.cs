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
    public class CreateValidVoucherFixture
    {
        public HttpResponseMessage Response { get; }

        public CreateValidVoucherFixture(TestHarness harness)
        {
            Response = harness.JohnDoe.CreateVoucher("{'money': {'amount': 10,'currency': 'GBP'} }").Result;
        }
    }

    [Collection("Test Harness")]
    public class CreateValidVoucher : OsdrWebTest, IClassFixture<CreateValidVoucherFixture>
    {
        public HttpResponseMessage Response { get; }
        public IMongoCollection<BsonDocument> Vouchers { get; }

        public CreateValidVoucher(TestHarness fixture, ITestOutputHelper output, CreateValidVoucherFixture initFixture)
            : base(fixture, output)
        {
            Response = initFixture.Response;
            Vouchers = fixture.MongoDb.GetCollection<BsonDocument>("Vouchers");
        }

        [Fact]
        public async Task CreateVoucher_ShouldCreateNewVoucherDocumentInDatabase()
        {
            var resultJson = await Response.Content.ReadAsStringAsync();
            Response.IsSuccessStatusCode.Should().BeTrue();
            var code = (string)JsonConvert.DeserializeObject<dynamic>(resultJson).voucherCode;
            var filter = new BsonDocument("VoucherCode", code);
            var result = await Vouchers.Find(filter).SingleOrDefaultAsync();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateVoucher_ShouldContainAppropriateData()
        {
            var resultJson = await Response.Content.ReadAsStringAsync();
            var code = (string)JsonConvert.DeserializeObject<dynamic>(resultJson).voucherCode;
            var filter = new BsonDocument("VoucherCode", code);
            var result = await Vouchers.Find(filter).SingleOrDefaultAsync();
            result.GetValue("Money").ToJson().Trim().Should().BeEquivalentTo("{ \"Amount\" : 10, \"Currency\" : \"GBP\" }");
        }
    }
}

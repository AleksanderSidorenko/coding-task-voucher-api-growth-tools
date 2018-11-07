using FluentAssertions;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Vouchers.IntegrationTests
{
    public class CreateInvalidVoucherFixture
    {
        public HttpResponseMessage Response { get; }

        public CreateInvalidVoucherFixture(TestHarness harness)
        {
            Response = harness.JohnDoe.CreateVoucher("{'money': {'amount': -5,'currency': 'GBP'} }").Result;
        }
    }

    [Collection("Test Harness")]
    public class CreateInvalidVoucher : OsdrWebTest, IClassFixture<CreateInvalidVoucherFixture>
    {
        public HttpResponseMessage Response { get; }

        public CreateInvalidVoucher(TestHarness fixture, ITestOutputHelper output, CreateInvalidVoucherFixture initFixture)
            : base(fixture, output)
        {
            Response = initFixture.Response;
        }

        [Fact]
        public async Task CreateVoucher_ShouldNotCreateNewVoucherDocumentInDatabase()
        {
            var resultJson = await Response.Content.ReadAsStringAsync();
            Response.IsSuccessStatusCode.Should().BeFalse();
        }

    }
}

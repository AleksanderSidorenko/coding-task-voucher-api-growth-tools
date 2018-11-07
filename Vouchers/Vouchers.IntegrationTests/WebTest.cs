using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Vouchers.IntegrationTests
{
    [CollectionDefinition("Test Harness")]
    public class OsdrTestCollection : ICollectionFixture<TestHarness>
    {
    }

    public class OsdrWebTest : BaseTest
    {
        public OsdrWebTest(TestHarness fixture, ITestOutputHelper output = null) : base(fixture)
        {
            if (output != null)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .CreateLogger()
                    .ForContext<BaseTest>();
            }
        }

        protected TestHarness WebFixture => Harness as TestHarness;
    }
}

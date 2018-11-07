using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Vouchers.IntegrationTests
{
    [CollectionDefinition("Test Harness")]
    public class TestCollection : ICollectionFixture<TestHarness>
    {
    }

    public abstract class BaseTest
    {
        public TestHarness Harness { get; }


        protected IMongoCollection<dynamic> Vouchers => Harness.MongoDb.GetCollection<dynamic>("Vouchers");

        public BaseTest(TestHarness fixture, ITestOutputHelper output = null)
        {
            Harness = fixture;

            if (output != null)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .CreateLogger()
                    .ForContext<BaseTest>();
            }
        }
    }
}

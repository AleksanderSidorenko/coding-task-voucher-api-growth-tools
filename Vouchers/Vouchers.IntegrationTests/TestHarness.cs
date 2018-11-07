using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Net.Http;

namespace Vouchers.IntegrationTests
{
    public class TestHarness : IDisposable
    {
        protected IServiceProvider _serviceProvider;
        public WebApiClient JohnDoe { get; }
        public IMongoDatabase MongoDb { get { return _serviceProvider.GetService<IMongoDatabase>(); } }
        public TestHarness()
        {
            var services = new ServiceCollection();

            var JohnClient = new HttpClient();

            JohnDoe = new WebApiClient(JohnClient);
            var mongoUrl = new MongoUrl("mongodb://mongo:27017/");
            services.AddSingleton(new MongoClient(mongoUrl));
            services.AddSingleton(service => service.GetService<MongoClient>().GetDatabase("ciklum"));

            _serviceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            JohnDoe.Dispose();
        }
    }
}

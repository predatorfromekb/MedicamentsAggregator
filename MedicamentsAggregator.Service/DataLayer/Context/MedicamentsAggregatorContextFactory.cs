using MedicamentsAggregator.Service.Models.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace MedicamentsAggregator.Service.DataLayer.Context
{
    public class MedicamentsAggregatorContextFactory
    {
        private readonly IConfiguration _configuration;

        public MedicamentsAggregatorContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MedicamentsAggregatorContext CreateContext()
        {
            return new MedicamentsAggregatorContext(
                new DbContextOptionsBuilder<MedicamentsAggregatorContext>()
                    .UseNpgsql(_configuration.GetConnectionString("MedicamentsAggregator"))
                    .UseLoggerFactory(EntityFrameworkLoggerFactory.Singleton)
                .Options);
        }
    }
}
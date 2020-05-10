using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vostok.Logging.Microsoft;

namespace MedicamentsAggregator.Service.Models.Logs
{
    public static class EntityFrameworkLoggerFactory
    {
        public static readonly ILoggerFactory Singleton = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name
                        && level == LogLevel.Information)
                    .AddConsole();
            })
            .AddVostok(new EntityFrameworkLog());
    }
}
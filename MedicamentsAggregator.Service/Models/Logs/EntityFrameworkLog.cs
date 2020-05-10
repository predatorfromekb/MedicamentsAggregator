using System.IO;
using System.Reflection;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;

namespace MedicamentsAggregator.Service.Models.Logs
{
    public class EntityFrameworkLog : FileLog
    {
        public EntityFrameworkLog() : base(new FileLogSettings
        {
            FilePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "../logs/ef/{RollingSuffix}.txt"),
            RollingStrategy = new RollingStrategyOptions
            {
                Type = RollingStrategyType.Hybrid,
                Period = RollingPeriod.Day,
                MaxSize = 1024 * 1024
            }
        }){}
    } 
}
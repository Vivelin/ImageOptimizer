using System;
using System.IO;
using System.Transactions;
using Decos.Diagnostics;

using Microsoft.Extensions.DependencyInjection;

namespace ImageOptimizer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddTraceSourceLogging(options =>
            {
                options.SetMinimumLogLevel(LogLevel.Debug);
                options.AddConsole();
            });

            var serviceProvider = services.BuildServiceProvider();
            var logFactory = serviceProvider.GetRequiredService<ILogFactory>();
            var log = serviceProvider.GetRequiredService<ILog<PhotoOptimizer>>();

            var optimizer = new PhotoOptimizer(log);
            foreach (var file in args)
            {
                var temp = Path.GetTempFileName();
                File.Delete(temp);

                var (size, quality) = optimizer.Optimize(file, temp);

                var target = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                if (size < int.MaxValue)
                    target += $".{size}px";
                if (quality < 100)
                    target += $".{quality}q";
                if (size == int.MaxValue && quality == 100)
                    target += ".optimized";
                target += ".jpg";

                try
                {
                    File.Copy(temp, target);
                }
                finally { File.Delete(temp); }

                log.Info("File has been optimized.", new { file });
            }

            logFactory.ShutdownAsync().GetAwaiter().GetResult();
        }
    }
}
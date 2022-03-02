using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Business
{
    public class ConfigurationManager
    {

        public ConfigurationManager(IConfiguration configuration, IHostEnvironment env)
        {
            Mode = (ApplicationMode) Enum.Parse(typeof(ApplicationMode), env.EnvironmentName);
        }

        public ApplicationMode Mode { get; }
    }

    public enum ApplicationMode
    {
        Development,
        Profiling,
        Staging,
        Production
    }
}
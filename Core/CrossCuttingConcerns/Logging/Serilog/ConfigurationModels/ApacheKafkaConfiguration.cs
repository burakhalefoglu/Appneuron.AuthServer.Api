namespace Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels
{
    public class ApacheKafkaConfiguration
    {
        public string BootstrapServer { get; set; }
        public string[] Topics { get; set; }
    }
}

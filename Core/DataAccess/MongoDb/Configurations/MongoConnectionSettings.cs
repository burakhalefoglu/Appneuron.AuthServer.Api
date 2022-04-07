namespace Core.DataAccess.MongoDb.Configurations;

public class MongoConnectionSettings
{
    public string Host { get; set; }
    public string Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string DatabaseName { get; set; }
}
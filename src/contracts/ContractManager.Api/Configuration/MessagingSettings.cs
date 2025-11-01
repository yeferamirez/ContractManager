namespace ContractManager.Api.Configuration;

public class MessagingSettings
{
    public string Host { get; set; } = default!;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string ConnectionString => $"amqp://{Username}:{Password}@{Host}/{VirtualHost}";
}

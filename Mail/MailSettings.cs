namespace TestReactOther.Mail;

public class MailSettings
{
    public string Server { get; }
    public int Port { get; }
    public string SenderName { get; }
    public string SenderEmail { get; }
    public string UserName { get; }
    public string Password { get; }

    public MailSettings(IConfiguration configuration)
    {
        Server = configuration["MailSettings:Server"];
        Port = int.Parse(configuration["MailSettings:Port"]);
        SenderName = "Events Montreal";
        SenderEmail = configuration["MailSettings:UserName"];
        UserName = configuration["MailSettings:UserName"];
        Password = configuration["MailSettings:Password"];
    }
}
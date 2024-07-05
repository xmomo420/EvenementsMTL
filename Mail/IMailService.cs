namespace TestReactOther.Mail;

public interface IMailService
{
    bool SendMail(MailData mailData);
}
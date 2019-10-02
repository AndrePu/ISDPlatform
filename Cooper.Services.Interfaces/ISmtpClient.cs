namespace Cooper.Services.Interfaces
{
    public interface ISmtpClient
    {
        string SmtpServerName { get; }

        void SendMail(string to, string subject, string body);
    }
}
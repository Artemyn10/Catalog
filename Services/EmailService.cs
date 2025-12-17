using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Catalog.Data;  // Namespace для ApplicationDbContext (опционально, для логирования)

namespace Catalog.Services
{
    // Интерфейс для DI и тестирования
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string toEmail, string username, string confirmationToken);
    }

    // Реализация сервиса
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendConfirmationEmailAsync(string toEmail, string username, string confirmationToken)
        {
            // Формирование сообщения
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["EmailSettings:SenderName"],
                _configuration["EmailSettings:SenderEmail"]));
            message.To.Add(new MailboxAddress(username, toEmail));
            message.Subject = "Подтверждение регистрации в Каталоге рецептов";

            // Ссылка для подтверждения (замените localhost на production URL)
            var confirmationLink = $"https://localhost:7193/Account/ConfirmEmail?token={confirmationToken}";
            var body = $@"
                <h2>Добро пожаловать, {username}!</h2>
                <p>Спасибо за регистрацию. Чтобы подтвердить email, перейдите по <a href='{confirmationLink}'>этой ссылке</a>.</p>
                <p>Ссылка действительна 24 часа. Если вы не регистрировались, игнорируйте это письмо.</p>
                <hr>
                <small>Это автоматическое письмо от Каталога рецептов. Не отвечайте на него.</small>";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            message.Body = bodyBuilder.ToMessageBody();

            // Отправка через SMTP
            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["EmailSettings:SmtpServer"],
                int.Parse(_configuration["EmailSettings:SmtpPort"]),
                SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(
                _configuration["EmailSettings:Username"],
                _configuration["EmailSettings:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);  // Graceful shutdown
        }
    }
}
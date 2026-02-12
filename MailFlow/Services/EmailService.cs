using MailFlow.Dtos;
using MailFlow.Settings;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MailFlow.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendVerificationCodeAsync(string receiverEmail, string subject, string body)
        {// MimeMessage → Mailin tamamını temsil eder (kimden, kime, konu, içerik vs.)
            MimeMessage mimeMessage = new MimeMessage();

            mimeMessage.From.Add(new MailboxAddress("MailFlow", _emailSettings.Email);
            // Maili gönderen kişi bilgisi (gözüken isim + gerçek mail)

            mimeMessage.To.Add(new MailboxAddress("", receiverEmail));
            // Mailin gideceği kişi (alıcı)

            mimeMessage.Subject = subject;
            // Mail başlığı (kullanıcının inbox’ta gördüğü konu)


            // BodyBuilder → Mail içeriğini hazırlayan yardımcı sınıf (text veya html yazabiliriz)
            var bodyBuilder = new BodyBuilder();

            bodyBuilder.TextBody = body;
            // Mailin düz metin içeriği (kullanıcı maili açınca okuduğu yazı)

            // MimeMessage.Body → MimeEntity tipindedir.
            // BodyBuilder ile hazırladığımız içeriği mail formatına çevirip buraya atıyoruz.
            mimeMessage.Body = bodyBuilder.ToMessageBody();


            // SMTP (Simple Mail Transfer Protocol) → Mail gönderme protokolü
            // Yani maili internet üzerinden gerçekten gönderen kısım
            SmtpClient smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(_emailSettings.Host, _emailSettings.Port, false);
            // Gmail SMTP sunucusuna bağlan (adres + port)

            await smtpClient.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
            // Mail hesabı ile giriş yap (uygulama şifresi)

            await smtpClient.SendAsync(mimeMessage);
            // Hazırladığımız maili gönder

            await smtpClient.DisconnectAsync(true);
            // Bağlantıyı güvenli şekilde kapat

        }
    }
}


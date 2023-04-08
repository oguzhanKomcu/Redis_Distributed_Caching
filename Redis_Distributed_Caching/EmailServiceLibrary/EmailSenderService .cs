using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
namespace Redis_Distributed_Caching.EmailServiceLibrary
{
    public class EmailSenderService : BackgroundService
    {
        private readonly Timer _timer;

        public EmailSenderService()
        {
            DateTime now = DateTime.Now;
            TimeSpan timeToGo = new TimeSpan(0, 15, 0);
            TimeSpan interval = new TimeSpan(1, 0, 0, 0);
            TimeSpan timeOfDay = TimeSpan.Parse("00:15");
            DateTime nextTime = now.Date.Add(timeOfDay);

            if (now > nextTime)
                nextTime = nextTime.Add(interval);

            timeToGo = nextTime - now;
            _timer = new Timer(SendEmail, null, timeToGo, interval);
        }

        private void SendEmail(object state)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("oguz_fener_390@hotmail.com");
            mail.To.Add("oguz_fener_390@hotmail.com");
            mail.Subject = "Message Broker Deneme";
            mail.Body = "Eğitim tarihiniz gelmiştir.Asp.Net Core ";

            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("oguz_fener_390@hotmail.com", "Oguzddd.1Oguzddd.1");
            smtp.EnableSsl = true;

            try
            {
                smtp.Send(mail);
                Console.WriteLine("E-posta gönderildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}


using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using NoteApplication.Data;
using NoteApplication.Hubs;
using NoteApplication.Models;
using System;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
namespace NoteApplication.Services
{
    public class AlarmService : BackgroundService
    {
        private readonly IHubContext<NoteHub> _hubContext;
        public AlarmService(IHubContext<NoteHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var alarm in AlarmStorage.AlarmTimes)
                {
                    /*var email = alarm.Value;
                    Console.WriteLine(email);*/
                    if (alarm.Key <= DateTime.Now)
                    {
                        await _hubContext.Clients.Clients(alarm.Value).SendAsync("alarmTriggered", alarm);
                       // var email = alarm.Value;
                       // Console.WriteLine(email);
                       /* MailMessage message = new MailMessage();
                        message.From = new MailAddress("rakesh.kumar23@chicmic.co.in");
                        message.To.Add(new MailAddress(alarm.Value));
                        message.Subject = "Reset your Password";
                        message.Body = $"link on the below link to verify and then reset your passoword \n" + modifiedLink;

                        SmtpClient Newclient = new SmtpClient();
                        Newclient.Credentials = new NetworkCredential("rakesh.kumar23@chicmic.co.in", "Chicmic@2022");
                        Newclient.Host = "mail.chicmic.co.in";
                        Newclient.Port = 587;
                        Newclient.EnableSsl = true;
                        Newclient.Send(message);*/
                        AlarmStorage.AlarmTimes.Remove(alarm.Key);
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
    public static class AlarmStorage
    {
        // public static Dictionary<DateTime, string> AlarmTimes { get; set; } = new Dictionary<DateTime, string>();
        public static Dictionary<DateTime, List<string>> AlarmTimes { get; set; } = new Dictionary<DateTime, List<string>>();

    }

}

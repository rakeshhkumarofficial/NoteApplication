﻿using Microsoft.AspNetCore.SignalR;
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

                    if (alarm.Key <= DateTime.Now)
                    {
                        Response res = new Response()
                        {
                            StatusCode = 200,
                            Message = " Reminder for Note",
                            IsSuccess = true,
                            Data = alarm
                        };
                        await _hubContext.Clients.Clients(alarm.Value).SendAsync("alarmTriggered", res);
                        var email = alarm.Value[1];
                        MailMessage message = new MailMessage();
                        message.From = new MailAddress("rakesh.kumar23@chicmic.co.in");
                        message.To.Add(new MailAddress(email));
                        message.Subject = "Reminder for Note";
                        message.Body = $"Title: " + alarm.Value[3]+" \n" + "Text: " + alarm.Value[4] + " \n" + "Image: " + alarm.Value[5];
                        SmtpClient Newclient = new SmtpClient();
                        Newclient.Credentials = new NetworkCredential("rakesh.kumar23@chicmic.co.in", "Chicmic@2022");
                        Newclient.Host = "mail.chicmic.co.in";
                        Newclient.Port = 587;
                        Newclient.EnableSsl = true;
                        Newclient.Send(message);
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

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using NoteApplication.Data;
using NoteApplication.Hubs;
using NoteApplication.Models;
using System;
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
                        await _hubContext.Clients.Clients(alarm.Value).SendAsync("alarmTriggered", alarm.Key);
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

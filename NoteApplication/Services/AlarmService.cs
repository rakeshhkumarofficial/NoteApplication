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

        /*protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                if (AlarmStorage.AlarmTime.HasValue && AlarmStorage.AlarmTime.Value <= DateTime.Now)
                {

                    await _hubContext.Clients.All.SendAsync("alarmTriggered");

                    AlarmStorage.AlarmTime = null;

                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }*/
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Loop through all the alarms in the shared object
                foreach (var alarm in AlarmStorage.AlarmTimes)
                {
                    // Check if the alarm has been triggered
                    if (alarm.Value <= DateTime.Now)
                    {
                        // Notify the caller that the alarm has been triggered
                        await _hubContext.Clients.All.SendAsync("alarmTriggered", alarm.Key);

                        // Remove the alarm from the shared object
                        AlarmStorage.AlarmTimes.Remove(alarm.Key);
                    }
                }

                // Wait for 1 second before checking again
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }


    }
    public static class AlarmStorage
    {
        // Shared object to store the alarm time
        // public static DateTime? AlarmTime { get; set; }
        public static Dictionary<string, DateTime> AlarmTimes { get; } = new Dictionary<string, DateTime>();
    }

}

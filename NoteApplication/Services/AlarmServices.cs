/*using Microsoft.AspNetCore.SignalR;
using System.Globalization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NoteApplication.Hubs;

namespace NoteApplication.Services
{
    public class AlarmServices
    {
        private class Alarm
        {
            public Guid NoteId { get; set; }
            public DateTime AlarmTime { get; set; }
        }

        // Dictionary to store the list of alarms for each connection
        private readonly Dictionary<string, List<Alarm>> _alarms = new Dictionary<string, List<Alarm>>();

        private readonly IHubContext<NoteHub> _hubContext;

        public AlarmServices(IHubContext<NoteHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void SetAlarm(string connectionId, Guid NoteId, string alarmTime)
        {
            DateTime dateTime = DateTime.ParseExact(alarmTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            // Add the alarm time for the specified object to the list
            if (!_alarms.ContainsKey(connectionId))
            {
                _alarms[connectionId] = new List<Alarm>();
            }
            _alarms[connectionId].Add(new Alarm { NoteId = NoteId, AlarmTime = dateTime });

            // Create a timer that will trigger the alarm after the specified time
            Timer timer = new Timer((state) =>
            {
                // Get the list of alarms for the current connection
                if (_alarms.TryGetValue(connectionId, out List<Alarm> alarms))
                {
                    // Get the alarm time for the current object
                    Alarm alarm = alarms.FirstOrDefault(a => a.NoteId == NoteId);

                    if (alarm != null)
                    {
                        // Trigger the ShowAlarm function for the current connection
                        _hubContext.Clients.Client(connectionId).SendAsync("ShowAlarm");

                        // Remove the alarm time from the list
                        alarms.Remove(alarm);
                    }
                }
            }, null, (dateTime - DateTime.Now), TimeSpan.Zero);
        }
    }
}



*/
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NoteApplication.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Security.Claims;
using NoteApplication.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NoteApplication.Services;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;
using System;

namespace NoteApplication.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NoteHub : Hub
    {
        private readonly NoteAPIDbContext _dbContext;
        Response response = new Response();
        private static Dictionary<string, string> Connections = new Dictionary<string, string>();
        public NoteHub(NoteAPIDbContext dbContext)
        {
            _dbContext = dbContext;         
        }
        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var user1 = httpContext.User;
            var email = user1.FindFirst(ClaimTypes.Name)?.Value;
            if (Connections.Keys.Contains(email))
            {
                Clients.Caller.SendAsync("AlreadyLogined");
                return base.OnConnectedAsync();
            }
            Connections.Add(email, Context.ConnectionId);
            Clients.All.SendAsync("Refresh");
            return base.OnConnectedAsync();
        }
        public async Task<Response> AddNote(AddNoteRequest request)
        {
            var httpContext = Context.GetHttpContext();
            var user = httpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var note = new Note()
            {
                NoteId = Guid.NewGuid(),
                CreatorEmail = email,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Title = request.Title,
                Text = null,
                Images = null,
                MessageType = null,
            };
            if (request.MessageType == 1)
            {
                note.Text = request.Message;
                note.MessageType = 1;
            }

            if (request.MessageType == 2) {
                note.Images = request.Message;
                note.MessageType = 2;
            }
            _dbContext.Notes.Add(note);
            _dbContext.SaveChanges();

            response.Data = note;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Note Added";
            await Clients.Caller.SendAsync("Others", response);
            
            return response;
        }

        public async Task<Response> AddReminder(string Id, string Time)
        {

            Guid NoteId = new Guid(Id);
            DateTime dateTime = DateTime.ParseExact(Time, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

            var httpContext = Context.GetHttpContext();
            var user = httpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;

            bool noteExist = _dbContext.Notes.Where(x => x.NoteId == NoteId).Any();
            if (!noteExist)
            {
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.Message = "Note Not exist";
                response.Data = null;
                await Clients.Caller.SendAsync("NoteReminder", response);
                return response;
            }

            if (dateTime < DateTime.Now)
            {
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = "Wrong Time";
                response.Data = null;
                await Clients.Caller.SendAsync("NoteReminder", response);
                return response;
            }

            var reminder = new Reminder()
            {
                RemId = Guid.NewGuid(),
                NoteId = NoteId,
                Email = email,
                RemindAt = dateTime
            };
            _dbContext.Reminders.Add(reminder);
            _dbContext.SaveChanges();
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Data = reminder;
            response.Message = "Reminder Added";
            //AlarmStorage.AlarmTime = dateTime;
            AlarmStorage.AlarmTimes[Id] = dateTime;
            await Clients.Caller.SendAsync("NoteReminder", response);         
            return response;
        }

        public async Task CancelReminder(string alarmId)
        {
            AlarmStorage.AlarmTimes.Remove(alarmId);
            await Clients.Caller.SendAsync("alarmCancelled");
        }


        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var user1 = httpContext.User;
            var email = user1.FindFirst(ClaimTypes.Name)?.Value;
            Connections.Remove(email);
            return base.OnDisconnectedAsync(exception);
        }
    }   
}

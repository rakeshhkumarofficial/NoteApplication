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
                IsArchived = false,
                IsTrashed = false,
                Pin = 0 ,
            };
            if(request.Pin == 1)
            {
                note.Pin = request.Pin;
            }
            if (request.MessageType == 1)
            {
                note.Text = request.Message;
                note.MessageType = 1;
            }

            if (request.MessageType == 2) {
                note.Images = request.URL;
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
        public async Task<Response> AddReminder(string Id, DateTime Time)
        {

            Guid NoteId = new Guid(Id);
           // DateTime dateTime = DateTime.ParseExact(Time, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            DateTime dateTime = DateTime.Parse(Time.ToString("yyyy-MM-dd HH:mm"));

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
            var notes = _dbContext.Notes.Where(x => x.CreatorEmail == email && x.IsTrashed == false && x.IsArchived == false && x.NoteId == NoteId).ToList();
            if (notes.Count == 0)
            {
                response.Data = null;
                response.StatusCode = 200;
                response.IsSuccess = false;
                response.Message = "You don't have the permission to add Reminder.";
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
            var note = _dbContext.Notes.Where(x=>x.NoteId == NoteId).FirstOrDefault();
            
        
        
            //AlarmStorage.AlarmTimes.Add(dateTime, Context.ConnectionId);
            AlarmStorage.AlarmTimes.Add(dateTime, new List<string> { Context.ConnectionId,note.CreatorEmail,note.NoteId.ToString(),note.Title, note.Text, note.Images });

            await Clients.Caller.SendAsync("NoteReminder", response);         
            return response;
        }       
        public async Task<Response> GetNotes()
        {
            var httpContext = Context.GetHttpContext();
            var user = httpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var notes = _dbContext.Notes.Where(x=>x.CreatorEmail == email && x.IsTrashed == false && x.IsArchived == false && x.Pin == 0 ).ToList();
            if (notes.Count == 0)
            {
                response.Data = null;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "No Notes Available";
                await Clients.Caller.SendAsync("RecieveNotes", response);
                return response;
            }
            response.Data = notes;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "All Notes";
            await Clients.Caller.SendAsync("RecieveNotes",response);
            return response;
        }

        public async Task<Response> GetPinnedNotes()
        {
            var httpContext = Context.GetHttpContext();
            var user = httpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var notes = _dbContext.Notes.Where(x => x.CreatorEmail == email && x.IsTrashed == false && x.IsArchived == false && x.Pin==1).OrderByDescending(x=>x.UpdatedAt).ToList();
            if (notes.Count == 0)
            {
                response.Data = null;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "No Notes Available";
                await Clients.Caller.SendAsync("RecievePinnedNotes", response);
                return response;
            }
            response.Data = notes;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "All Pinned Notes";
            await Clients.Caller.SendAsync("RecievePinnedNotes", response);
            return response;
        }

        public async Task<Response> GetArchiveNote()
        {
            var httpContext = Context.GetHttpContext();
            var user = httpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var notes = _dbContext.Notes.Where(x => x.CreatorEmail == email && x.IsArchived == true).ToList();
            if(notes.Count == 0)
            {
                response.Data = null;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Archive is Empty";
                await Clients.Caller.SendAsync("RecieveArchiveNotes", response);
                return response;
            }
            response.Data = notes;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "All Notes";
            await Clients.Caller.SendAsync("RecieveArchiveNotes", response);
            return response;
        }

        public async Task<Response> GetTrashNote()
        {
            var httpContext = Context.GetHttpContext();
            var user = httpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var notes = _dbContext.Notes.Where(x => x.CreatorEmail == email && x.IsTrashed == true).ToList();
            if (notes.Count == 0)
            {
                response.Data = null;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Trash is Empty";
                await Clients.Caller.SendAsync("RecieveTrashNotes", response);
                return response;
            }
            response.Data = notes;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "All Notes";
            await Clients.Caller.SendAsync("RecieveTrashNotes", response);
            return response;
        }
        public async Task<Response> ArchiveNote(string Id)
        {
            Guid NoteId = new Guid(Id);
            var httpContext = Context.GetHttpContext();
            var user = httpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var note = _dbContext.Notes.Find(NoteId);
            bool creator = note.CreatorEmail == email;
            if (creator)
            {
                note.UpdatedAt = DateTime.Now;
                note.IsArchived = true;
                _dbContext.SaveChanges();
                response.Data = note;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Note is Archived";
                await Clients.Caller.SendAsync("RecievedArchive", response);
                return response;
            }
            response.Data = null;
            response.StatusCode = 200;
            response.IsSuccess = false;
            response.Message = "You don't have the permission to delete.";
            await Clients.Caller.SendAsync("RecievedArchive", response);
            return response;
        }
        public async Task<Response> DeleteNote(string Id)
        {
            Guid NoteId = new Guid(Id);
            var httpContext = Context.GetHttpContext();
            var user = httpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var note = _dbContext.Notes.Find(NoteId);
            bool creator = note.CreatorEmail == email ;
            if (creator)
            {
                note.UpdatedAt = DateTime.Now;
                note.IsArchived = false;
                note.IsTrashed = true;
                _dbContext.SaveChanges();
                response.Data = note;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Note is Trashed";
                await Clients.Caller.SendAsync("RecievedTrash", response);
                return response;
            }
            response.Data = null;
            response.StatusCode = 200;
            response.IsSuccess = false;
            response.Message = "You don't have the permission to delete.";
            await Clients.Caller.SendAsync("RecievedTrash", response);
            return response;

        }
        public async Task<Response> PinNotes(string Id , int Pin)
        {
            Guid NoteId = new Guid(Id);
            var httpContext = Context.GetHttpContext();
            var user = httpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var note = _dbContext.Notes.Find(NoteId);
            bool creator = note.CreatorEmail == email;
            if (creator)
            {
                note.Pin = Pin;
                note.UpdatedAt = DateTime.Now;
                _dbContext.SaveChanges();
                response.Data = note;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Note is Pinned";
                await Clients.Caller.SendAsync("RecievedPinnedNotes", response);
                return response;
            }
            response.Data = null;
            response.StatusCode = 200;
            response.IsSuccess = false;
            response.Message = "You don't have the permission to Pin";
            await Clients.Caller.SendAsync("RecievedPinnedNotes", response);
            return response;
        }
        
        

       /* public async Task CancelReminder(string alarmId)
        {
            AlarmStorage.AlarmTimes.Remove(alarmId);
            await Clients.Caller.SendAsync("alarmCancelled");
        }  */ 
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

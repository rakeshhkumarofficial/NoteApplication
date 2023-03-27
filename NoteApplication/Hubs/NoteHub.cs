using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NoteApplication.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Security.Claims;
using NoteApplication.Models;

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
        public async Task AddNote(AddNoteRequest request)
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
                Images = null
             };
            if(request.ContentType == 1)
            {
                note.Text = request.Content;
            }

            if(request.ContentType == 2) { 
               note.Images = request.Content;         
            };

            _dbContext.Notes.Add(note);
            _dbContext.SaveChanges();

            response.Data = note;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Note Added";
            await Clients.Caller.SendAsync("Others", response);
           
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

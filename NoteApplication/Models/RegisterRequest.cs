﻿namespace NoteApplication.Models
{
    public class RegisterRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public long Phone { get; set; }
        public string? Password { get; set; }
    }
}
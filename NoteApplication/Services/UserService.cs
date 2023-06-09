﻿using Microsoft.IdentityModel.Tokens;
using NoteApplication.Data;
using NoteApplication.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace NoteApplication.Services
{
    public class UserService : IUserService
    {
        Response response = new Response();
        private readonly NoteAPIDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public UserService(NoteAPIDbContext dbContext , IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Register a New User
        public Response Register(RegisterRequest user)
        {
            response.Data = null;
            response.StatusCode = 400;
            response.IsSuccess=false;
            response.Message = "Email already exits";
            if (user.FirstName == null || user.FirstName == "")
            {
                response.Message = "FirstName cannot be empty";
                return response;
            }
            if (user.LastName == null || user.LastName == "")
            {
                response.Message = "LastName cannot be empty";
                return response;
            }
            string regexPatternEmail = "^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$";
            if (!Regex.IsMatch(user.Email, regexPatternEmail))
            {
                response.Message = "Enter Valid email";
                return response;
            }
            string regexPatternPhone = "^[6-9]\\d{9}$";
            if (!Regex.IsMatch(user.Phone.ToString(), regexPatternPhone))
            {
                response.Message = "Enter Valid Phone Number";
                return response;
            }
            string regexPatternPassword = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
            if (!Regex.IsMatch(user.Password, regexPatternPassword))
            {
                response.Message = "Password should contain one Upper, lower alphabet and one special symbol, minimum 8 Characters";
                return response;
            }
           
            bool IsUserExists = _dbContext.Users.Where(u => u.Email == user.Email).Any();
            if (!IsUserExists)
            {
                CreatePasswordHash(user.Password, out byte[] PasswordHash, out byte[] PasswordSalt);
                var obj = new User()
                {
                    UserId = Guid.NewGuid(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PasswordHash = PasswordHash,
                    PasswordSalt = PasswordSalt,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Phone = user.Phone,
                    ProfileImage = "wwwroot\\Images\\BoyImage_20230330065257363.png"
                };
                _dbContext.Users.Add(obj);
                _dbContext.SaveChanges();
                string token = CreateToken(obj, _configuration);
                int len = obj == null ? 0 : 1;
                if (len == 1)
                {
                    OutputRequest outputRequest = new OutputRequest();
                    outputRequest.FirstName = obj.FirstName;
                    outputRequest.LastName = obj.LastName;
                    outputRequest.Email = obj.Email;
                    outputRequest.Token = token;
                    response.Data = outputRequest;
                    response.StatusCode = 200;
                    response.IsSuccess = true;
                    response.Message = "User Registered Successfully";
                    return response;
                }
            }
            return response;
        }

        // Create a JWT Token whenever user register or login
        private string CreateToken(User obj, IConfiguration _configuration)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,obj.Email),
                new Claim(ClaimTypes.Role,"Login")

           };
            var Key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        // Create a PasswordHash and PasswordSalt for the Password
        private void CreatePasswordHash(string Password, out byte[] PasswordHash, out byte[] PasswordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                PasswordSalt = hmac.Key;
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
            }
        }

        // Login a User
        public Response Login(LoginRequest login)
        {
            response.Data = null;
            response.StatusCode = 400;
            response.IsSuccess = false;
            string regexPatternEmail = "^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$";
            if (!Regex.IsMatch(login.Email, regexPatternEmail))
            {
                response.Message = "Enter Valid email";
                return response;
            }
            if (login.Password == "" || login.Password == null)
            {
                response.Message = "Please Enter the Password";
                return response;
            }

            var obj = _dbContext.Users.Where(u => u.Email == login.Email).FirstOrDefault();

            if (obj == null)
            {
                response.Data = null;
                response.StatusCode = 404;
                response.Message = "Wrong Email";
                return response;
            }
            if (!VerifyPasswordHash(login.Password, obj.PasswordHash, obj.PasswordSalt))
            {
                response.StatusCode = 404;
                response.Message = "Wrong Password";
                return response;
            }
            string token = CreateToken(obj, _configuration);
            OutputRequest outputRequest = new OutputRequest();
            outputRequest.FirstName = obj.FirstName;
            outputRequest.LastName = obj.LastName;
            outputRequest.Email = obj.Email;
            outputRequest.Token = token;
            response.Data = outputRequest;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Login Successfull";
            return response;
        }

        // Verify the LoggedIn User Password 
        private bool VerifyPasswordHash(string Password, byte[] PasswordHash, byte[] PasswordSalt)
        {
            using (var hmac = new HMACSHA512(PasswordSalt))
            {
                byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
                return computedHash.SequenceEqual(PasswordHash);
            }
        }

        // Change the old Password
        public Response ChangePassword(ChangePasswordRequest changePasswordRequest, string email)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var obj = _dbContext.Users.FirstOrDefault(x => x.Email == email);
            int len = obj == null ? 0 : 1;

            if (len == 0)
            {
                response.Message = "User Not Found";
                return response;
            }
            if (!VerifyPasswordHash(changePasswordRequest.OldPassword, obj.PasswordHash, obj.PasswordSalt))
            {
                response.Message = "OldPassword is Wrong";
                return response;
            }

            string regexPatternPassword = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
            if (!Regex.IsMatch(changePasswordRequest.NewPassword, regexPatternPassword))
            {
                response.StatusCode = 400;
                response.Message = "Password should be of 8 length contains atleast one Upper, lower alphabet and one special symbol ";
                return response;
            }
         
            CreatePasswordHash(changePasswordRequest.NewPassword, out byte[] PasswordHash, out byte[] PasswordSalt);
            obj.PasswordHash = PasswordHash;
            obj.PasswordSalt = PasswordSalt;
            obj.UpdatedAt = DateTime.Now;
            _dbContext.SaveChanges();

            response.Data = obj;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Password Changed Successfully";
            return response;
        }

        // Update a User Profile
        public Response UpdateProfile(UpdateProfileRequest update, string email)
        {
            response.StatusCode = 400;
            response.Data = null;
            response.IsSuccess = false;
            var obj = _dbContext.Users.FirstOrDefault(x => x.Email == email);
            int len = obj == null ? 0 : 1;
            if (len == 0)
            { 
                response.Message = "You have changed the email, Login Again"; 
                return response;
            }
            if (update.FirstName != null) { obj.FirstName = update.FirstName; }
            if (update.LastName != null) { obj.LastName = update.LastName; }
            if (update.Phone != -1 || update.Phone == 0)
            {
                string regexPatternPhone = "^[6-9]\\d{9}$";
                if (!Regex.IsMatch(update.Phone.ToString(), regexPatternPhone))
                {
                    response.Message = "Enter Valid Phone Number";
                    return response;
                }
                obj.Phone = update.Phone;
            }
            if (update.Email != null)
            {
                string regexPatternEmail = "^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$";
                if (!Regex.IsMatch(update.Email, regexPatternEmail))
                {
                    response.Message = "Enter Valid email";
                    return response;
                }
                obj.Email = update.Email;
            }
            
            obj.UpdatedAt = DateTime.Now;
            _dbContext.SaveChanges();
            response.Data = obj;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "User details updated";
            return response;
        }

        // Upload Profie Image
        public Response FileUpload(FileUploadRequest upload, string email)
        {
            var obj = _dbContext.Users.FirstOrDefault(x => x.Email == email);
            int len = obj == null ? 0 : 1;
            if (len == 0)
            {
                response.Data = null;
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.Message = "User Not Found";
                return response;
            }
            string fileName = Path.GetFileNameWithoutExtension(upload.File.FileName);
            string fileExt = Path.GetExtension(upload.File.FileName);
            string uniqueFileName = $"{fileName}_{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}{fileExt}";
            string folderName = "wwwroot//Images//";
            string path = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var fullpath = path + "//" + uniqueFileName;
            var filestream = File.Create(fullpath);
            upload.File.CopyTo(filestream);
            filestream.Close();
            obj.ProfileImage = folderName + uniqueFileName;
            _dbContext.SaveChanges();
            response.Data = folderName + uniqueFileName;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "File Uploaded Successfully..";
            return response;
        }

        // Get details of LoggedIn User
        public Response GetUser(string email)
        {
            var obj = _dbContext.Users.FirstOrDefault(x => x.Email == email);
            Response res = new Response();
            res.StatusCode = 200;
            res.Message = "User Details";
            res.Data = new { obj.FirstName, obj.LastName, obj.Email, obj.Phone, obj.ProfileImage };
            return res;
        }
    }
}

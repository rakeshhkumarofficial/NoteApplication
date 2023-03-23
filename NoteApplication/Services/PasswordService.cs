using NoteApplication.Data;
using NoteApplication.Models;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NoteApplication.Services
{
    public class PasswordService : IPasswordService
    {
        Response response = new Response();
        private readonly NoteAPIDbContext _dbContext;
        public readonly IConfiguration _configuration;
        public PasswordService(NoteAPIDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;

        }
        public Response ForgetPassword(ForgetPasswordRequest forget)
        {
            response.StatusCode = 400;
            response.Data = null;
            response.IsSuccess = false;
            if (forget.Url == null || forget.Url == "")
            {
                response.Message = "Url Needed";
                return response;
            }
            if (forget.Email == null || forget.Email == "")
            {
                response.Message = "Please Enter The Email";
                return response;
            }
            string regexPatternEmail = "^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$";
            if (!Regex.IsMatch(forget.Email, regexPatternEmail))
            {
                response.Message = "Enter Valid email";
                return response;
            }

            var user = _dbContext.Users.FirstOrDefault(x => x.Email == forget.Email);
            bool IsUserExists = _dbContext.ForgetPasswords.Where(u => u.Email == forget.Email).Any();

            if (user == null)
            {
                response.StatusCode = 404;
                response.Message = "Email Not found";
                return response;
            }
            string urldirect = forget.Url;
            UriBuilder builder = new UriBuilder(urldirect);

            if (!IsUserExists)
            {
                ForgetPassword p = new ForgetPassword();
                p.Id = Guid.NewGuid();
                p.Email = forget.Email;
                p.ResetPasswordToken = CreateToken(user, _configuration);
                p.ExpiresAt = DateTime.Now.AddDays(1);
                _dbContext.ForgetPasswords.Add(p);
                _dbContext.SaveChanges();

                string encodedToken = System.Net.WebUtility.UrlEncode(p.ResetPasswordToken);
                builder.Query = "token=" + encodedToken;
                string NewUrlLink = builder.ToString();

                MailMessage message = new MailMessage();
                message.From = new MailAddress("rakesh.kumar23@chicmic.co.in");
                message.To.Add(new MailAddress(forget.Email));
                message.Subject = "Reset your Password";
                message.Body = $"Click on the below link to verify and then reset your passoword \n" + NewUrlLink;

                SmtpClient Newclient = new SmtpClient();
                Newclient.Credentials = new NetworkCredential("rakesh.kumar23@chicmic.co.in", "Chicmic@2022");
                Newclient.Host = "mail.chicmic.co.in";
                Newclient.Port = 587;
                Newclient.EnableSsl = true;
                Newclient.Send(message);

                response.Data = p.Email;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Verification Mail is Sent";
                return response;
            }
            var fpuser = _dbContext.ForgetPasswords.FirstOrDefault(x => x.Email == forget.Email);
            fpuser.ResetPasswordToken = CreateToken(user, _configuration);
            fpuser.ExpiresAt = DateTime.Now.AddDays(1);
            _dbContext.SaveChanges();

            string encodedtoken = System.Net.WebUtility.UrlEncode(fpuser.ResetPasswordToken);
            builder.Query = "token=" + encodedtoken;
            string newUrlLink = builder.ToString();

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("rakesh.kumar23@chicmic.co.in");
            msg.To.Add(new MailAddress(forget.Email));
            msg.Subject = "Reset your Password";
            msg.Body = $"Link on the below link to verify and then reset your passoword \n" + newUrlLink;

            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential("rakesh.kumar23@chicmic.co.in", "Chicmic@2022");
            client.Host = "mail.chicmic.co.in";
            client.Port = 587;
            client.EnableSsl = true;
            client.Send(msg);

            response.Data = fpuser.Email;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Verification Mail is Sent";
            return response;
        }
        public Response ResetPassword(ResetPasswordRequest reset, string email)
        {
            string regexPatternPassword = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
            if (!Regex.IsMatch(reset.NewPassword, regexPatternPassword))
             {
                response.StatusCode = 400;
                response.Message = "Password should be of 8 length contains atleast one Upper, lower alphabet and one special symbol ";
                response.Data = null;
                return response;
             }
             var dbuser = _dbContext.Users.FirstOrDefault(x => x.Email == email);
             var fpUser = _dbContext.ForgetPasswords.FirstOrDefault(x => x.Email == dbuser.Email);

             if (fpUser == null || fpUser.ExpiresAt < DateTime.Now)
              {
                 response.Data = null;
                 response.StatusCode = 404;
                 response.Message = "Link Expired";
                 return response;
              }

              CreatePasswordHash(reset.NewPassword, out byte[] PasswordHash, out byte[] PasswordSalt);
              dbuser.PasswordHash = PasswordHash;
              dbuser.PasswordSalt = PasswordSalt;
              dbuser.UpdatedAt = DateTime.Now;
              _dbContext.SaveChanges();

              response.Data = dbuser;
              response.StatusCode = 200;
              response.IsSuccess = true;
              response.Message = "Password Reset Successfully";
              _dbContext.Remove(fpUser);
              _dbContext.SaveChanges();
              return response;
            }
        private string CreateToken(User obj, IConfiguration _configuration)
            {
                List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,obj.Email),
                new Claim(ClaimTypes.Role,"Reset")
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
        private void CreatePasswordHash(string Password, out byte[] PasswordHash, out byte[] PasswordSalt)
            {
                using (var hmac = new HMACSHA512())
                {
                    PasswordSalt = hmac.Key;
                    PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
                }
            }

     }
}

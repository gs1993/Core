using BC = BCrypt.Net.BCrypt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Helpers;
using WebApi.Dto.Accounts;
using WebApi.Entities.Accounts;
using Shared.Options;

namespace WebApi.Services
{
    public interface IAccountService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
        void Register(RegisterRequest model, string origin);
        void VerifyEmail(string token);
        void ForgotPassword(ForgotPasswordRequest model, string origin);
        void ValidateResetToken(ValidateResetTokenRequest model);
        void ResetPassword(ResetPasswordRequest model);
        IEnumerable<AccountResponse> GetAll();
        AccountResponse GetById(long id);
        AccountResponse Update(long id, UpdateRequest model);
        void Delete(long id);
        void ChangeRole(long id, Role role);
    }

    public class AccountService : IAccountService
    {
        private readonly DataContext _context;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;

        public AccountService(DataContext context, IOptions<AppSettings> appSettings, IEmailService emailService)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _emailService = emailService;
        }


        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);
            if (account == null || !account.IsVerified || !BC.Verify(model.Password, account.Password.PasswordHash))
                throw new AppException("Email or password is incorrect");

            var refreshToken = GenerateRefreshToken(ipAddress);
            account.AddRefreshTokenAndRemoveOldTokens(refreshToken, DateTime.UtcNow, _appSettings.RefreshTokenTtlInDays);
            _context.Update(account);
            _context.SaveChanges();

            var jwtToken = GenerateJwtToken(account);
            return AuthenticateResponse.CreateFromModel(account, jwtToken, refreshToken.Token);
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            var (refreshToken, account) = GetRefreshToken(token);
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoke(DateTime.UtcNow, ipAddress, newRefreshToken.Token);
            account.AddRefreshTokenAndRemoveOldTokens(newRefreshToken, DateTime.UtcNow, _appSettings.RefreshTokenTtlInDays);

            _context.Update(account);
            _context.SaveChanges();

            var jwtToken = GenerateJwtToken(account);
            return AuthenticateResponse.CreateFromModel(account, jwtToken, refreshToken.Token);
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var (refreshToken, account) = GetRefreshToken(token);
            refreshToken.Revoke(DateTime.UtcNow, ipAddress);
            _context.Update(account);
            _context.SaveChanges();
        }

        public void Register(RegisterRequest model, string origin)
        {
            if (_context.Accounts.Any(x => x.Email == model.Email))
            {
                SendAlreadyRegisteredEmail(model.Email, origin);
                return;
            }

            var nameResult = Name.Create(model.Title, model.FirstName, model.LastName);
            if (nameResult.IsFailure)
                throw new AppException(nameResult.Error);

            var emailResult = Email.Create(model.Email);
            if (emailResult.IsFailure)
                throw new AppException(emailResult.Error);

            string passwordSalt = BC.GenerateSalt();
            string passwordHash = BC.HashPassword(model.Password, passwordSalt);
            var passwordResult = Password.Create(passwordHash, passwordSalt);
            if (passwordResult.IsFailure)
                throw new AppException(passwordResult.Error);

            string verificationToken = RandomTokenString();

            var role = _context.Accounts.Any() ? Role.User : Role.Admin;

            var createAccountResult = Account.Create(nameResult.Value, emailResult.Value,
                passwordResult.Value, role, verificationToken, DateTime.UtcNow);
            if (createAccountResult.IsFailure)
                throw new AppException(createAccountResult.Error);

            var account = createAccountResult.Value;
            _context.Accounts.Add(account);
            _context.SaveChanges();

            SendVerificationEmail(account, origin);
        }

        public void VerifyEmail(string token)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.VerificationToken == token);
            if (account == null) 
                throw new AppException("Verification failed");

            account.SetVerificationSucceded(DateTime.UtcNow);

            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);
            if (account == null) return;

            var resetToken = RandomTokenString();
            var tokenExpireDate = DateTime.UtcNow.AddDays(1); // TODO: move day into appsettings
            account.AddResetToken(resetToken, tokenExpireDate);

            _context.Accounts.Update(account);
            _context.SaveChanges();

            SendPasswordResetEmail(account, origin);
        }

        public void ValidateResetToken(ValidateResetTokenRequest model)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.ResetToken == model.Token && x.ResetTokenExpires > DateTime.UtcNow);
            if (account == null)
                throw new AppException("Invalid token");
        }

        public void ResetPassword(ResetPasswordRequest model)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.ResetToken == model.Token && x.ResetTokenExpires > DateTime.UtcNow);
            if (account == null)
                throw new AppException("Invalid token");

            string newPasswordSalt = BC.GenerateSalt();
            string newPasswordHash = BC.HashPassword(model.Password);
            var passwordResult = Password.Create(newPasswordHash, newPasswordSalt);
            if (passwordResult.IsFailure)
                throw new AppException(passwordResult.Error);

            var passwordResetDate = DateTime.UtcNow;
            account.ResetPassword(passwordResult.Value, passwordResetDate);
            
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public IEnumerable<AccountResponse> GetAll()
        {
            var accounts = _context.Accounts;
            return accounts.Select(x => AccountResponse.CreateFromModel(x));
        }

        public AccountResponse GetById(long id)
        {
            var account = GetAccount(id);
            return AccountResponse.CreateFromModel(account);
        }

        public AccountResponse Update(long id, UpdateRequest model)
        {
            var account = GetAccount(id);

            var newNameResult = Name.Create(model.Title, model.FirstName, model.LastName);
            if (newNameResult.IsFailure)
                throw new AppException(newNameResult.Error);

            account.Update(newNameResult.Value, DateTime.UtcNow);
            _context.Accounts.Update(account);
            _context.SaveChanges();

            return AccountResponse.CreateFromModel(account);
        }

        public void Delete(long id)
        {
            var account = GetAccount(id);
            account.Delete(DateTime.UtcNow);
            _context.SaveChanges();
        }

        public void ChangeRole(long id, Role role)
        {
            var account = GetAccount(id);
            account.ChangeRole(role);
            _context.SaveChanges();
        }


        private Account GetAccount(long id)
        {
            var account = _context.Accounts.Find(id);
            if (account == null) 
                throw new KeyNotFoundException("Account not found");

            return account;
        }

        private (RefreshToken, Account) GetRefreshToken(string token)
        {
            var account = _context.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token)) ?? throw new AppException("Invalid token");
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive) 
                throw new AppException("Invalid token");

            return (refreshToken, account);
        }

        private string GenerateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(_appSettings.ExpirationTimeInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var refreshTokenResult = Entities.Accounts.RefreshToken.Create(
                RandomTokenString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(7), ipAddress);
            if (refreshTokenResult.IsFailure)
                throw new AppException(refreshTokenResult.Error);

            return refreshTokenResult.Value;
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void SendVerificationEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{account.VerificationToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Verify Email",
                html: $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
            );
        }

        private void SendAlreadyRegisteredEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
                message = $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
            else
                message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

            _emailService.Send(
                to: email,
                subject: "Sign-up Verification API - Email Already Registered",
                html: $@"<h4>Email Already Registered</h4>
                         <p>Your email <strong>{email}</strong> is already registered.</p>
                         {message}"
            );
        }

        private void SendPasswordResetEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{account.ResetToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message}"
            );
        }
    }
}

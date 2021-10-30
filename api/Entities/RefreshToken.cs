using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities
{
    [Owned]
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public Account Account { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsActive => !Revoked.HasValue && !IsExpired;


        public void Revoke(DateTime revoked, string revokedByIp, string replacedByToken = null)
        {
            if (revoked == default)
                throw new ArgumentNullException(nameof(revoked));
            if (string.IsNullOrWhiteSpace(revokedByIp))
                throw new ArgumentNullException(nameof(revokedByIp));

            Revoked = revoked;
            RevokedByIp = revokedByIp;
            ReplacedByToken = string.IsNullOrWhiteSpace(replacedByToken) ? string.Empty : replacedByToken;
        }
    }
}
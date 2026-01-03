using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllulExpressApi.Models
{
    public class Employees
    {
        public int Id { get; set; } // Primary Key (auto)
        public string? Name { get; set; }
        [Phone]
        [Required]
        public string? Phonenum1 { get; set; }
        public string Phonenum2 { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Password { get; set; }

        public string? Role { get; set; }
        public string? IDimagefront { get; set; }
        public string? IDimageback { get; set; }
        public DateTime Savedate { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public string? Language { get; set; }

        public string? OtpCode { get; set; }
        public DateTime? OtpExpiry { get; set; }
        [Required]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role? RoleNavigation { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace mobile_api_test.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string? Username { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string? PasswordHash { get; set; } = string.Empty;
        public string? FullName { get; set; }

        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}

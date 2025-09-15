using System.ComponentModel.DataAnnotations;

namespace mobile_api_test.Models
{
    public class MessageUser
    {
        [Key]
        public int MessageId { get; set; }
        public string? UserName { get; set; }
        public string? Content { get; set; }
        public DateTime SendTime { get; set; } = DateTime.Now;
    }
}

using System.Text.Json.Serialization;

namespace HisaberAccountServer.Models.LoginSystem
{
    public class SignUp
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
    }
}

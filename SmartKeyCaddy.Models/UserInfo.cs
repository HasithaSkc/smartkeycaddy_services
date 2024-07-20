using System.Text.Json.Serialization;

namespace SmartKeyCaddy.Models
{
    public class UserInfo
    {
        [JsonPropertyName("UserInfoId")]
        public Guid UserUuId { get; set; }

        [JsonIgnore]
        public int UserInfoId { get; set; }
        public string UserId { get; set;}
        public string Password { get; set; }
        public DateTime LastUpdated { get; set; }
        private DateTime _createdDate{ get; set; }
    }
}
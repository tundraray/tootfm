using System.Runtime.Serialization;

namespace Posmotrim.TootFM.PhoneServices.Models
{
    [DataContract]
    public class Account
    {
        [DataMember(Name = "account")]
        public User User { get; set; }
    }

    [DataContract]
    public class User
    {
        [DataMember(Name = "user_token")]
        public string Token { get; set; }
        [DataMember(Name = "token")]
        public string FoursquareToken { get; set; }
        
       
    }
}
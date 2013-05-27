using System.Runtime.Serialization;

namespace Posmotrim.TootFM.PhoneServices.Models
{
    [DataContract]
    public class DeezerTrack
    {
         [DataMember(Name = "preview")]
        public string Preview { get; set; }
    }
}
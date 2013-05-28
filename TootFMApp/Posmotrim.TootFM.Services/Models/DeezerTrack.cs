using System.Runtime.Serialization;

namespace Posmotrim.TootFM.PhoneServices.Models
{
    [DataContract]
    public class DeezerTrack
    {
         [DataMember(Name = "preview")]
        public string Preview { get; set; }

         [DataMember(Name = "artist")]
         public DeezerArtist Artist { get; set; }

         [DataMember(Name = "album")]
         public DeezerAlbum Album { get; set; }
    }


    [DataContract]
    public class DeezerAlbum
    {
        [DataMember(Name = "cover")]
        public string Picture { get; set; }
    }

    [DataContract]
    public class DeezerArtist
    {
        [DataMember(Name = "picture")]
        public string Picture { get; set; }
    }
}
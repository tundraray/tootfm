using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Posmotrim.TootFM.PhoneServices.Models
{
    [DataContract]
    public class Venues
    {
        [DataMember(Name = "venues")]
        public List<Venue> Items { get; set; }
    }

    [DataContract]
    public class CheckIn
    {
        [DataMember(Name = "venue")]
        public Venue Venue { get; set; }
    }

    [DataContract]
    public class CheckInW
    {
        [DataMember(Name = "check_in")]
        public CheckIn CheckIn { get; set; }
    }

    [DataContract]
    public class Playlist
    {
        [DataMember(Name = "general_playlist")]
        public List<Track> General { get; set; }
        [DataMember(Name = "current_playlist")]
        public List<Track> Current { get; set; }
    }
}
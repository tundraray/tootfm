using System.Runtime.Serialization;

namespace Posmotrim.TootFM.PhoneServices.Models
{
    [DataContract]
    public class Venue
    {
         [DataMember(Name = "id")]
        public int Id { get; set; }
         [DataMember(Name = "lat")]
        public float Lat { get; set; }
         [DataMember(Name = "lng")]
        public float Lng { get; set; }
         [DataMember(Name = "name")]
        public string Name { get; set; }
        public VenueLocation Location { get; set; }

        public bool IsUser { get; set; }
    }

    [DataContract]
    public class VenueLocation
    {
        [DataMember(Name = "address")]
        public string Address { get; set; }
    }
}
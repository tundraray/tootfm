using System;
using System.Runtime.Serialization;
using GalaSoft.MvvmLight;

namespace Posmotrim.TootFM.PhoneServices.Models
{
    [DataContract]
    public class Track 
    {
        [DataMember(Name = "album")]
        public string Album { get; set; }
        [DataMember(Name = "deezer_id")]
        public int DeezerId { get; set; }
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "artist")]
        public Artist Artist { get; set; }

      

        //[IgnoreDataMember]
        //public bool IsPlay
        //{
        //    get { return isPlay; }
        //    set
        //    {
        //        isPlay = value;
        //        RaisePropertyChanged("IsPlay");
        //    }
        //}
      
        public string Description
        {
            get
            {
                return string.Format("{0}{1}", this.Artist != null ? Artist.Name : "",
                                     !string.IsNullOrEmpty(Album) ? string.Format(", {0}", Album) : "");
            }
        }
        [DataMember(Name = "deezerTrack")]
        public DeezerTrack Audio { get; set; }
    }
    [DataContract]
    public class Artist
    {
        [DataMember(Name = "mbid")]
        public string MBID { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }


    public class ObsTrack:ObservableObject
    {
        private bool isPlay = false;
      
        public bool IsPlay
        {
            get { return isPlay; }
            set
            {
                isPlay = value;
                RaisePropertyChanged("IsPlay");
            }
        }
        private Track track;
        public Track Track
        {
            get { return track; }
            set
            {
                track = value;
                RaisePropertyChanged("Track");
            }
        }

        
    }
}
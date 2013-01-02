using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Altaria.Model
{
    [DataContract]
    public class Metadata
    {
        [DataMember]
        public string artist { get; set; }

        [DataMember]
        public string release { get; set; }

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public string genre { get; set; }

        [DataMember]
        public int bitrate { get; set; }

        [DataMember]
        public int sample_rate { get; set; }

        [DataMember]
        public int duration { get; set; }

        [DataMember]
        public string filename { get; set; }

        [DataMember]
        public int samples_decoded { get; set; }

        [DataMember]
        public int given_duration { get; set; }

        [DataMember]
        public int start_offset { get; set; }

        [DataMember]
        public double version { get; set; }

        [DataMember]
        public double codegen_time { get; set; }

        [DataMember]
        public double decode_time { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Altaria_DRM.Model
{
    [DataContract]
    public class SongInfo
    {
        [DataMember]
        public Metadata metadata { get; set; }

        [DataMember]
        public int code_count { get; set; }

        [DataMember]
        public String code { get; set; }

        [DataMember]
        public int tag { get; set; }
    }
}
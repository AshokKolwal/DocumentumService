using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DocumentumService.DataContracts
{
    [DataContract]
    public class ResponseBase
    {
        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public string Message { get; set; }             

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DocumentumService.DataContracts
{
    [DataContract]
    public class UploadDocumentEntity
    {
        [DataMember]
        public string DocumentName { get; set; }
        [DataMember]
        public string IdfID { get; set; }
        [DataMember]
        public string IdfPath { get; set; }
        [DataMember]
        public int Status { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
    }
}
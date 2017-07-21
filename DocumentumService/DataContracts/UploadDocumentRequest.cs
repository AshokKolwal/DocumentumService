using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DocumentumService.DataContracts
{
    [DataContract]
    public class UploadDocumentRequest
    {
        [DataMember]
        public string DocInputFolderPath { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DocumentumService.DataContracts
{
    [DataContract]
    public class UploadDocumentResponse:ResponseBase
    {        
        [DataMember]
        public List<UploadDocumentEntity> Result { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DocumentumService.DataContracts;

namespace DocumentumService.ServiceContracts
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDocBrockerService" in both code and config file together.
    [ServiceContract]
    public interface IDocBrockerService
    {
        [OperationContract]
        UploadDocumentResponse UploadDocuments(UploadDocumentRequest request);
    }
}

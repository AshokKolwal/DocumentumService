
#region Revesion History
/*************************************************************************************************************************
   BugID	     Date               Author              Method
 * -----------------------------------------------------------------------------------------------------------------------   
  0000          11-Jul-2017        Ashok Kolwal         Created

**************************************************************************************************************************/
#endregion Revesion History

#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DocumentumService.ServiceContracts;
using DocumentumService.DataContracts;
using DocumentumService.Utility;
using System.ServiceModel.Description;
using System.Web;
using System.ServiceModel.Activation;

#endregion

namespace DocumentumService.Services
{
  
    public class DocBrockerService : IDocBrockerService
    {
        #region Service Exposed Methods 

        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: This method is used for upload the document with it's content in documentum.        
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="request">Local drive document input folder path</param>
        /// <returns>document and it's idfID</returns>
        
        public UploadDocumentResponse UploadDocuments(UploadDocumentRequest request)
        {
            UploadDocumentResponse response = new UploadDocumentResponse();
            response.Status = 1;
            response.Message = "SUCCESS";
            try
            {
                response.Result = new DocBrockerServiceUtility().UploadDoc(request.DocInputFolderPath);
            }
            catch (Exception ex)
            {
                response.Status = 0;
                response.Message = ServiceBase.GetExceptionMessage(ex);
            }
            return response;

        }


        #endregion 
    }
}

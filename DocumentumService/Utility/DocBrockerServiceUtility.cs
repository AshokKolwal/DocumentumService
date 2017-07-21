#region Revesion History
/*************************************************************************************************************************
   BugID	     Date               Author              Method
 * -----------------------------------------------------------------------------------------------------------------------   
  0000          11-Jul-2017        Ashok Kolwal         Created

**************************************************************************************************************************/
#endregion Revesion History

#region Namespaces

using com.documentum.fc.client;
using DocumentumService.DataContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

#endregion

namespace DocumentumService.Utility
{
    /// <summary>
    /// AUTHOR: Ashok Kolwal
    /// COMPANY: VITRANA
    /// Version: 1.0
    /// Description: This class is used for docbrocker service for manage all 
    /// the content related work.
    /// Last Modified date: 11 Jul,2017
    /// </summary>
    public class DocBrockerServiceUtility
    {
        #region Variable & Consts
        // Read the config parameters.
        static string documentumServerIP = ConfigurationManager.AppSettings["DocumentumServer"].ToString();
        static string documentumURL = ConfigurationManager.AppSettings["DocumentumURL"].ToString();
        static string documentumServerUser = ConfigurationManager.AppSettings["UserName"].ToString();
        static string documentumServerPassword = ConfigurationManager.AppSettings["Password"].ToString();
        static string cabinetName = ConfigurationManager.AppSettings["CabinetName"].ToString();
        static string reposioryName = ConfigurationManager.AppSettings["RepositoryName"].ToString();
        static string folderName = ConfigurationManager.AppSettings["FolderName"].ToString();
        static string folderPath = string.Concat("/", cabinetName, "/", folderName);
        // Get the instance of DocumentumApi class this is singletone class
        // only single object of this class will comunicate over the network.               
        DocumentumApi docApi = DocumentumApi.GetInstance();
        GetSessionEntity sessionEntityObject = null;

        #endregion

        #region Constructor 

        public DocBrockerServiceUtility()
        {
            // Create the object of session entity class.
            sessionEntityObject = new GetSessionEntity();
            sessionEntityObject.Domain = documentumServerIP;
            sessionEntityObject.UserName = documentumServerUser;
            sessionEntityObject.Password = documentumServerPassword;
            sessionEntityObject.RepositoryName = reposioryName;
            sessionEntityObject.SessionVariableType = IDFSessionVariableType.IDfSessionManager;

        }

        #endregion 

        #region Public Methods 

        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: This method is used for docbrocker service for upload
        /// the document in document using dfc client 6.7
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        public List<UploadDocumentEntity> UploadDoc(string docInputFolderPath)
        {
            List<UploadDocumentEntity> docList = null;
            try
            {


                if (!string.IsNullOrEmpty(documentumServerUser))
                {
                    if (!string.IsNullOrEmpty(documentumServerPassword))
                    {
                        if (!string.IsNullOrEmpty(reposioryName))
                        {
                            if (!string.IsNullOrEmpty(folderName))
                            {
                                if (!string.IsNullOrEmpty(folderName))
                                {
                                    if (System.IO.Directory.Exists(docInputFolderPath))
                                    {
                                        string[] fileList = System.IO.Directory.GetFiles(docInputFolderPath);
                                        if (fileList != null && fileList.Length > 0)
                                        {
                                            // Get the instance of documentum SessionManager class.           
                                            IDfSessionManager idfSessionMgr = (IDfSessionManager)docApi.GetSessionManager(sessionEntityObject);
                                            docList = docApi.MakeDocument(idfSessionMgr, reposioryName, folderPath, docInputFolderPath);
                                        }
                                        else
                                        {
                                            throw new Exception("docInputFolderPath folder does not have files for upload in documentum");
                                        }

                                    }
                                    else
                                    {
                                        throw new Exception("docInputFolderPath does not exists");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Please provide the cabinetName in documentumService Config file");
                                }
                            }
                            else
                            {
                                throw new Exception("Please provide the folderName in documentumService Config file");
                            }

                        }
                        else
                        {
                            throw new Exception("Please provide the RepositoryName in documentumService Config file");
                        }
                    }
                    else
                    {
                        throw new Exception("Please provide the password in documentumService Config file");
                    }
                }
                else
                {
                    throw new Exception("Please provide the userName in documentumService Config file");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return docList;
        }

        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: This method is used for download the document based on idfID.
        /// the document in document using dfc client 6.7
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        public bool DownloadDoc(string idfID, string docOutputFolderPath)
        {
            bool result = false;
            try
            {

                if (!string.IsNullOrEmpty(docOutputFolderPath) && !string.IsNullOrEmpty(idfID))
                {
                    if (System.IO.Directory.Exists(docOutputFolderPath))
                    {
                        IDfSessionManager idfSessionMgr = (IDfSessionManager)docApi.GetSessionManager(sessionEntityObject);
                        result = docApi.GetContent(idfSessionMgr, reposioryName, idfID, docOutputFolderPath);
                    }
                    else
                    {
                        throw new Exception("docOutputFolderPath does not exists");
                    }
                }
                else
                {
                    throw new Exception("docOutputFolderPath and idfID can not be empty");
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: This method is used for drop the document based on idfID.
        /// the document in document using dfc client 6.7
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        public bool DropObject(string idfID)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(idfID))
                {
                    IDfSessionManager idfSessionMgr = (IDfSessionManager)docApi.GetSessionManager(sessionEntityObject);
                    result = docApi.DestroyObject(idfSessionMgr, reposioryName, idfID);
                }
                else
                {
                    throw new Exception("idfID can not be empty");
                }

            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }

            return result;
        }

        #endregion 
    }
}
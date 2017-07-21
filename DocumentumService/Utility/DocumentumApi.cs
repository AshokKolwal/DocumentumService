
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
using System.Web;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using com.documentum.com;
using com.documentum.fc.client;
using com.documentum.fc.common;
using com.documentum.operations;
using ikvm.runtime;
using DocumentumService.DataContracts;
using System.Configuration;
using com.documentum.fc.commands.admin;

#endregion

namespace DocumentumService.Utility
{

    /// <summary>
    /// AUTHOR: Ashok Kolwal
    /// COMPANY: VITRANA
    /// Version: 1.0
    /// Description: This is singleton class only a single object will comunicate over the network.
    /// Last Modified date: 11 Jul,2017
    /// </summary>
    public class DocumentumApi
    {
        #region Variable & Consts

        private static DocumentumApi instance = null;
        private static Object objSync = new object();
        private const String DM_CABINET = "dm_cabinet";
        private const String DM_FOLDER = "dm_folder";
        private const String DM_DOCUMENT = "dm_document";

        #endregion

        #region Constructor

        private DocumentumApi()
        {

        }

        #endregion

        #region Api Methods
        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: Get the DocumentumApi class object
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <returns>DocumentumApi object</returns>
        public static DocumentumApi GetInstance()
        {
            if (instance == null)
            {
                lock (objSync)
                {
                    if (instance == null)
                    {
                        instance = new DocumentumApi();
                    }
                }
            }


            return instance;
        }


        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: This method is used for get the session variable based on documentum server info.
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="sessionVariableObject"></param>
        /// <returns></returns>
        public object GetSessionManager(GetSessionEntity sessionVariableObject)
        {

            object sessionVariable = null;
            try
            {

                IDfClientX clientX = new DfClientX();
                // Console.WriteLine("[GetSessionManager] IDfClientX instance created");
                IDfClient client = clientX.getLocalClient();
                // Console.WriteLine("[GetSessionManager] IDfClient instance created");
                IDfSessionManager sMgr = client.newSessionManager();
                // Console.WriteLine("[GetSessionManager] IDfSessionManager instance created");
                IDfLoginInfo loginInfo = clientX.getLoginInfo();
                // Console.WriteLine("[GetSessionManager] IDfLoginInfo instance created");
                loginInfo.setUser(sessionVariableObject.UserName);
                loginInfo.setPassword(sessionVariableObject.Password);
                loginInfo.setDomain("");
                sMgr.setIdentity(sessionVariableObject.RepositoryName, loginInfo);
                // Console.WriteLine("[GetSessionManager] LoginInfo identity is set");
                IDfSession session = sMgr.getSession(sessionVariableObject.RepositoryName);
                // Console.WriteLine("[GetSessionManager] IDfSession instance created");
                switch (sessionVariableObject.SessionVariableType)
                {
                    case IDFSessionVariableType.IDfSession:
                        sessionVariable = session;
                        break;
                    case IDFSessionVariableType.IDfSessionManager:
                        sessionVariable = sMgr;
                        break;
                };

            }
            catch (Exception ex)
            {
                throw new Exception("Error in [GetSessionManager] ", ex);
            }

            return sessionVariable;
        }

        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description:  A cabinet is a top level container object used to hold folders. 
        ///  The cabinet is, in fact, a type of folder, and is created using the interface IDfFolder.
        ///  Setting its object type to“dm_cabinet”gives it additional features, including the ability to exist as a top-level object. 
        ///  Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="repositoryName"></param>
        /// <param name="cabinetName"></param>
        /// <returns></returns>
        public Boolean MakeCabinet(IDfSessionManager sessionManager, String repositoryName, String cabinetName)
        {
            IDfSession mySession = null;
            try
            {
                mySession = sessionManager.getSession(repositoryName);
                // check to see if the cabinet already exists
                IDfFolder myCabinet = mySession.getFolderByPath("/" + cabinetName);
                if (myCabinet == null)
                {
                    IDfSysObject newCabinet = (IDfFolder)mySession.newObject(DM_CABINET);
                    newCabinet.setObjectName(cabinetName);
                    newCabinet.save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while creating cabinet: " + ex.Message + "\n StackTrace: " + ex.StackTrace);
                return false;
            }
            finally
            {
                sessionManager.release(mySession);
            }

        }


        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: Creating a folder is similar to creating a cabinet. The essential differences are that you will 
        /// create a dm_folder object and identify the parent cabinet or folder in which you’llcreate it
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="repositoryName"></param>
        /// <param name="folderName"></param>
        /// <param name="parentName"></param>
        /// <returns></returns>
        public Boolean MakeFolder(IDfSessionManager sessionManager, String repositoryName, String folderName, String parentName)
        {
            IDfSession mySession = null;
            try
            {
                mySession = sessionManager.getSession(repositoryName);
                IDfSysObject newFolder = (IDfFolder)mySession.newObject(DM_FOLDER);
                IDfFolder aFolder = mySession.getFolderByPath(parentName + "/" + folderName);
                if (aFolder == null)
                {
                    newFolder.setObjectName(folderName);
                    newFolder.link(parentName);
                    newFolder.save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error while creating Folder: " + ex.Message + "\n StackTrace: " + ex.StackTrace);
                return false;
            }
            finally
            {
                sessionManager.release(mySession);
            }
        }


        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: A document object represents both the content of a document and the metadata that describe the document.
        /// In most cases, you create a document by importing an existing document from a local source to the repository
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="sessionManager">The session manager object</param>
        /// <param name="repositoryName">The repository name </param>
        /// <param name="documentName">The document name of document which will visible on documentum server</param>
        /// <param name="documentType">Document extension</param>
        /// <param name="sourcePath">Input file local machine path e.g D:\test1.txt</param>
        /// <param name="folderName">/CabinetName/FolderName e.g /dmadmin/TransformPV</param>
        /// <returns>IDFId</returns>       
        public List<UploadDocumentEntity> MakeDocument(IDfSessionManager sessionManager, String repositoryName, String folderPath, String documentInputFolderPath)
        {

            List<UploadDocumentEntity> docList = new List<UploadDocumentEntity>();
            try
            {
                // Upload files form local drive folder.
                if (Directory.Exists(documentInputFolderPath))
                {
                    string[] documentList = Directory.GetFiles(documentInputFolderPath);
                    if (documentList != null && documentList.Length > 0)
                    {

                        IDfSession mySession = sessionManager.getSession(repositoryName);
                        // Console.WriteLine("[MakeDocument]  IDfSession instance is created");
                        try
                        {
                            string idfID = null;
                            string documentFullName = null;
                            string documentName = null;
                            string docExtension = null;
                            string documentType = null;

                            for (int i = 0; i < documentList.Length; i++)
                            {
                                try
                                {
                                    // Get file individual file for Input document directory.
                                    documentFullName = documentList[i];
                                    documentName = Path.GetFileNameWithoutExtension(documentFullName);
                                    docExtension = Path.GetExtension(documentFullName);
                                    documentType = GetFileMetaData(documentFullName)[0];
                                    FileInfo fileInfo = new FileInfo(documentFullName);
                                    IDfSysObject newDoc = (IDfSysObject)mySession.newObject(DM_DOCUMENT);                                                                        
                                    newDoc.setObjectName(documentName);
                                    newDoc.setContentType(documentType);
                                    newDoc.setFile(documentFullName);                                   
                                    //-- /CabinetName/FolderName
                                    newDoc.link(folderPath);                                    
                                    newDoc.save();                                   
                                    idfID = newDoc.getObjectId().getId();
                                   
                                    ////////////////////////////////////////////////
                                   // IDfApplyExecSQL execSQL = DfAdminCommand.getCommand(__Fields.APPLY_EXEC_SQL);
                                   // IDfApplyExecSQL execSQL =(IDfApplyExecSQL) DfAdminCommand.getCommand(109);
                                   // execSQL.setQuery("DM_DOCUMENT where r_object_id='"+idfID+"'");
                                   // IDfCollection sqlResult= execSQL.execute(mySession);
                                   // Type t= sqlResult.GetType();
                                    ///////////////////////////////////////////////////

                                    // Set out param value.
                                    UploadDocumentEntity uploadDocObj = new UploadDocumentEntity();
                                    uploadDocObj.IdfID = idfID;
                                    uploadDocObj.IdfPath = newDoc.getPath(0);
                                    uploadDocObj.DocumentName = string.Concat(documentName, docExtension);
                                    uploadDocObj.Status = 1;
                                    uploadDocObj.ExceptionMessage = null;
                                    docList.Add(uploadDocObj);                                   
                                  
                                   
                                }
                                catch (Exception ex)
                                {
                                    UploadDocumentEntity uploadDocObj = new UploadDocumentEntity();
                                    uploadDocObj.IdfID = idfID;
                                    uploadDocObj.DocumentName = string.Concat(documentName, docExtension);
                                    uploadDocObj.Status = 0;
                                    uploadDocObj.ExceptionMessage = ServiceBase.GetExceptionMessage(ex);
                                    docList.Add(uploadDocObj);          
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("[MakeDocument] Error while get sessionManager.getSession() :" + ex.Message, ex);
                        }
                        finally
                        {
                            sessionManager.release(mySession);
                        }

                    }
                    else
                    {
                        throw new Exception("[MakeDocument] InputDocumentPath folder does not have files for upload in documentum");
                    }
                }
                else
                {
                    throw new Exception("[MakeDocument] InputDocumentPath does not exists...");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[MakeDocument] GeneralException: " + ex.Message);
            }

            return docList;
        }


        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: This method is used for get the source document(file) metadata
        /// like format and mimetype based on file extension.
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static List<string> GetFileMetaData(string fileExtension)
        {
            List<string> docInfo = new List<string>();
            string mimeType = string.Empty;
            string format = string.Empty;
            try
            {
                switch (Path.GetExtension(fileExtension).Trim().ToLower())
                {
                    case ".xml":
                        mimeType = "application/xml";
                        format = "xml";
                        break;
                    case ".txt":
                        mimeType = "text/plain";
                        format = "crtext";
                        break;
                    case ".pdf":
                        mimeType = "application/pdf";
                        format = "pdf";
                        break;
                    case ".doc":
                    case ".wpd":
                        mimeType = "application/msword";
                        format = "msw8";
                        break;
                    case ".docx":
                        mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        format = "msw8";
                        break;
                    case ".xls":
                    case ".xlsx":
                        mimeType = "application/vnd.ms-excel";
                        format = "excel8book";
                        break;
                    case ".dicom":
                        mimeType = "application/dicom";
                        format = "crtext";
                        break;
                    case ".csv":
                        mimeType = " text/csv";
                        format = "crtext";
                        break;
                    case ".rtf":
                        mimeType = " text/rtf";
                        format = "crtext";
                        break;
                    case ".html":
                    case ".htm":
                        mimeType = "text/html";
                        format = "html";
                        break;
                    case ".ppt":
                    case ".pptx":
                        mimeType = "application/x-mspowerpoint";
                        format = "ppt8";
                        break;
                    case ".tif":
                    case ".tiff":
                        mimeType = " image/tiff";
                        format = "image";
                        break;
                    case ".gif":
                        mimeType = "image/gif";
                        format = "gif";
                        break;
                    case ".png":
                        mimeType = "image/png";
                        format = "png";
                        break;
                    case ".jpeg":
                    case ".jpg":
                        mimeType = "image/jpeg";
                        format = "jpeg";
                        break;
                    case ".bmp":
                        mimeType = "image/bmp";
                        format = "bmp";
                        break;
                    default:
                        mimeType = "text/plain";
                        format = "crtext";
                        break;

                };
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                throw ex;
            }

            docInfo.Add(format);
            docInfo.Add(mimeType);
            return docInfo;
        }

        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: The IDfSysObject.getContent() command lets you get the contents of a document as a ByteArrayInputStream
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="repositoryName"></param>
        /// <param name="objectIdString"></param>
        /// <returns></returns>
        public bool GetContent(IDfSessionManager sessionManager, String repositoryName, String objectIdString, String outputFolderPath)
        {
            IDfSession mySession = null;
            bool result = false;
            try
            {
                mySession = sessionManager.getSession(repositoryName);
                // Get the object ID based on the object ID string. 
                IDfId idObj = mySession.getIdByQualification("dm_sysobject where r_object_id='" + objectIdString + "'");
                // Instantiate an object from the ID. 
                IDfSysObject sysObj = (IDfSysObject)mySession.getObject(idObj);
                // This is file output path plus file name.
                string fileFullPath = string.Concat(outputFolderPath, "\\", sysObj.getObjectName(), ".", sysObj.getContentType());
                java.io.ByteArrayInputStream inputByteStrm = sysObj.getContent();
                java.io.InputStream inputStrm = inputByteStrm;
                java.io.OutputStream outputStrm = new java.io.FileOutputStream(fileFullPath);
                // Transfer bytes from in to out
                byte[] byteArry = new byte[30720];
                int len = 0;
                while ((len = inputStrm.read(byteArry)) > 0)
                {
                    outputStrm.write(byteArry, 0, len);
                }
                inputStrm.close();
                outputStrm.close();
                result = true;
               // Console.WriteLine("Document has been exported with Name: " + Path.GetFileName(fileFullPath));
            }
            // Handle any exceptions. 
            catch (Exception ex)
            {
               // Console.WriteLine(ex.Message);
                throw new Exception("[GetContent] Error: " + ex.Message, ex);                
            }
            // Always, always, release the session in the "finally" clause. 
            finally
            {
                sessionManager.release(mySession);
            }

            return result;
        }


        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: You can use the IDfSystemObject.destroyAllVersions() method to permanently remove an object from the database.
        /// If you use the IDfPersistentObject.destroy() method, you will destroy only the specific system object 
        /// corresponding to the r_object_id your provide. In this example, we use the destroyAllVersions()method,
        /// which destroys not only the system object with the correspondingID but all iterations of the object.
        /// If you attempt to destroy a directory that has children,the method will return an error.
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="repositoryName"></param>
        /// <param name="objectIdString"></param>
        /// <returns></returns>
        public Boolean DestroyObject(IDfSessionManager sessionManager, String repositoryName, String objectIdString)
        {
            IDfSession mySession = null;
            try
            {
                mySession = sessionManager.getSession(repositoryName);
                IDfId idObj = mySession.getIdByQualification("dm_sysobject where r_object_id='" + objectIdString + "'");
                IDfSysObject sysObj = (IDfSysObject)mySession.getObject(idObj);
                sysObj.destroyAllVersions();
               // Console.WriteLine("Object has been deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while Deleting object : " + ex.Message + "\n StackTrace: " + ex.StackTrace);
                return false;
            }
            finally
            {
                sessionManager.release(mySession);
            }
        }

        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: The execute method of an IDfDeleteOperation object removes documents and folders from the repository.
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="repositoryName"></param>
        /// <param name="docId"></param>
        /// <param name="versionNo"></param>
        /// <returns></returns>
        public String DeleteDocument(IDfSessionManager sessionManager, String repositoryName, String docId, int versionNo)
        {

            IDfSession mySession = null;
            try
            {
                mySession = sessionManager.getSession(repositoryName);
                // Get the object ID based on the object ID string.
                IDfId idObj = mySession.getIdByQualification("dm_sysobject where r_object_id='" + docId + "'");
                // Instantiate an object from the ID. 
                IDfSysObject sysObj = (IDfSysObject)mySession.getObject(idObj);
                // Create a new client instance.
                IDfClientX clientx = new DfClientX();
                // Use the factory method to create an IDfDeleteOperation instance.
                IDfDeleteOperation delo = clientx.getDeleteOperation();
                // Create a document object that represents the document being copied. 
                //IDfDocument doc = (IDfDocument)mySession.getObject(new DfId(docId));
                IDfDocument doc = (IDfDocument)mySession.getObject(idObj);
                // Set the deletion policy. You must do this prior to adding nodes to the 
                // Delete operation. 
                delo.setVersionDeletionPolicy(versionNo);
                // Create a delete node using the factory method. 
                IDfDeleteNode node = (IDfDeleteNode)delo.add(doc);
                if (node == null)
                    return "Node is null.";
                // Execute the delete operation and return results.
                if (delo.execute())
                {
                    return "Delete operation succeeded.";
                }
                else
                {
                    return "Delete operation failed";
                }
            }
            // Handle any exceptions. 
            catch (Exception ex)
            {
                Console.WriteLine("Error while Deleting document : " + ex.Message + "\n StackTrace: " + ex.StackTrace);
                return "Exception has been thrown: " + ex;
            }
            // Always, always, release the session in the "finally" clause. 
            finally
            {
                sessionManager.release(mySession);
            }
        }

        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: The execute method of an IDfCopyOperation object copies the current versions of documents
        /// or folders from one repository location to another.
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="repositoryName"></param>
        /// <param name="docId"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public String CopyDocument(IDfSessionManager sessionManager, String repositoryName, String docId, String destination)
        {

            IDfSession mySession = null;
            StringBuilder sb = new StringBuilder("");
            try
            {
                mySession = sessionManager.getSession(repositoryName);
                // Get the object ID based on the object ID string.
                IDfId idObj = mySession.getIdByQualification("dm_sysobject where r_object_id='" + docId + "'");
                // Instantiate an object from the ID. 
                IDfSysObject sysObj = (IDfSysObject)mySession.getObject(idObj);
                // Create a new client instance. 
                IDfClientX clientx = new DfClientX();
                // Use the factory method to create an IDfCopyOperation instance. 
                IDfCopyOperation co = clientx.getCopyOperation();
                // Remove the path separator if it exists. 
                if (destination.LastIndexOf("/") == destination.Length - 1 || destination.LastIndexOf("\\") == destination.Length - 1)
                {
                    destination = destination.Substring(0, destination.Length - 1);
                }
                // Create an instance for the destination directory.
                IDfFolder destinationDirectory = mySession.getFolderByPath(destination);
                // Set the destination directory by ID.
                co.setDestinationFolderId(destinationDirectory.getObjectId());
                // Create a document object that represents the document being copied.
                //IDfDocument doc = (IDfDocument)mySession.getObject(new DfId(docId));
                IDfDocument doc = (IDfDocument)mySession.getObject(idObj);
                // Create a copy node, adding the document to the copy operation object.
                IDfCopyNode node = (IDfCopyNode)co.add(doc);
                // Execute and return results 
                if (co.execute())
                {
                    return "Copy operation successful.";
                }
                else
                {
                    return "Copy operation failed.";
                }
            }
            // Handle any exceptions. 
            catch (Exception ex)
            {
                Console.WriteLine("Error while copying object : " + ex.Message + "\n StackTrace: " + ex.StackTrace);
                return "Exception has been thrown: " + ex;
            }
            // Always, always, release the session in the "finally" clause. 
            finally
            {
                sessionManager.release(mySession);
            }
        }


        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: The execute method of an IDfMoveOperation object moves the current versions of documents or folders from one 
        /// repository location to an other by unlinking them from the source location and linking them to the destination.
        /// Versions other than the current version remain linked to the original location.
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="mySession"></param>
        /// <param name="docId"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public String MoveObject(IDfSession mySession, String docId, String destination)
        {
            try
            {
                IDfId idObj = mySession.getIdByQualification("dm_sysobject where r_object_id='" + docId + "'");
                // Create a new client instance. 
                IDfClientX clientx = new DfClientX();
                // Use the factory method to create an IDfCopyOperation instance. 
                IDfMoveOperation mo = clientx.getMoveOperation();
                // Create an instance for the destination directory. 
                IDfFolder destinationDirectory = mySession.getFolderByPath(destination);
                // Set the destination directory by ID.
                mo.setDestinationFolderId(destinationDirectory.getObjectId());
                // Create a document object that represents the document being copied. 
                IDfDocument doc = (IDfDocument)mySession.getObject(idObj);
                // Create a move node, adding the document to the move operation object.
                IDfMoveNode node = (IDfMoveNode)mo.add(doc);
                // Execute and return results 
                if (mo.execute())
                {
                    return "Move operation successful.";
                }
                else
                {
                    return "Move operation failed.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while moving object : " + ex.Message + "\n StackTrace: " + ex.StackTrace);
                return null;
            }


        }

        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: The execute method of an IDfCheckoutOperation object checks out the documents in the operation.
        /// The checkout operation: 
        /// • Locks the document 
        /// • Copies the document to your local disk 
        /// • Always creates registry entries to enable DFC to manage the files it creates on the file system
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="repositoryName"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public String Checkout(IDfSessionManager sessionManager, String repositoryName, String docId)
        {
            StringBuilder result = new StringBuilder("");
            IDfSession mySession = null;
            try
            {
                mySession = sessionManager.getSession(repositoryName);
                // Get the object ID based on the object ID string. 
                IDfId idObj = mySession.getIdByQualification("dm_sysobject where r_object_id='" + docId + "'");
                // Instantiate an object from the ID. 
                IDfSysObject sysObj = (IDfSysObject)mySession.getObject(idObj);
                // Instantiate a client. 
                IDfClientX clientx = new DfClientX();
                // Use the factory method to create a checkout operation object. 
                IDfCheckoutOperation coOp = clientx.getCheckoutOperation();
                // Set the location where the local copy of the checked out file 
                // is stored. 
                coOp.setDestinationDirectory("C:\\");
                // Get the document instance using the document ID.
                IDfDocument doc = (IDfDocument)mySession.getObject(idObj);
                // Create the checkout node by adding the document to the checkout 
                // operation. 
                IDfCheckoutNode coNode = (IDfCheckoutNode)coOp.add(doc);
                // Verify that the node exists. 
                if (coNode == null)
                {
                    result.Append("coNode is null");
                }
                // Execute the checkout operation. Return the result. 
                if (coOp.execute())
                {
                    result.Append("Successfully checked out file ID: " + docId);
                }
                else
                {
                    result.Append("Checkout failed.");
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while checkout : " + ex.Message + "\n StackTrace: " + ex.StackTrace);
                return "Exception hs been thrown: " + ex;
            }
            finally
            {
                sessionManager.release(mySession);
            }


        }

        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: The execute method of an IDfCheckinOperation object checks documents into the repository.
        /// It creates new objects as required, transfers the content to the repository, 
        /// and removes local files if appropriate. It checks in existing objects that any of the nodes 
        /// refer to(forexample, throughXML links). 
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="repositoryName"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public String Checkin(IDfSessionManager sessionManager, String repositoryName, String docId)
        {
            IDfSession mySession = null;
            try
            {
                mySession = sessionManager.getSession(repositoryName);
                // Get the object ID based on the object ID string.
                IDfId idObj = mySession.getIdByQualification("dm_sysobject where r_object_id='" + docId + "'");
                // Instantiate an object from the ID.
                IDfSysObject sysObj = (IDfSysObject)mySession.getObject(idObj);
                // Instantiate a client. 
                IDfClientX clientx = new DfClientX();
                // Use the factory method to create an IDfCheckinOperation instance. 
                IDfCheckinOperation cio = clientx.getCheckinOperation();
                // Set the version increment. In this case, the next major version 
                // ( version + 1)
                cio.setCheckinVersion(cio.getCheckinVersion() + 1);
                // When updating to the next major version, you need to explicitly 
                // set the version label for the new object to "CURRENT".
                cio.setVersionLabels("CURRENT");
                // Create a document object that represents the document being 
                // checked in. 
                IDfDocument doc = (IDfDocument)mySession.getObject(idObj);
                // Create a checkin node, adding it to the checkin operation. 
                IDfCheckinNode node = (IDfCheckinNode)cio.add(doc);
                // Execute the checkin operation and return the result. 
                if (!cio.execute())
                {
                    return "Checkin failed.";
                }
                // After the item is created, you can get it immediately using the 
                // getNewObjectId method. 
                IDfId newId = node.getNewObjectId();
                return "Checkin succeeded - new object ID is: " + newId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while checkin object : " + ex.Message + "\n StackTrace: " + ex.StackTrace);
                return "Checkin failed.";
            }
            finally
            {
                sessionManager.release(mySession);
            }
        }


        /// <summary>
        /// AUTHOR: Ashok Kolwal
        /// COMPANY: VITRANA
        /// Version: 1.0
        /// Description: The execute method of an IDfCancelCheckoutOperation object cancels the checkout of documents by releasing locks,
        /// deleting local files if appropriate,and removing registry entries. If the operation’s add method receives a virtual
        /// document as an argument, it also adds all of the document’sdescendants(determined by applying the applicable binding rules),
        /// creating a separate operation node for each.
        /// Last Modified date: 11 Jul,2017
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="repositoryName"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public String CancelCheckOut(IDfSessionManager sessionManager, String repositoryName, String docId)
        {
            IDfSession mySession = null;
            try
            {
                mySession = sessionManager.getSession(repositoryName);
                // Get the object ID based on the object ID string. 
                IDfId idObj = mySession.getIdByQualification("dm_sysobject where r_object_id='" + docId + "'");
                // Instantiate an object from the ID. 
                IDfSysObject sysObj = (IDfSysObject)mySession.getObject(idObj);
                // Get a new client instance. 
                IDfClientX clientx = new DfClientX();
                // Use the factory method to create a checkout operation object. 
                IDfCancelCheckoutOperation cco = clientx.getCancelCheckoutOperation();
                // Instantiate the document object from the ID string. 
                IDfDocument doc = (IDfDocument)mySession.getObject(idObj);
                // Indicate whether to keep the local file. 
                cco.setKeepLocalFile(true);
                // Create an empty cancel checkout node. 
                IDfCancelCheckoutNode node;
                // Populate the cancel checkout node and add it to the cancel checkout 
                // operation. 
                node = (IDfCancelCheckoutNode)cco.add(doc);
                // Check to see if the node is null - this will not throw an error. 
                if (node == null)
                {
                    return "Node is null";
                }
                // Execute the operation and return the result. 
                if (!cco.execute())
                {
                    return "Operation failed";
                }
                return "Successfully cancelled checkout of file ID: " + docId;
            }
            // Handle any exceptions. 
            catch (Exception ex)
            {
                Console.WriteLine("Error while cancelCheckOut object : " + ex.Message + "\n StackTrace: " + ex.StackTrace);
                return "Exception has been thrown: " + ex;
            }
            // Always, always, release the session in the "finally" clause. 
            finally
            {
                sessionManager.release(mySession);
            }
        }



        #endregion
    }

}

/// <summary>
/// AUTHOR: Ashok Kolwal
/// COMPANY: VITRANA
/// Version: 1.0
/// Description: This namespace is used for register or load the class IKVM classes.
/// Last Modified date: 11 Jul,2017
/// </summary>

namespace vitrana.DocumentumModel.Infrastructure.Ikvm
{
    public class SystemClassLoader : java.lang.ClassLoader
    {
        string configFolderPath = ConfigurationManager.AppSettings["DocumentumConfigFolderPath"].ToString();

        public SystemClassLoader(java.lang.ClassLoader parent)
            : base(new AppDomainAssemblyClassLoader(typeof(SystemClassLoader).Assembly))
        {
        }

        override protected java.net.URL findResource(string name)
        {
            if (Directory.Exists(configFolderPath))
            {
                if (name.EndsWith("properties"))
                {
                    java.io.File file = new java.io.File(System.IO.Path.Combine(configFolderPath, name));
                    java.net.URL resourceUrl = file.toURI().toURL();
                    return resourceUrl;

                }
                return base.findResource(name);

            }
            else
            {
                throw new Exception("SystemClassLoader.cs DocumentumConfigFolderPath does not exists");
            }

        }
    }

}
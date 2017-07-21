using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentumService.Utility
{
    public static class ServiceBase
    {
        public static string GetExceptionMessage(Exception ex)
        {
            string errorMessage = string.Empty;
            if (ex != null)
            {
                errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " InnerExceptionMessage: " + GetBaseException(ex.InnerException);
                }
                errorMessage += " StackTrace: " + ex.StackTrace;
            }

            return errorMessage;

        }

        public static Exception GetBaseException(this Exception ex)
        {
            if(ex.InnerException==null)
            {
                return ex;
            }
            else
            {
              return  GetBaseException(ex.InnerException);
            }
        }

    }
}
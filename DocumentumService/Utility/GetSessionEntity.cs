using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentumService.Utility
{
    public class GetSessionEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RepositoryName { get; set; }
        public string Domain { get; set; }
        public IDFSessionVariableType SessionVariableType { get; set; }
    }

    public enum IDFSessionVariableType
    {
        IDfSession = 0,
        IDfSessionManager = 1,
    }
}
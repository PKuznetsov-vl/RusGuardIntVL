using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WcfService1.App_Code
{
    public class Initialize
    {
       
        public static string AppInitializeConn()
        {
            string v1 = ConfigurationManager.AppSettings["ConnStr"];      
            return v1;
        }
        public static string AppInitializeLogin()
        {
            string v1 = ConfigurationManager.AppSettings["Login"];
            return v1;
        }
        public static string AppInitializePass()
        {
            string v1 = ConfigurationManager.AppSettings["Pass"];
            return v1;
        }

    }
}
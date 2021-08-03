using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WcfService1.App_Code
{
    public class Initialize
    {
       
        public static string AppInitialize()
        {
            string v1 = ConfigurationManager.AppSettings["ConnStr"];
            return v1;
        }

    }
}
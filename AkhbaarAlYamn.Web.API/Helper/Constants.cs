﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AkhbaarAlYamn.Web.API.Helper
{
    public class Constants
    {
        public static string Akhbaar_CP_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["Akhbaar_CP_URL"];
            }
        }
    }
}
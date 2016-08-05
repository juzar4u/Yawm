using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotunity.DataAccess.helpers
{
    public static class DataContextHelper
    {

        public static PetaPoco.Database GetBackgroundProcessDataContext()
        {

            return (new PetaPoco.Database("BGSConnectionString"));
            //return GetWebRequestScopedDataContext<ArgaamDataModelDataContext>("Portal", ConfigurationManager.ConnectionStrings["PPConnectionString"].ConnectionString);
        }

        public static PetaPoco.Database GetPPDataContext()
        {
            /* Uncomment to Activate MiniProfiling of Sql Queries; Also Uncomment Code lines 217 & 218 in PetaPoco.cs */
            //SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["PPConnectionString"].ConnectionString);
            //cnn.Open();
            //PetaPoco.Database pd = new PetaPoco.Database(new MvcMiniProfiler.Data.ProfiledDbConnection(cnn, MvcMiniProfiler.MiniProfiler.Current));
            //return (pd);
            /* END */

            return (new PetaPoco.Database("PPDefaultConnection"));

            //return GetWebRequestScopedDataContext<ArgaamDataModelDataContext>("Portal", ConfigurationManager.ConnectionStrings["PPConnectionString"].ConnectionString);
        }

        public static PetaPoco.Database GetCPDataContext()
        {
            return (new PetaPoco.Database("CPDefaultConnection"));
        }
    }
}


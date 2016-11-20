using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarBGSLIb.EmailSender
{
    public class EmailSyncController
    {
        protected DateTime executionStartDateTime = DateTime.MinValue;

        public int GetLabelSyncIntervalInSeconds
        {
            get
            {
                try
                {
                    return int.Parse(System.Configuration.ConfigurationManager.AppSettings["EmailSyncIntervalInSeconds"]);
                }
                catch (Exception ex)
                {
                    return 60; //60 seconds
                }
            }
        }

        public void ExecuteEmailSync()
        {
            try
            {
                (new AkhbaarEmailSyncController()).ExecuteEmailSync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkhbaarBGSLIb.Helpers;
namespace AkhbaarBGSLIb
{
    public class AkhbaarEmailSyncController
    {
        static AkhbaarEmailSyncController _instance = null;

        public static AkhbaarEmailSyncController Instance
        {
            get
            {
                if (_instance == null) { _instance = new AkhbaarEmailSyncController(); }
                return _instance;
            }
        }
        
        public void ExecuteEmailSync()
        {
            try
            {
                Helpers.EmailSender es = new Helpers.EmailSender();
                es.SendSimpleEmails();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}

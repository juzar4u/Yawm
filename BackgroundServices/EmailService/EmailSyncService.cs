using AkhbaarBGSLIb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public partial class EmailSyncService : ServiceBase
    {
        readonly System.Threading.Thread emailSyncThread = new System.Threading.Thread(EmailSyncThread.Execute);

       
        protected override void OnStart(string[] args)
        {
            emailSyncThread.Start();
        }

        protected override void OnStop()
        {
            try
            {
                emailSyncThread.Abort();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

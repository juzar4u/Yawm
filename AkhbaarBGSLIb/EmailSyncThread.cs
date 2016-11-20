using AkhbaarBGSLIb.EmailSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AkhbaarBGSLIb
{
    public class EmailSyncThread
    {
        public static void Execute()
        {
            int normalSleepTime = 0;

            try
            {
                normalSleepTime = int.Parse(System.Configuration.ConfigurationManager.AppSettings["EmailSyncIntervalInSeconds"]);
            }
            catch (Exception ex)
            {
                
                throw;
            }

            normalSleepTime *= 1000; // Convert to mili seconds

            do
            {
                
                try
                {
                    (new EmailSyncController()).ExecuteEmailSync();
                }
                catch (Exception ex)
                {
                      throw;
                }
                // Normal sleep cycle
                Thread.Sleep(normalSleepTime);

            } while (true);

        }
    }
}


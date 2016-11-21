using AkhbaarBGSLIb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarBGS
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                EmailSender es = new EmailSender();
                es.SendSimpleEmails();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

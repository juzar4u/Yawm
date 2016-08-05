using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using AkhbaarAlYawm.DataAccess.Custom.Entities.CommonModel;
using Spotunity.DataAccess.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AkhbaarAlYawm.Application.Services
{
    public class CommonServices
    {
        private static CommonServices _instace;

        public static CommonServices GetInstance
        {
            get
            {
                if (_instace == null)
                {
                    _instace = new CommonServices();
                }

                return _instace;
            }
        }

        public int UpdateCurrentQiyaam(CurrentQiyaam EM)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(EM);
            }
        }

        public int UpdateEmailToken(EmailTokens token)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(token);
            }
        }

        public CurrentQiyaam GetCurrentQiyaamDetailsByCurrentQiyaamID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<CurrentQiyaam>("select * from CurrentQiyaam where CurrentQiyaamID = @0", id).FirstOrDefault();
            }
        }
        public CurrentQiyaamModel GetCurrentQiyaamModelByCurrentQiyaamID(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<CurrentQiyaamModel>("select * from CurrentQiyaam where CurrentQiyaamID = @0", id).FirstOrDefault();
            }
        }

        public string GetCurrentQiyaamMauze(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<string>("select CurrentQiyaamMauze from CurrentQiyaam where CurrentQiyaamID = @0", id).FirstOrDefault();
            }
        }

        public List<EmailTokens> GetListOfEmailTokens()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<EmailTokens>("select * from emailtokens where status <> 'S'");

            }
        }



    }
}

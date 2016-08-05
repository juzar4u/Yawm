using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.DataAccess.Custom.Entities.CommonModel
{
    public class CurrentQiyaamModel
    {
        public int CurrentQiyaamID { get; set; }
        public string CurrentQiyaamMauze { get; set; }

    }

    public class CreativeModel
    {
        public string Url { get; set; }
        public string Thumbnail { get; set; }
        public string creativeType { get; set; }
        public string ErrorMessage { get; set; }
    }
}

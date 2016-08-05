using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.DataAccess.Custom.Entities
{
    
        public class CategoryModel
        {
            public int CategoryID { get; set; }
            public int? ParentCategoryID { get; set; }
            public string CategoryNameEn { get; set; }
            public string DescriptionEn { get; set; }
            public int IsDeleted { get; set; }
            public DateTime CreatedOn { get; set; }
            public DateTime CreatedBy { get; set; }
            public DateTime ModifiedOn { get; set; }
            public int ModifiedBy { get; set; }

            public List<Categories> ParentListCategories { get; set; }
            public List<Categories> ListCategories { get; set; }
            public List<Categories> LowerListCategories { get; set; }
            public List<Categories> ChildListCategories { get; set; }
            public Categories CategoryDetails { get; set; }
            
        }
}

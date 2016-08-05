using AkhbaarAlYawm.DataAccess;
using Spotunity.DataAccess.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.Application.Services
{
    public class CategoriesServices
    {
        private static CategoriesServices _instace;

        public static CategoriesServices GetInstance
        {
            get
            {
                if (_instace == null)
                {
                    _instace = new CategoriesServices();
                }

                return _instace;
            }
        }

        public int UpdateCategory(Categories _category)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(_category);
            }
        }

        public int DeleteCategory(Categories _category)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Delete(_category);
            }
        }

        public Categories GetCategoryByCategoryID(int CategoryID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Categories>("select * from Categories where CategoryID=@0", CategoryID).FirstOrDefault();
            }
        }
        public List<Categories> GetAllParentCategories()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Categories>("select * from Categories where IsDeleted = 0 and ParentCategoryID IS NULL");
            }
        }

        public List<Categories> GetFilterCategoriesByName(string name)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Categories>(String.Format("select * from Categories where CategoryNameEn like '{0}%'", name));
            }
        }
        public List<Categories> GetAllCategories()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Categories>("select * from categories where isdeleted = 0");
            }
        }
        public List<Categories> GetAllChildCategories()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Categories>("select * from Categories where IsDeleted = 0 and ParentCategoryID IS NOT NULL");
            }
        }

        public List<Categories> GetChildCategoryByName(string name)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Categories>("select * from Categories where ParentCategoryID = (select  CategoryID from Categories where CategoryNameEn = @0)", name);
            }
        }
        public List<Categories> GetParentCategory(int id)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Categories>("select * from Categories where IsDeleted = 0 and CategoryID = @0", id);
            }
        }
        public void InsertCategory(Categories category)
        {

            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                context.Insert(new Categories()
                {
                    ParentCategoryID = category.ParentCategoryID,
                    CategoryNameEn = category.CategoryNameEn,
                    DescriptionEn = category.DescriptionEn,
                    IsDeleted = category.IsDeleted,
                    CreatedOn = category.CreatedOn,
                    CreatedBy = category.CreatedBy,
                    ModifiedBy = category.ModifiedBy,
                    ModifiedOn = category.ModifiedOn
                });
            }
        }

    }
}

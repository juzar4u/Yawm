using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.CommonCode.Helpers;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Controllers
{
    [Authentication]
    public class CategoryController : Controller
    {
        //
        // GET: /Category/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            CategoryModel model = new CategoryModel();
            model.ParentListCategories = CategoriesServices.GetInstance.GetAllParentCategories();
            model.ListCategories = CategoriesServices.GetInstance.GetAllCategories();
            model.LowerListCategories = CategoriesServices.GetInstance.GetAllParentCategories();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CategoryModel model)
        {
            if (model.CategoryNameEn != null)
            {
                Categories category = new Categories();
                category.ParentCategoryID = model.ParentCategoryID;
                category.CategoryNameEn = model.CategoryNameEn;
                category.DescriptionEn = model.DescriptionEn;
                category.IsDeleted = false;
                category.CreatedOn = DateTime.Now;
                category.ModifiedOn = null;
                CategoriesServices.GetInstance.InsertCategory(category);
                return RedirectToAction("Create");
            }
            return Redirect(Request.Url.AbsoluteUri);
        }

        [HttpGet]
        public ActionResult CreateSubCategory()
        {
            CategoryModel model = new CategoryModel();
            model.ParentListCategories = CategoriesServices.GetInstance.GetAllParentCategories();
            model.ChildListCategories = CategoriesServices.GetInstance.GetAllChildCategories();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSubCategory(CategoryModel model)
        {
            if (model.CategoryNameEn != null)
            {
                Categories category = new Categories();
                category.ParentCategoryID = model.ParentCategoryID;
                category.CategoryNameEn = model.CategoryNameEn;
                category.DescriptionEn = model.DescriptionEn;
                category.IsDeleted = false;
                category.CreatedOn = DateTime.Now;

                CategoriesServices.GetInstance.InsertCategory(category);
                return RedirectToAction("CreateSubCategory");
            }
            return Redirect(Request.Url.AbsoluteUri);

        }

        [HttpGet]
        public ActionResult SelectCategory()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EditCategory(int CategoryID)
        {

            CategoryModel model = new CategoryModel();
            model.ListCategories = CategoriesServices.GetInstance.GetAllCategories();
            model.CategoryDetails = CategoriesServices.GetInstance.GetCategoryByCategoryID(CategoryID);

            model.CategoryID = model.CategoryDetails.CategoryID;
            model.CategoryNameEn = model.CategoryDetails.CategoryNameEn;
            model.DescriptionEn = model.CategoryDetails.DescriptionEn;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditCategory(CategoryModel model)
        {
            Categories _category = CategoriesServices.GetInstance.GetCategoryByCategoryID(model.CategoryID);

            _category.CategoryNameEn = model.CategoryNameEn;
            _category.DescriptionEn = model.DescriptionEn;
            _category.ModifiedOn = DateTime.Now;
            CategoriesServices.GetInstance.UpdateCategory(_category);
            if (_category.ParentCategoryID == null)
                return RedirectToAction("Create");
            else
                return RedirectToAction("CreateSubCategory");
        }

        [HttpGet]
        public ActionResult SearchCategory(int id)
        {
            CategoryModel _category = new CategoryModel();
            _category.LowerListCategories = CategoriesServices.GetInstance.GetParentCategory(id);
            _category.ParentListCategories = CategoriesServices.GetInstance.GetAllParentCategories();
            return PartialView("categoryList", _category);
        }
        [HttpGet]
        public ActionResult SearchChildCategoryByName(string name)
        {
            CategoryModel _category = new CategoryModel();
            _category.ParentListCategories = CategoriesServices.GetInstance.GetAllParentCategories();
            _category.ChildListCategories = CategoriesServices.GetInstance.GetChildCategoryByName(name);
            return PartialView("childCategoryList", _category);
        }

        public ActionResult getCategoryItems(string name)
        {
            var categoryList = CategoriesServices.GetInstance.GetFilterCategoriesByName(name);

            var result = (from c in categoryList
                          select new
                          {
                              CategoryID = c.CategoryID,
                              CategoryNameEn = c.CategoryNameEn,
                              ParentCategoryID = c.ParentCategoryID
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DeleteCategory(int CategoryID)
        {
            CategoryModel _category = new CategoryModel();
            _category.CategoryDetails = CategoriesServices.GetInstance.GetCategoryByCategoryID(CategoryID);

            return View(_category);
        }

        [HttpPost]
        public ActionResult DeleteCategory(CategoryModel model)
        {
            Categories _categories = CategoriesServices.GetInstance.GetCategoryByCategoryID(model.CategoryID);

            CategoriesServices.GetInstance.DeleteCategory(_categories);
            if (_categories.ParentCategoryID == null)
                return RedirectToAction("Create");
            else
                return RedirectToAction("CreateSubCategory");
        }


    }
}

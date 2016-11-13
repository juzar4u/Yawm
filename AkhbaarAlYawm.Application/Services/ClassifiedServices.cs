using Akhbaar.Shared.Helper.Enum;
using AkhbaarAlYawm.Application.Helper;
using AkhbaarAlYawm.Application.Helper.CacheManager;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using PetaPoco;
using Spotunity.DataAccess.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.Application.Services
{
    public class ClassifiedServices
    {
        private static ClassifiedServices _instace;

        public static ClassifiedServices GetInstance
        {
            get
            {
                if (_instace == null)
                {
                    _instace = new ClassifiedServices();
                }

                return _instace;
            }
        }

        public int InsertClassified(Classified _classified)
        {
            using(PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Insert(_classified);
            }
        }

        public int DeleteClassified(Classified _classified)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                List<NewsfeedClassifiedMedia> media = context.Fetch<NewsfeedClassifiedMedia>("select * from NewsfeedClassifiedMedia where entityid = @0", _classified.ClassifiedID);
                foreach (var item in media)
                {
                    context.Delete(item);
                }
                return (int)context.Delete(_classified);
            }
        }

        public int UpdateClassified(Classified _classified)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Update(_classified);
            }
        }

        public int InsertNewsfeedClassifiedMedia(NewsfeedClassifiedMedia creativeMedia)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Insert(creativeMedia);
            }
        }

        public int InsertUserLikes(UserLikes likes)
        {
            int likecount = 0;
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                context.Insert(likes);
                likecount = GetUserLikeCount(likes.EntityID, likes.EntityCategoryID);
            }

            return likecount;
        }

        public int UpdateUserLikes(UserLikes likes)
        {
            int likecount = 0;
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
               context.Update(likes);
               likecount = GetUserLikeCount(likes.EntityID, likes.EntityCategoryID);
            
            }
            return likecount;
        }

        public int GetUserLikeCount(int entityId, int entityCategoryId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<int>("select count(*) from userlikes where entityid = @0 and entitycategoryid = @1 and isLike = 1", entityId, entityCategoryId).FirstOrDefault();
            }
        }
        public Page<ClassifiedIndexModel> GetAllClassified(int pageNo, int pageSize, UserModel user)
        {
            List<ClassifiedIndexModel> model = new List<ClassifiedIndexModel>();
            string key = string.Format("GetAllClassified#{0}#{1}", pageNo, pageSize);
            Page<ClassifiedIndexModel> result = CacheManager.Get(key) as Page<ClassifiedIndexModel>;
            if (result == null)
            {
                using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
                {
                    var ppsql = "select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID order by ads.PublishedDate desc";
                    result = context.Page<ClassifiedIndexModel>(pageNo, pageSize, ppsql);
                    foreach (var item in result.Items)
                    {
                        if (user.UserID == item.UserID)
                        {
                            int liked = context.Fetch<int>("select count(*) from userlikes where UserID = @0 and entityid = @1 and entitycategoryid = @2 and isLike = 1", item.UserID, item.ClassifiedID, (int)CommentCategoryEnum.ClassifiedType).FirstOrDefault();
                            item.UserLikedClassified = liked > 0 ? true : false;
                            int likecount = GetUserLikeCount(item.ClassifiedID, (int)CommentCategoryEnum.ClassifiedType);
                            string likeText = likecount > 1 ? "persons like this" : "person like this";
                            item.userlikescount = string.Format("{0}{1}{2}", likecount, " ", likeText);
                            if (likecount < 1)
                            {
                                item.userlikescount = string.Empty;
                            }
                        }
                        item.mediaList = context.Fetch<NewsfeedClassifiedMedia>("select * from NewsfeedClassifiedMedia where EntityTypeID = 1 and EntityID = @0", item.ClassifiedID);
                    }
                    CacheManager.Set(key, result, DateTime.Now.AddSeconds(ApplicationConstants.ListCacheTimeInSec));
                }
            }
            return result;
        }

        public List<ClassifiedAdCategories> GetParentClassifiedCategories()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<ClassifiedAdCategories>("select * from ClassifiedAdCategories where parentCategoryID is Null and isApproved = 1");
            }
        }

        public List<ClassifiedAdCategories> GetChildClassifiedCategories(int parentCategoryID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<ClassifiedAdCategories>("select * from ClassifiedAdCategories where parentCategoryID = @0 and isApproved = 1", parentCategoryID);
            }
        }


        public List<CommentMaster> GetClassifiedCommentsByClassifiedID(int classifiedID, int commentCategoryID)
        {
            List<CommentModel> _comments = new List<CommentModel>();
            List<CommentMaster> classifiedComments = new List<CommentMaster>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _comments = context.Fetch<CommentModel>("select cmt.CommentID, cmt.ArticleID, cmt.ParentCommentID,cmt.IsApproved,cmt.Content, cmt.CreatedOn, cmt.CreatedBy, cmt.UpdatedOn, cmt.UpdatedBy, usr.FirstName as UserName, usr.ThumbnailProfileImg from Comments cmt left join Users usr on usr.UserID = cmt.CreatedBy where cmt.CommentCategoryID = @0 and cmt.ArticleID = @1 and cmt.ParentCommentID is null and cmt.IsApproved = 1", commentCategoryID, classifiedID);

                foreach (var item in _comments)
                {
                    classifiedComments.Add(new CommentMaster()
                    {
                        ParentComment = item,
                        Comments = context.Fetch<CommentModel>("select cmt.CommentID, cmt.ArticleID, cmt.ParentCommentID,cmt.IsApproved,cmt.Content, cmt.CreatedOn, cmt.CreatedBy, cmt.UpdatedOn, cmt.UpdatedBy, usr.FirstName as UserName, usr.ThumbnailProfileImg from Comments cmt left join Users usr on usr.UserID = cmt.CreatedBy where cmt.CommentCategoryID = @0 and cmt.ParentCommentID = @1 and cmt.IsApproved = 1", commentCategoryID, item.CommentID)
                    });
                }
            }
            return classifiedComments;
        }

        public UserLikes GetUserLike(int userId, int entityId, int entityCategoryId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<UserLikes>("select * from userLikes where UserID = @0 and EntityID = @1 and EntityCategoryID = @2", userId, entityId, entityCategoryId).FirstOrDefault();
            }

        }

        public List<ClassifiedIndexModel> GetFilterClassifiedList(FilterClassifiedModel model, int elements)
        {
            List<ClassifiedIndexModel> CompleteClassifiedList = new List<ClassifiedIndexModel>();
            List<ClassifiedIndexModel> ClassifiedList = new List<ClassifiedIndexModel>();
            List<ClassifiedIndexModel> models = new List<ClassifiedIndexModel>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                if (elements == 4)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.CityID = @0 and ads.Price >= @1 and ads.Price <= @2 and ads.ClassifiedAdCategoryID = @3 order by ads.PublishedDate desc", model.CityID, model.MinPrice, model.MaxPrice, model.ChildClassifiedAdCategoryID);
                    ClassifiedList = models;
                }
                else if (elements == 3)
                {
                    ClassifiedList = FilterClassifiedByThreeFilter(model);
                }
                else if (elements == 2)
                {
                    ClassifiedList = FilterClassifiedByTwoFilter(model);
                }
                else
                {
                    ClassifiedList = FilterClassifiedByOneItem(model);
                }
                foreach (var item in ClassifiedList)
                {
                    item.mediaList = context.Fetch<NewsfeedClassifiedMedia>("select * from NewsfeedClassifiedMedia where EntityTypeID = 1 and EntityID = @0", item.ClassifiedID);
                    
                }
            }
            return ClassifiedList;
        }

        private List<ClassifiedIndexModel> FilterClassifiedByThreeFilter(FilterClassifiedModel model)
        {
            List<ClassifiedIndexModel> ClassifiedList = new List<ClassifiedIndexModel>();
            List<ClassifiedIndexModel> models = new List<ClassifiedIndexModel>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                if (model.ChildClassifiedAdCategoryID > 0 && model.MinPrice > 0 && model.MaxPrice > 0 && model.CityID <= 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.Price >= @0 and ads.ClassifiedAdCategoryID = @1 and ads.Price <= @2 order by ads.PublishedDate desc", model.MinPrice, model.ChildClassifiedAdCategoryID, model.MaxPrice);
                    ClassifiedList = models;
                }
                else if (model.CityID > 0 && model.MinPrice > 0 && model.MaxPrice > 0 && model.ChildClassifiedAdCategoryID <= 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.CityID = @0 and ads.Price >= @1 and ads.Price <= @2 order by ads.PublishedDate desc", model.CityID, model.MinPrice, model.MaxPrice);
                    ClassifiedList = models;
                }
                else if (model.CityID > 0 && model.ChildClassifiedAdCategoryID > 0 && model.MaxPrice > 0 && model.MinPrice <= 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.CityID = @0 and ads.Price <= @1 and ads.ClassifiedAdCategoryID = @2 order by ads.PublishedDate desc", model.CityID, model.MaxPrice, model.ChildClassifiedAdCategoryID);
                    ClassifiedList = models;
                }
                else if (model.CityID > 0 && model.ChildClassifiedAdCategoryID > 0 && model.MinPrice > 0 && model.MaxPrice <= 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.CityID = @0 and ads.Price >= @1 and ads.ClassifiedAdCategoryID = @2 order by ads.PublishedDate desc", model.CityID, model.MinPrice, model.ChildClassifiedAdCategoryID);
                    ClassifiedList = models;
                }
            }
            return ClassifiedList;
        }

        private List<ClassifiedIndexModel> FilterClassifiedByTwoFilter(FilterClassifiedModel model)
        {
            List<ClassifiedIndexModel> ClassifiedList = new List<ClassifiedIndexModel>();
            List<ClassifiedIndexModel> models = new List<ClassifiedIndexModel>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                if (model.CityID > 0 && model.ChildClassifiedAdCategoryID > 0 && model.MinPrice <= 0 && model.MaxPrice <= 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.CityID = @0 and ads.ClassifiedAdCategoryID = @1 order by ads.PublishedDate desc", model.CityID, model.ChildClassifiedAdCategoryID);
                    ClassifiedList = models;
                }
                else if (model.CityID > 0 && model.MinPrice > 0 && model.ChildClassifiedAdCategoryID <= 0 && model.MaxPrice <= 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.CityID = @0 and ads.Price >= @1 order by ads.PublishedDate desc", model.CityID, model.MinPrice);
                    ClassifiedList = models;
                }
                else if (model.CityID > 0 && model.MaxPrice > 0 && model.ChildClassifiedAdCategoryID <= 0 && model.MinPrice <= 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.CityID = @0 and ads.Price <= @1 order by ads.PublishedDate desc", model.CityID, model.MaxPrice);
                    ClassifiedList = models;
                }
                else if (model.ChildClassifiedAdCategoryID > 0 && model.MinPrice > 0 && model.CityID <= 0 && model.MaxPrice <= 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.Price >= @0 and ads.ClassifiedAdCategoryID = @1 order by ads.PublishedDate desc", model.MinPrice, model.ChildClassifiedAdCategoryID);
                    ClassifiedList = models;
                }
                else if (model.ChildClassifiedAdCategoryID > 0 && model.MaxPrice > 0 && model.MinPrice <= 0 && model.CityID <= 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.Price <= @0 and ads.ClassifiedAdCategoryID = @1 order by ads.PublishedDate desc", model.MaxPrice, model.ChildClassifiedAdCategoryID);
                    ClassifiedList = models;
                }
                else if (model.MinPrice > 0 && model.MaxPrice > 0 && model.ChildClassifiedAdCategoryID <= 0 && model.CityID <= 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.Price >= @0 and ads.Price <= @1 order by ads.PublishedDate desc", model.MinPrice, model.MaxPrice);
                    ClassifiedList = models;
                }
            }
            return ClassifiedList;

        }
        private List<ClassifiedIndexModel> FilterClassifiedByOneItem(FilterClassifiedModel model)
        {
            List<ClassifiedIndexModel> ClassifiedList = new List<ClassifiedIndexModel>();
            List<ClassifiedIndexModel> models = new List<ClassifiedIndexModel>();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                if (model.CityID > 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.CityID = @0 order by ads.PublishedDate desc", model.CityID);
                    ClassifiedList = models;
                }
                else if (model.ChildClassifiedAdCategoryID > 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.ClassifiedAdCategoryID = @0 order by ads.PublishedDate desc", model.ChildClassifiedAdCategoryID);
                    foreach (var each in models.Where(x => !ClassifiedList.Exists(a => a.ClassifiedID == x.ClassifiedID)))
                    {
                        ClassifiedList = models;
                    }
                }
                else if (model.MinPrice > 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.Price >= @0 order by ads.PublishedDate desc", model.MinPrice);
                    foreach (var each in models.Where(x => !ClassifiedList.Exists(a => a.ClassifiedID == x.ClassifiedID)))
                    {
                        ClassifiedList = models;
                    }
                }
                else if (model.MaxPrice > 0)
                {
                    models = context.Fetch<ClassifiedIndexModel>("select ads.ClassifiedID, ads.Title, ads.CommentCount, ads.Description, ads.CityID, ads.Address, ads.PhoneCode, ads.PhoneNo, ads.CurrencyID, ads.Price, ads.UserID, ads.PublishedDate, cr.Code as CurrencyCode, city.Name as CityName, country.Name as CountryName, users.FirstName, users.LastName, users.ThumbnailProfileImg as UserProfileImg, cat.Name as ClassifiedCategoryName from Classified ads left join Currency cr on cr.CurrencyID = ads.CurrencyID left join Cities city on city.CityID = ads.CityID left join States states on states.StateID = city.StateID left join Countries country on country.CountryID = states.CountryID left join Users users on users.UserID = ads.UserID left join ClassifiedAdCategories cat on cat.ClassifiedAdCategoryID = ads.ClassifiedAdCategoryID where ads.Price <= @0 order by ads.PublishedDate desc", model.MaxPrice);
                    foreach (var each in models.Where(x => !ClassifiedList.Exists(a => a.ClassifiedID == x.ClassifiedID)))
                    {
                        ClassifiedList = models;
                    }
                }
            }

            return ClassifiedList;
        }


        public List<City> GetCityListOfClassifieds()
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<City>("select * from cities where cityid in (select Cityid from classified)");
            }
        }

        public ClassifiedModel GetclassifiedById(int classifiedId)
        {
            ClassifiedModel model = new ClassifiedModel();
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {

                var sql = string.Format("select C.title,C.ClassifiedID,C.CurrencyID,C.Address,C.Description,C.PhoneCode,C.PhoneNo, C.Price, c.UserID,cur.Code as CurrencyCode, Cat.ParentCategoryID as ParentClassifiedAdCategoryID, cat.Name as ChildCategoryName,cat.ClassifiedAdCategoryID as ChildClassifiedAdCategoryID, state.StateID as StateID, city.Name as CityName,C.CityId,state.Name as StateName,country.Name as CountryName,country.CountryID from classified C left join cities city on city.cityid = C.cityid left join states state on state.StateID = city.StateID left join Countries country on country.CountryID = state.CountryID left join Currency cur on cur.CurrencyID = C.CurrencyID left join ClassifiedAdCategories Cat on Cat.ClassifiedAdCategoryID = c.ClassifiedAdCategoryID where C.ClassifiedID = {0}", classifiedId);

                model = context.FirstOrDefault<ClassifiedModel>(sql);
                model.StateList = context.Fetch<State>("select * from States where CountryID = @0", model.CountryID);
                model.CityList = context.Fetch<City>("select * from Cities where StateID = @0", model.StateID);
                model.ImgsUrl = context.Fetch<NewsfeedClassifiedMedia>("select * from NewsfeedClassifiedMedia where EntityID = @0 and EntityTypeID = @1", classifiedId, Akhbaar.Shared.Helper.Enum.EntityTypeEnum.Classifieds);
            }
            return model;
        }

        public Classified GetclassifiedForDeleteById(int classifiedId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Classified>("select * from Classified where ClassifiedID = @0", classifiedId).FirstOrDefault();
            }
        }
    }
}

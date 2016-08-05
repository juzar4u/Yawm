using Akhbaar.Shared.Helper.Enum;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using AkhbaarAlYawm.DataAccess.Custom.Entities.PP_Entities_Model;
using Spotunity.DataAccess.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.Application.Services
{
    public class CommentServices
    {
        private static CommentServices _instace;

        public static CommentServices GetInstance
        {
            get
            {
                if (_instace == null)
                {
                    _instace = new CommentServices();
                }

                return _instace;
            }
        }
        public int InsertComment(Comment comment)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Insert(comment);
            }
        }

        public int DeleteComment(Comment comment)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return (int)context.Delete(comment);
            }
        }

        public CommentMasterModel GetAllCommentsByArticleID(int articleID, int commentCategoryID, UserModel user)
        {
            List<CommentMaster> comments = new List<CommentMaster>();
            List<CommentModel> _comments = new List<CommentModel>();
            CommentMasterModel _comment = new CommentMasterModel();
            _comment.User = user;
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                _comments = context.Fetch<CommentModel>("select Comments.ArticleID, Comments.CommentID, Comments.Content, Comments.CreatedBy, Comments.CreatedOn, Comments.IsApproved, Comments.ParentCommentID, Comments.UpdatedBy, Comments.UpdatedOn, usr.FirstName as UserName, usr.ThumbnailProfileImg from Comments left join Users usr on usr.UserID = Comments.CreatedBy where ParentCommentID is null and Comments.IsApproved = 1 and ArticleID = @0 and CommentCategoryID = @1", articleID, commentCategoryID);
                foreach (var item in _comments)
                {
                    comments.Add(new CommentMaster()
                    {
                        ParentComment = item,
                        Comments = context.Fetch<CommentModel>("select Comments.ArticleID, Comments.CommentID, Comments.Content, Comments.CreatedBy, Comments.CreatedOn, Comments.IsApproved, Comments.ParentCommentID, Comments.UpdatedBy, Comments.UpdatedOn, usr.FirstName as UserName, usr.ThumbnailProfileImg from Comments left join Users usr on usr.UserID = Comments.CreatedBy where ArticleID = @0 and ParentCommentID = @1 and Comments.IsApproved = 1 and CommentCategoryID = @2", articleID, item.CommentID, commentCategoryID)
                    });
                }
                _comment.MasterComments = comments;
                _comment.ArticleID = articleID;
            }
            return _comment;
        }

        public Comment GetCommentByCommentID(int commentId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<Comment>("select * from comments where commentId = @0", commentId).FirstOrDefault();
            }
        }

        public void DeleteCommentsByCommentID(int commentId, int CommentCategoryID)
        {
            Comment comment = new Comment();

            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                comment = context.Fetch<Comment>("select * from comments where commentId = @0", commentId).FirstOrDefault();
                if (comment.ParentCommentID == null)
                {
                    List<Comment> childcomments = new List<Comment>();
                    childcomments = context.Fetch<Comment>("select * from comments where ParentCommentID = @0", comment.CommentID);

                    foreach (var item in childcomments)
                    {
                        DeleteComment(item);
                    }
                }
                DeleteComment(comment);

                updateCommentCount((int)comment.ArticleID, CommentCategoryID);
            }
        }

        public void updateCommentCount(int articleId, int commentcategoryID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                //Article _article = new Article();
                int commentcount = context.Fetch<int>("select count(*) from Comments where ArticleID = @0", articleId).FirstOrDefault();
                switch (commentcategoryID)
                {
                    case (int)CommentCategoryEnum.ArticleType:
                        Article _article = new Article();
                        _article = context.Fetch<Article>("select * from Articles where ArticleID = @0", articleId).FirstOrDefault();
                        _article.CommentCount = commentcount;

                        PP_ArticleServices.GetInstance.UpdateArticle(_article);
                        break;

                    case (int)CommentCategoryEnum.ClassifiedType:
                        Classified _classified = new Classified();
                        _classified = context.Fetch<Classified>("select * from Classified where ClassifiedID = @0", articleId).FirstOrDefault();
                        _classified.CommentCount = commentcount;

                        ClassifiedServices.GetInstance.UpdateClassified(_classified);
                        break;

                }

                //_article = context.Fetch<Article>("select * from Articles where ArticleID = @0 and CommentCategoryID = @1", articleId, commentcategoryID).FirstOrDefault();
                //_article.CommentCount = commentcount;

                //PP_ArticleServices.GetInstance.UpdateArticle(_article);
            }
        }

    }
}

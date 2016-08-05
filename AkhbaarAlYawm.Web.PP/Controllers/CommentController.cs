using Akhbaar.Shared.Helper.Enum;
using AkhbaarAlYawm.Application.Helper;
using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.DataAccess;
using AkhbaarAlYawm.DataAccess.Custom.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AkhbaarAlYawm.Web.PP.Controllers
{
    public class CommentController : Controller
    {
        //
        // GET: /Comment/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NewComment(CommentMasterModel model)
        {
            UserComment usercomment = new UserComment();
            usercomment.user = AuthHelper.LoginFromCookie();
            usercomment.ArticleID = model.ArticleID;
            CommentServices.GetInstance.updateCommentCount(model.ArticleID, model.ArticleCategoryTypeID);
            return PartialView(usercomment);
        }

        public ActionResult PostComment(int userId, int articleId, string comment, int parentCommentId, int commentCategoryID)
        {
            Comment _comment = new Comment();
            _comment.ArticleID = articleId;
            _comment.Content = comment;
            _comment.CreatedBy = userId;
            _comment.CreatedOn = DateTime.Now;
            _comment.IsApproved = true;
            if (parentCommentId > 0)
            {
                _comment.ParentCommentID = parentCommentId;
            }
            else
            {
                _comment.ParentCommentID = null;
            }
            _comment.CommentCategoryID = commentCategoryID;
            CommentServices.GetInstance.InsertComment(_comment);
            CommentServices.GetInstance.updateCommentCount(articleId, commentCategoryID);
            ViewBag.message = "Comment Posted!";
            return PartialView("~/Views/Shared/PartialCustomPopupMessage.cshtml");
        }


        public ActionResult DeleteComment(int commentId, int commentCategoryID)
        {
            CommentServices.GetInstance.DeleteCommentsByCommentID(commentId, commentCategoryID);


            ViewBag.message = "Comment Deleted!";
            return PartialView("~/Views/Shared/PartialCustomPopupMessage.cshtml");
        }

        public ActionResult ReplyComment(int commentId, string IsParentComment)
        {
            UserModel _user = AuthHelper.LoginFromCookie();
            if (_user != null)
            {
                ReplyComment model = new ReplyComment();
                model.CommentID = commentId;
                model.IsParentComment = IsParentComment;
                model.user = _user;
                return PartialView(model);
            }

            return Redirect("/Account/Login");
        }

        public ActionResult PostReplyComment(int userId, int commentId, string isParentComment, string comment, int CommentCategoryID)
        {
            Comment NewComment = new Comment();
            Comment _comment = CommentServices.GetInstance.GetCommentByCommentID(commentId);

            NewComment.ArticleID = _comment.ArticleID;
            if (isParentComment == "true")
            {
                NewComment.ParentCommentID = _comment.CommentID;
            }
            else
            {
                NewComment.ParentCommentID = _comment.ParentCommentID;
            }
            NewComment.Content = comment;
            NewComment.CreatedBy = userId;
            NewComment.CreatedOn = DateTime.Now;
            NewComment.IsApproved = true;
            NewComment.CommentCategoryID = CommentCategoryID;
            CommentServices.GetInstance.InsertComment(NewComment);
            CommentServices.GetInstance.updateCommentCount((int)_comment.ArticleID, CommentCategoryID);
            ViewBag.message = "Successfully Replied";
            return PartialView("~/Views/Shared/PartialCustomPopupMessage.cshtml");
        }

    }
}

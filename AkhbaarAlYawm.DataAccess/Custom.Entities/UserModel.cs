using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarAlYawm.DataAccess.Custom.Entities
{
    public class UserModel
    {
        public int? UserID { get; set; }

        [Required(ErrorMessage="E-jamat ID is required")]
        public int EjamatID { get; set; }
        [Required(ErrorMessage="Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password mismatch")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string UserGUID { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsVerified { get; set; }
        public int UserStatusID { get; set; }
        public string UserStatus { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public List<UserList> UserList { get; set; }
        public string ProfileImg { get; set; }
        public string ThumbnailProfileImg { get; set; }
        public string changePassword { get; set; }
    }

    public class UserProfileModel
    {
        public int UserProfileID { get; set; }
        public int UserID { get; set; }
        public int EjamatID { get; set; }
        public string Jamaat { get; set; }
        public string Specialisation { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string HomeAddress { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ThumbnailProfileImg { get; set; }
        public int? LoggedInUserID { get; set; }
    }

    public class ClearUserProfileModel
    {
        public int UserID { get; set; }
    }


    public class SendMessageModel
    {
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public string subject { get; set; }
        public string Content { get; set; }
    }


    public class LoginModel
    {

        [Required(ErrorMessage="Email is Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }

    [Serializable]
    public class SessionAkhbaarUserEntity
    {
        public int UserID { get; set; }
        public string UserGUID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserStatusID { get; set; }
    }

    public class UserRoleModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class UserList
    {
        public int UserID { get; set; }
        public int EjamatID { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsVerified { get; set; }
        public int UserStatusID { get; set; }
        public string UserStatus { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }

    public class UserComment 
    {
        public int ArticleID { get; set; }
        public UserModel user { get; set; }
    }

    public class ReplyComment
    {
        public int CommentID { get; set; }
        public string IsParentComment { get; set; }
        public UserModel user { get; set; }
    }



    public class CommentModel
    {
        public int CommentID { get; set; }
        public int ArticleID { get; set; }
        public int ParentCommentID { get; set; }
        public bool IsApproved { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UserName { get; set; }
        public string ThumbnailProfileImg { get; set; }

    }

    public class classifiedCommentModel
    {
        public CommentMasterModel ClassifiedComments { get; set; }
    }

    public class CommentMaster
    {
        public CommentModel ParentComment { get; set; }
        public List<CommentModel> Comments { get; set; }
    }

    public class CommentMasterModel
    {
        public int ArticleCategoryTypeID { get; set; }
        public int ArticleID { get; set; }
        public UserModel User { get; set; }
        public List<CommentMaster> MasterComments { get; set; }
    }
    public class ClassifiedModel
    {
        public string Title { get; set; }
        public int CityID { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string address { get; set; }
        public string Description { get; set; }
        public int PhoneCode { get; set; }
        public long PhoneNo { get; set; }
        public string CurrencyCode { get; set; }
        public int price { get; set; }
        public UserModel user { get; set; }
        public int UserID { get; set; }
        public List<Currency> CurrencyList { get; set; }
        public int CurrencyID { get; set; }
        public string CurrencyName { get; set; }
        public List<Country> CountryList { get; set; }
        public int ParentClassifiedAdCategoryID { get; set; }
        public string ParentCategoryName { get; set; }
        public string ChildCategoryName { get; set; }
        public int ChildClassifiedAdCategoryID { get; set; }
        public List<ClassifiedAdCategories> ClassifiedAdParentList { get; set; }
        public string StateID { get; set; }
        public List<ClassifiedAdCategories> ClassifiedAdChildList { get; set; }
        public int CountryID { get; set; }
        public List<State> StateList { get; set; }
        public List<City> CityList { get; set; }
        public List<NewsfeedClassifiedMedia> ImgsUrl { get; set; }

    }


    public class ClassifiedIndexModel
    {
        public string Title { get; set; }
        public int CityID { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public string address { get; set; }
        public string Description { get; set; }
        public int PhoneCode { get; set; }
        public long PhoneNo { get; set; }
        public string CurrencyCode { get; set; }
        public int price { get; set; }
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserProfileImg { get; set; }
        public int CurrencyID { get; set; }
        public DateTime PublishedDate { get; set; }
        public int ClassifiedID { get; set; }
        public string ClassifiedCategoryName { get; set; }
        public List<NewsfeedClassifiedMedia> mediaList { get; set; }
        public int CommentCount { get; set; }
        public bool UserLikedClassified { get; set; }
        public string userlikescount { get; set; }
    }

    public class classifiedDeleteModel
    {
        public int ClassifiedID { get; set; }
    }
    public class ClassfiedAdModel
    {
        public ClassifiedAdCategories ParentCategory { get; set; }
        public List<ClassifiedAdCategories> ChildCategories { get; set; }
    }

    public class FilterClassifiedModel
    {
        public List<ClassifiedAdCategories> ParentCategories { get; set; }
        public List<Country> CountryList { get; set; }
        public int ClassifiedAdCategoryID { get; set; }
        public int CountryID { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public int ChildClassifiedAdCategoryID { get; set; }
    }

    [Serializable]
    public class ArgaamJson
    {
        public string ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public string Data { get; set; }
        public int JsonRequestBehavior { get; set; }
    }
    [Serializable]
    public class ArgaamUserJson
    {
        public string ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public UserModel Data { get; set; }
        public int JsonRequestBehavior { get; set; }
    }


    public class JsonUserLikes
    {
        public string CountText { get; set; }
        public int CountData { get; set; }
    }


}

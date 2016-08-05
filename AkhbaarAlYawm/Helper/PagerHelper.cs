using System;
using Spotunity.Web.CP;
public class PagerHelper
{
    public PagerHelper()
    {
        this.AjaxEnabled = false;
    }
    public const string CURRENT_PAGE_NUMBER = "C_PG_N";
    public const string ORDER = "ORDER";
    public const string ORDER_BY = "ORDER_BY";
    public const string ORDER_FOR_PAGER = "ORDER_FOR_PAGER";
    public const string TOTAL_RECORDS = "TOTAL_REC_COUNT";

    
    public const int DirectlyNavigablePageCount = 10;

    public int TotalRecords { get; set; }
    public int CurrentPage { get; set; }

    public int RecordsPerPage
    {
        get
        {
            if(recordPerpage == 0)
                return (DirectlyNavigablePageCount);
            else
                return recordPerpage;
        }
        set 
        {
            recordPerpage = value;
        }
    }
    int recordPerpage = 0;

    //public Views ParentView { get; set; }
    public string OrderBy { get; set; }
    public string OrderType { get; set; }

    public int TotalPages
    {
        get
        {
            return ((int) Math.Ceiling((double) TotalRecords/RecordsPerPage));

        }
    }

    public int StartPageNumber
    {
        get
        {
            return Math.Abs((((((int) Math.Ceiling((double) CurrentPage/DirectlyNavigablePageCount))
                               *DirectlyNavigablePageCount) - DirectlyNavigablePageCount) + 1));
        }
    }

    public int EndPageNumber
    {
        get
        {
            if (StartPageNumber + DirectlyNavigablePageCount > TotalPages)
            {
                return (TotalPages);
            }
            return (StartPageNumber + DirectlyNavigablePageCount - 1);
        }
    }

    public bool ShowFirstPrevLinks
    {
        get
        {
            if (TotalPages == 0 || TotalPages < CurrentPage)
                return false;
            return (StartPageNumber > DirectlyNavigablePageCount);
        }
    }

    public bool ShowNextLastLinks
    {
        get
        {
            if (TotalPages == 0 || TotalPages < CurrentPage)
                return false;
            return (EndPageNumber < TotalPages);
        }
    }

    public static string ToggleOrder(string order)
    {
        return (order == "ASC" ? "DESC" : "ASC");
    }

    public bool IsForSearchPage { get; set; }

    public bool AjaxEnabled { get; set; }
    public string OnClientClickAjaxCall { get; set; }
    public string GetAjaxRelatedAttributes()
    {
        if (!AjaxEnabled)
            return "";
        else
            return string.Format("onclick='{0};'", this.OnClientClickAjaxCall) ;
    }
}
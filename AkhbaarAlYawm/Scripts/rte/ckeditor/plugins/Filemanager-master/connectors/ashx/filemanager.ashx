<%@ WebHandler Language="C#" Class="filemanager" %>

//	** Filemanager ASP.NET connector
//
//	** .NET Framework >= 2.0
//
//	** license	    MIT License
//	** author		Ondřej "Yumi Yoshimido" Brožek | <cholera@hzspraha.cz>
//	** Copyright	Author

using System;
using System.Web;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using System.Web;

public class filemanager : IHttpHandler
{

    //===================================================================
    //==================== EDIT CONFIGURE HERE ==========================
    //===================================================================

    public string IconDirectory = "./images/fileicons/"; // Icon directory for filemanager. [string]
    public string[] imgExtensions = new string[] { ".jpg", ".png", ".jpeg", ".gif", ".bmp" }; // Only allow this image extensions. [string]

    //===================================================================
    //========================== END EDIT ===============================
    //===================================================================       


    private bool IsImage(FileInfo fileInfo)
    {
        foreach (string ext in imgExtensions)
        {
            if (Path.GetExtension(fileInfo.FullName) == ext)
            {
                return true;
            }
        }

        return false;
    }


    private string getFolderInfo(string path)
    {
        //DirectoryInfo RootDirInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(path));
        System.Collections.Generic.List<ArgaamPlus.Shared.DataServices.ImagesRepository> img = ArgaamPlus.CP.Application.Services.CommonServices.Instance.GetImagesRepository();
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("{");

        int i = 0;

        foreach (var item in img)
        {
            if (i > 0)
            {
                sb.Append(",");
                sb.AppendLine();
            }

            sb.AppendLine("\"" + item.ImageURL + "\": {");
            sb.AppendLine("\"Path\": \"" + item.ImageURL + "/\",");
            sb.AppendLine("\"Filename\": \"" + item.ImageTitleEn + "\",");
            sb.AppendLine("\"File Type\": \"dir\",");
            sb.AppendLine("\"Preview\": \"" + IconDirectory + "_Open.png\",");
            sb.AppendLine("\"Properties\": {");
            sb.AppendLine("\"Date Created\": \"" + item.CreatedOn + "\", ");
            sb.AppendLine("\"Date Modified\": \"" + item.CreatedOn + "\", ");
            sb.AppendLine("\"Height\": 0,");
            sb.AppendLine("\"Width\": 0,");
            sb.AppendLine("\"Size\": 0 ");
            sb.AppendLine("},");
            sb.AppendLine("\"Error\": \"\",");
            sb.AppendLine("\"Code\": 0	");
            sb.Append("}");

            i++;
        }


        foreach (var item in img)
        {
            if (i > 0)
            {
                sb.Append(",");
                sb.AppendLine();
            }

            sb.AppendLine("\"" + item.ImageURL + "\": {");
            sb.AppendLine("\"Path\": \"" + item.ImageURL + "\",");
            sb.AppendLine("\"Filename\": \"" + item.ImageTitleEn + "\",");
            sb.AppendLine("\"File Type\": \"" + ".jpg" + "\",");

            //if (IsImage(fileInfo))
            //{
            sb.AppendLine("\"Preview\": \"" + item.ImageURL + "\",");
            //}
            //else
            //{
            //    sb.AppendLine("\"Preview\": \"" + String.Format("{0}{1}.png", IconDirectory, fileInfo.Extension.Replace(".", "")) + "\",");
            //}

            sb.AppendLine("\"Properties\": {");
            sb.AppendLine("\"Date Created\": \"" + item.CreatedOn + "\", ");
            sb.AppendLine("\"Date Modified\": \"" + item.CreatedOn + "\", ");

            //if (IsImage(fileInfo))
            //{
            //    using (System.Drawing.Image img = System.Drawing.Image.FromFile(fileInfo.FullName))
            //    {
            //        sb.AppendLine("\"Height\": " + img.Height.ToString() + ",");
            //        sb.AppendLine("\"Width\": " + img.Width.ToString() + ",");
            //    }
            //}

            sb.AppendLine("\"Height\": " + "300" + ",");
            sb.AppendLine("\"Width\": " + "300" + ",");

            sb.AppendLine("\"Size\": " + "285" + " ");
            sb.AppendLine("},");
            sb.AppendLine("\"Error\": \"\",");
            sb.AppendLine("\"Code\": 0	");
            sb.Append("}");

            i++;
        }

        //foreach (DirectoryInfo DirInfo in RootDirInfo.GetDirectories()) 
        //{
        //    if (i > 0) 
        //    {
        //        sb.Append(",");
        //        sb.AppendLine();
        //    }

        //    sb.AppendLine("\"" + Path.Combine(path, DirInfo.Name) + "\": {");
        //    sb.AppendLine("\"Path\": \"" + Path.Combine(path, DirInfo.Name) + "/\",");
        //    sb.AppendLine("\"Filename\": \"" + DirInfo.Name + "\",");
        //    sb.AppendLine("\"File Type\": \"dir\",");
        //    sb.AppendLine("\"Preview\": \"" + IconDirectory + "_Open.png\",");
        //    sb.AppendLine("\"Properties\": {");
        //    sb.AppendLine("\"Date Created\": \"" + DirInfo.CreationTime.ToString() + "\", ");
        //    sb.AppendLine("\"Date Modified\": \"" + DirInfo.LastWriteTime.ToString() + "\", ");
        //    sb.AppendLine("\"Height\": 0,");
        //    sb.AppendLine("\"Width\": 0,");
        //    sb.AppendLine("\"Size\": 0 ");
        //    sb.AppendLine("},");
        //    sb.AppendLine("\"Error\": \"\",");
        //    sb.AppendLine("\"Code\": 0	");
        //    sb.Append("}");

        //    i++;
        //}

        //foreach (FileInfo fileInfo in RootDirInfo.GetFiles())
        //{
        //    if (i > 0)
        //    {
        //        sb.Append(",");
        //        sb.AppendLine();
        //    }

        //    sb.AppendLine("\"" + Path.Combine(path, fileInfo.Name) + "\": {");
        //    sb.AppendLine("\"Path\": \"" + Path.Combine(path, fileInfo.Name) + "\",");
        //    sb.AppendLine("\"Filename\": \"" + fileInfo.Name + "\",");
        //    sb.AppendLine("\"File Type\": \"" + fileInfo.Extension.Replace(".","") + "\",");

        //    if (IsImage(fileInfo))
        //    {
        //        sb.AppendLine("\"Preview\": \"" + Path.Combine(path, fileInfo.Name) + "\",");
        //    }
        //    else
        //    {
        //        sb.AppendLine("\"Preview\": \"" + String.Format("{0}{1}.png", IconDirectory, fileInfo.Extension.Replace(".", "")) + "\",");
        //    }

        //    sb.AppendLine("\"Properties\": {");
        //    sb.AppendLine("\"Date Created\": \"" + fileInfo.CreationTime.ToString() + "\", ");
        //    sb.AppendLine("\"Date Modified\": \"" + fileInfo.LastWriteTime.ToString() + "\", ");

        //    if (IsImage(fileInfo)) 
        //    {
        //        using (System.Drawing.Image img = System.Drawing.Image.FromFile(fileInfo.FullName))
        //        {
        //            sb.AppendLine("\"Height\": " + img.Height.ToString() + ",");
        //            sb.AppendLine("\"Width\": " + img.Width.ToString() + ",");
        //        }
        //    }

        //    sb.AppendLine("\"Size\": " + fileInfo.Length.ToString() + " ");
        //    sb.AppendLine("},");
        //    sb.AppendLine("\"Error\": \"\",");
        //    sb.AppendLine("\"Code\": 0	");
        //    sb.Append("}");

        //    i++;
        //}

        sb.AppendLine();
        sb.AppendLine("}");

        return sb.ToString();
    }

    private string getInfo(string path)
    {
        StringBuilder sb = new StringBuilder();

        //FileAttributes attr = File.GetAttributes(HttpContext.Current.Server.MapPath(path));

        //if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
        //{
        //    DirectoryInfo DirInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(path));

        //    sb.AppendLine("{");
        //    sb.AppendLine("\"Path\": \"" + path + "\",");
        //    sb.AppendLine("\"Filename\": \"" + DirInfo.Name + "\",");
        //    sb.AppendLine("\"File Type\": \"dir\",");
        //    sb.AppendLine("\"Preview\": \"" + IconDirectory + "_Open.png\",");
        //    sb.AppendLine("\"Properties\": {");
        //    sb.AppendLine("\"Date Created\": \"" + DirInfo.CreationTime.ToString() + "\", ");
        //    sb.AppendLine("\"Date Modified\": \"" + DirInfo.LastWriteTime.ToString() + "\", ");
        //    sb.AppendLine("\"Height\": 0,");
        //    sb.AppendLine("\"Width\": 0,");
        //    sb.AppendLine("\"Size\": 0 ");
        //    sb.AppendLine("},");
        //    sb.AppendLine("\"Error\": \"\",");
        //    sb.AppendLine("\"Code\": 0	");
        //    sb.AppendLine("}");
        //}
        //else
        //{
        //FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(path));

        sb.AppendLine("{");
        sb.AppendLine("\"Path\": \"" + path + "\",");
        sb.AppendLine("\"Filename\": \"" + path + "\",");
        sb.AppendLine("\"File Type\": \"" + ".jpg" + "\",");

        //if (IsImage(fileInfo))
        //{
        sb.AppendLine("\"Preview\": \"" + path + "\",");
        //}
        //else
        //{
        //    sb.AppendLine("\"Preview\": \"" + String.Format("{0}{1}.png", IconDirectory, fileInfo.Extension.Replace(".", "")) + "\",");
        //}

        sb.AppendLine("\"Properties\": {");
        sb.AppendLine("\"Date Created\": \"" + "" + "\", ");
        sb.AppendLine("\"Date Modified\": \"" + "" + "\", ");

        //if (IsImage(fileInfo)) 
        //{
        //    using (System.Drawing.Image img = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(path)))
        //    {
        sb.AppendLine("\"Height\": " + "300" + ",");
        sb.AppendLine("\"Width\": " + "300" + ",");
        //    }
        //}

        sb.AppendLine("\"Size\": " + "300" + " ");
        sb.AppendLine("},");
        sb.AppendLine("\"Error\": \"\",");
        sb.AppendLine("\"Code\": 0	");
        sb.AppendLine("}");
        //}

        return sb.ToString();

    }

    private string Rename(string path, string newName)
    {
        FileAttributes attr = File.GetAttributes(HttpContext.Current.Server.MapPath(path));

        StringBuilder sb = new StringBuilder();

        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(path));
            Directory.Move(HttpContext.Current.Server.MapPath(path), Path.Combine(dirInfo.Parent.FullName, newName));

            DirectoryInfo fileInfo2 = new DirectoryInfo(Path.Combine(dirInfo.Parent.FullName, newName));

            sb.AppendLine("{");
            sb.AppendLine("\"Error\": \"No error\",");
            sb.AppendLine("\"Code\": 0,");
            sb.AppendLine("\"Old Path\": \"" + path + "\",");
            sb.AppendLine("\"Old Name\": \"" + newName + "\",");
            sb.AppendLine("\"New Path\": \"" +
                fileInfo2.FullName.Replace(HttpRuntime.AppDomainAppPath, "/").Replace(Path.DirectorySeparatorChar, '/') + "\",");
            sb.AppendLine("\"New Name\": \"" + fileInfo2.Name + "\"");
            sb.AppendLine("}");

        }
        else
        {
            FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(path));
            File.Move(HttpContext.Current.Server.MapPath(path), Path.Combine(fileInfo.Directory.FullName, newName));

            FileInfo fileInfo2 = new FileInfo(Path.Combine(fileInfo.Directory.FullName, newName));

            sb.AppendLine("{");
            sb.AppendLine("\"Error\": \"No error\",");
            sb.AppendLine("\"Code\": 0,");
            sb.AppendLine("\"Old Path\": \"" + path + "\",");
            sb.AppendLine("\"Old Name\": \"" + newName + "\",");
            sb.AppendLine("\"New Path\": \"" +
                fileInfo2.FullName.Replace(HttpRuntime.AppDomainAppPath, "/").Replace(Path.DirectorySeparatorChar, '/') + "\",");
            sb.AppendLine("\"New Name\": \"" + fileInfo2.Name + "\"");
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private string Delete(string path)
    {
        FileAttributes attr = File.GetAttributes(HttpContext.Current.Server.MapPath(path));

        StringBuilder sb = new StringBuilder();

        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
        {
            Directory.Delete(HttpContext.Current.Server.MapPath(path), true);
        }
        else
        {
            File.Delete(HttpContext.Current.Server.MapPath(path));
        }

        sb.AppendLine("{");
        sb.AppendLine("\"Error\": \"No error\",");
        sb.AppendLine("\"Code\": 0,");
        sb.AppendLine("\"Path\": \"" + path + "\"");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private string AddFolder(string path, string NewFolder)
    {
        StringBuilder sb = new StringBuilder();

        Directory.CreateDirectory(Path.Combine(HttpContext.Current.Server.MapPath(path), NewFolder));

        sb.AppendLine("{");
        sb.AppendLine("\"Parent\": \"" + path + "\",");
        sb.AppendLine("\"Name\": \"" + NewFolder + "\",");
        sb.AppendLine("\"Error\": \"No error\",");
        sb.AppendLine("\"Code\": 0");
        sb.AppendLine("}");

        return sb.ToString();
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ClearHeaders();
        context.Response.ClearContent();
        context.Response.Clear();

        switch (context.Request["mode"])
        {
            case "getinfo":

                context.Response.ContentType = "plain/text";
                context.Response.ContentEncoding = Encoding.UTF8;

                context.Response.Write(getInfo(context.Request["path"]));

                break;
            case "getfolder":

                context.Response.ContentType = "plain/text";
                context.Response.ContentEncoding = Encoding.UTF8;

                context.Response.Write(getFolderInfo(context.Request["path"]));

                break;
            case "rename":

                context.Response.ContentType = "plain/text";
                context.Response.ContentEncoding = Encoding.UTF8;

                context.Response.Write(Rename(context.Request["old"], context.Request["new"]));

                break;
            case "delete":

                context.Response.ContentType = "plain/text";
                context.Response.ContentEncoding = Encoding.UTF8;

                context.Response.Write(Delete(context.Request["path"]));

                break;
            case "addfolder":

                context.Response.ContentType = "plain/text";
                context.Response.ContentEncoding = Encoding.UTF8;

                context.Response.Write(AddFolder(context.Request["path"], context.Request["name"]));

                break;
            case "download":

                FileInfo fi = new FileInfo(context.Server.MapPath(context.Request["path"]));

                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + context.Server.UrlPathEncode(fi.Name));
                context.Response.AddHeader("Content-Length", fi.Length.ToString());
                context.Response.ContentType = "application/octet-stream";
                context.Response.TransmitFile(fi.FullName);

                break;
            case "add":

                #region Creative
                HttpPostedFile hpf = context.Request.Files[0];
                string titleEn = context.Request.Form["fileTitleEn"];
                string titleAr = context.Request.Form["fileTitleAr"];

                var result = ArgaamPlus.Shared.Helpers.CreativeServiceHelper.Instance.Upload(hpf, 600, 600, false, false, false);
                //result = ArgaamPlus.Shared.Helpers.CreativeServiceHelper.Instance.Thumbnail(int.Parse(result.ReturnValue.ToString()), 175, 175);

                if (result.WasSuccessful)
                {
                    //string CKEditorFuncNum = Request["CKEditorFuncNum"];
                    //model.CreativeID = int.Parse(result.ReturnValue.ToString());
                    //model.ImageURL = result.StackTrack;
                    //Response.Write("<script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + model.ImageURL + "\");</script>");
                    //Response.End();

                    ArgaamPlus.CP.Application.Services.CommonServices.Instance.AddImageToRepository(result.StackTrack, titleEn, titleAr, int.Parse(result.ReturnValue.ToString()));
                    
                    context.Response.ContentType = "text/html";
                    context.Response.ContentEncoding = Encoding.UTF8;

                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("{");
                    sb.AppendLine("\"Path\": \"" + result.StackTrack + "\",");
                    sb.AppendLine("\"Name\": \"" + titleEn + "\",");
                    sb.AppendLine("\"Error\": \"No error\",");
                    sb.AppendLine("\"Code\": 0");
                    sb.AppendLine("}");

                    System.Web.UI.WebControls.TextBox txt = new System.Web.UI.WebControls.TextBox();
                    txt.TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine;
                    txt.Text = sb.ToString();

                    StringWriter sw = new StringWriter();
                    System.Web.UI.HtmlTextWriter writer = new System.Web.UI.HtmlTextWriter(sw);
                    txt.RenderControl(writer);

                    context.Response.Write(sw.ToString());

                    //break;
                }


                #endregion

                //System.Web.HttpPostedFile file = context.Request.Files[0];

                //string path = context.Request["currentpath"];

                //file.SaveAs(context.Server.MapPath(Path.Combine(path, Path.GetFileName(file.FileName))));

                //context.Response.ContentType = "text/html";
                //context.Response.ContentEncoding = Encoding.UTF8;

                //StringBuilder sb = new StringBuilder();

                //sb.AppendLine("{");
                //sb.AppendLine("\"Path\": \"" + path + "\",");
                //sb.AppendLine("\"Name\": \"" + Path.GetFileName(file.FileName) + "\",");
                //sb.AppendLine("\"Error\": \"No error\",");
                //sb.AppendLine("\"Code\": 0");
                //sb.AppendLine("}");

                //System.Web.UI.WebControls.TextBox txt = new System.Web.UI.WebControls.TextBox();
                //txt.TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine;
                //txt.Text = sb.ToString();

                //StringWriter sw = new StringWriter();
                //System.Web.UI.HtmlTextWriter writer = new System.Web.UI.HtmlTextWriter(sw);
                //txt.RenderControl(writer);

                //context.Response.Write(sw.ToString()); 

                break;
            default:
                break;
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
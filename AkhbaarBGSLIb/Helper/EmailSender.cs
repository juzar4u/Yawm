
using AkhbaarAlYawm.Application.Helper;
using AkhbaarAlYawm.Application.Services;
using AkhbaarAlYawm.DataAccess;
using Spotunity.DataAccess.helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AkhbaarBGSLIb.Helpers
{

    public class EmailSender
    {
        public event EmailSentDlgt EmailSent;
        public event EmailSendingFailedDlgt EmailSendingFailed;
        //const char USER_ENTITY = 'U';
        //const char EVENT_ENTITY = 'E';
        //const char COMMENT_ENTITY = 'C';
        static string enableQaMode = "";
        static string qaEmailList = "";

        private bool emailOkForQA(string toEmail)
        {
            if (string.IsNullOrEmpty(enableQaMode))
            {
                enableQaMode = ConfigurationManager.AppSettings["ENABLE_QA_MODE"].ToLower();
            }
            if (enableQaMode == "true")
            {
                if (string.IsNullOrEmpty(qaEmailList))
                {
                    qaEmailList = ConfigurationManager.AppSettings["QA_EMAIL_ADDRESS_LIST"].ToLower();
                }
                toEmail = toEmail.ToLower();
                if (qaEmailList.IndexOf(toEmail) > -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public void SendSimpleEmails()
        {

            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                int emailToSendCount = 10;

                //List<EmailTokens> _emailTokens = new List<EmailTokens>();

                List<EmailTokens> _emailTokens = CommonServices.GetInstance.GetListOfEmailTokens().Take(emailToSendCount).ToList();

                MailMessage mm = null;
                SmtpClient smtpClient = new SmtpClient();

                if (UsingExchangeServer)
                {
                    //smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                    
                }
                foreach (EmailTokens simpleEmailToken in _emailTokens)
                {

                    try
                    {

                        if (simpleEmailToken.EntityType.Equals("U"))
                        {
                            GetFinalEmailBodyAndSubjectForUser(simpleEmailToken, context);
                        }
                        mm = CreateMailMessage(
                                FromAddress,
                                new MailAddressCollection() { new MailAddress(simpleEmailToken.ToAddress) },
                                null,
                                null,
                                true,
                                simpleEmailToken.EmailSubject,
                             simpleEmailToken.EmailBody);
                        simpleEmailToken.ProcessedOn = DateTime.Now;
                        if (mm.To.Count == 0)
                        {
                            mm.To.Add(simpleEmailToken.ToAddress);
                        }


                        try
                        {
                            
                            smtpClient.EnableSsl = this.EnableSSL;
                            smtpClient.Send(mm);
                            
                            simpleEmailToken.Status = "S";
                            simpleEmailToken.UnprocessableReason = "";
                            CommonServices.GetInstance.UpdateEmailToken(simpleEmailToken);
                            System.Diagnostics.Debug.WriteLine(string.Format("Akhbaar24 - Email Srv - Simple Email - Email successfully send at {0}, From {1}, To {2}", DateTime.Now, mm.From, mm.To));
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Akhbaar24 - Email Srv - Simple Email - Email send failed at {0} , reason  ->{1} , From {2}, To {3} ", DateTime.Now, ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""), mm.From, mm.To));
                            simpleEmailToken.UnprocessableReason = ex.Message;
                            simpleEmailToken.Status = "X";
                            CommonServices.GetInstance.UpdateEmailToken(simpleEmailToken);
                            if (this.EmailSendingFailed != null)
                            {
                                this.EmailSendingFailed(mm.To[0].Address, mm.Subject, ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Akhbaar24 - Simple Email Thread error at {0}, msg: {1}", DateTime.Now, ex.Message));
                        // TODO: Also log an excuse for not using it.
                        simpleEmailToken.ProcessedOn = DateTime.Now;
                        simpleEmailToken.Status = "X";
                        simpleEmailToken.UnprocessableReason = ex.Message;
                        CommonServices.GetInstance.UpdateEmailToken(simpleEmailToken);
                    }

                }
            }
        }

        #region Helper Methods and Properties

        MailAddress FromAddress
        {
            get
            {
                return (new MailAddress(ConfigurationManager.AppSettings["EMAIL_FROM_ADDRESS"], "Akhbaar-Al-Mumineen"));
                //return (new MailAddress("admin@danatev.com", "Spotunity"));

            }
        }
        bool UsingExchangeServer
        {
            get
            {
                //return (ConfigurationManager.AppSettings["USING_EXCHANGE_SERVER"] != null
                //    && ConfigurationManager.AppSettings["USING_EXCHANGE_SERVER"] == "true");
                return true;
            }
        }
        bool EnableSSL
        {
            get
            {
                //return (ConfigurationManager.AppSettings["ENABLE_SSL"] != null
                //    && ConfigurationManager.AppSettings["ENABLE_SSL"] == "true");
                return false;
            }
        }


        private void GetFinalEmailBodyAndSubjectForUser(EmailTokens emailSimpleToken, PetaPoco.Database context)
        {
            
            EmailTemplates template = GetEmailTemplateByID(emailSimpleToken.EmailTemplateID);
            emailSimpleToken.EmailSubject = template.Subject;

            if (template.Name.Equals("ForgotPassword"))
            {
                emailSimpleToken.EmailBody = GetFinalEmailBodyForUserForgotPasswordtemplate(template.Body, emailSimpleToken.EntityID);
            }

            if (template.Name.Equals("AccountVerification"))
            {
                emailSimpleToken.EmailBody = GetFinalEmailBodyForUserVerificationtemplate(template.Body, emailSimpleToken.EntityID);
            }
        }


        private EmailTemplates GetEmailTemplateByID(int templateID)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                return context.Fetch<EmailTemplates>("select * from EmailTemplates where EmailTemplateID = @0", templateID).FirstOrDefault();
            }
        }

        private string GetFinalEmailBodyForUserForgotPasswordtemplate(string body, int userId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                Users user = context.Fetch<Users>("select * from users where userid = @0", userId).FirstOrDefault();
                body = body.Replace("<%ForgottenPasswordLink%>", string.Format("{2}/account/ForgotPassword/{0}/{1}", user.UserID.ToString(), user.UserGUID, ApplicationConstants.Akhbaar_PP_URL));
                body = body.Replace("<%FirstName%>", user.FirstName);
                body = body.Replace("<%ToEmail%>", user.Email);
                return body;
            }
        }

        private string GetFinalEmailBodyForUserVerificationtemplate(string body, int userId)
        {
            using (PetaPoco.Database context = DataContextHelper.GetCPDataContext())
            {
                Users user = context.Fetch<Users>("select * from users where userid = @0", userId).FirstOrDefault();
                body = body.Replace("<%VerificationLink%>", string.Format("{2}/account/UserVerification/{0}/{1}", user.UserID.ToString(), user.UserGUID, ApplicationConstants.Akhbaar_PP_URL));
                body = body.Replace("<%FirstName%>", user.FirstName);
                body = body.Replace("<%ToEmail%>", user.Email);
                return body;
            }
        }

        

        MailMessage CreateMailMessage(MailAddress from, MailAddressCollection to, MailAddressCollection cc, MailAddressCollection bcc, bool isHtml, string subject, string body)
        {
            MailMessage mm = new MailMessage();

            mm.From = from;

            foreach (var ma in to)
            {
                if (emailOkForQA(ma.Address))
                {
                    mm.To.Add(ma);
                }
            }

            if (cc != null)
            {
                foreach (var ma in cc)
                {
                    if (emailOkForQA(ma.Address))
                    {
                        mm.CC.Add(ma);
                    }
                }
            }

            if (bcc != null)
            {
                foreach (var ma in bcc)
                {
                    if (emailOkForQA(ma.Address))
                    {
                        mm.Bcc.Add(ma);
                    }
                }
            }

            mm.IsBodyHtml = isHtml;

            mm.Subject = subject;
            //mm.SubjectEncoding = Encoding.UTF8;
            mm.SubjectEncoding = Encoding.GetEncoding("ISO-8859-6"); // for Quoted Printable


            mm.Body = body;
            mm.BodyEncoding = Encoding.UTF8;

            return (mm);
        }

        #endregion
    }
}
public delegate void EmailSentDlgt(string toEmailAddress, string emailSubject, DateTime time);
public delegate void EmailSendingFailedDlgt(string toEmailAddress, string emailSubject, string errorMessage);

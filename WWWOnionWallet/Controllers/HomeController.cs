using Google.Authenticator;
using OnionWallet.Blockchain;
using OnionWallet.Data;
using OnionWallet.Framework.Email;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WWWOnionWallet.BaseClasses;
using WWWOnionWallet.Models;

namespace WWWOnionWallet.Controllers
{
    public class HomeController : BaseController
    {

        #region Login / Register / Logout
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (this.CurrentUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dancefloor");
            }

            IndexModel data = new IndexModel();

            if (TempData["LoginEmail"] != null)
            {
                data.LoginEmail = TempData["LoginEmail"].ToString();
            }

            if (TempData["RegisterEmail"] != null)
            {
                data.RegisterEmail = TempData["RegisterEmail"].ToString();
                TempData["Register"] = true;
            }

            return View(data);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(IndexModel data)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            OnionWalletEntities entities = new OnionWalletEntities();

            OnionUser user = entities.OnionUsers.FirstOrDefault(x => x.Email == data.LoginEmail);

            if (user != null && user.CheckPassword(data.LoginPassword))
            {
                if (!user.IsEmailConfirmed)
                {
                    TempData["ErrorMessage"] = "Please confirm email before login.";
                    TempData["LoginEmail"] = data.LoginEmail;
                    return RedirectToAction("Index");
                }

                if (user.TwoFactorGUID.HasValue)
                {
                    TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                    if (string.IsNullOrEmpty(data.TwoFactorAuthentication) || !tfa.ValidateTwoFactorPIN(user.TwoFactorGUID.ToString(), data.TwoFactorAuthentication.Replace(" ", "")))
                    {
                        TempData["ErrorMessage"] = "2FA Code not correct.";
                        TempData["LoginEmail"] = data.LoginEmail;
                        return RedirectToAction("Index");
                    }
                }

                if (SignIn(user))
                {
                    return Redirect(GetRedirectUrl(data.ReturnUrl));
                }
            }

            // Authentication failed, shouldn't get here.
            TempData["ErrorMessage"] = "Invalid email or password";
            TempData["LoginEmail"] = data.LoginEmail;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(IndexModel data)
        {
            if (!ModelState.IsValid)
            {
                return View(data);
            }

            if (data.RegisterPassword != data.RegisterRepeatPassword)
            {
                TempData["ErrorMessage"] = "Passwords do not match.";
                TempData["RegisterEmail"] = data.RegisterEmail;
                return RedirectToAction("Index");
            }

            if (!data.RegisterAcceptTOS)
            {
                TempData["ErrorMessage"] = "Please accept terms of service.";
                TempData["RegisterEmail"] = data.RegisterEmail;
                return RedirectToAction("Index");
            }

            OnionWalletEntities entities = new OnionWalletEntities();

            OnionUser existing = entities.OnionUsers.FirstOrDefault(x => x.Email == data.RegisterEmail.ToLower());
            if (existing != null)
            {
                TempData["ErrorMessage"] = "Email already exists!";
                TempData["RegisterEmail"] = data.RegisterEmail;
                return RedirectToAction("Index");
            }

            OnionUser user = new OnionUser();
            user.InitGUIDs();

            user.Email = data.RegisterEmail;
            user.SetPassword(data.RegisterPassword);
            user.IsMailing = data.RegisterIsMailing;
            user.OnionAddress = "gugus";
            user.IsActive = true;
            user.CreateDate = DateTime.Now;
            
            entities.OnionUsers.Add(user);
            entities.SaveChanges();

            string subject = "OnionWallet Email confirmation";
            string body = "Hi" + Environment.NewLine + Environment.NewLine + "You have successfully created your Web OnionWallet on onionwallet.ch!" + Environment.NewLine + Environment.NewLine;
            body = body + "Please click the link below to activate your wallet:" + Environment.NewLine + Environment.NewLine;
            body = body + ConfigurationManager.AppSettings["BaseURL"].ToString() + "/mailconfirmation/" + user.EmailConfirmationGUID.ToString() + Environment.NewLine + Environment.NewLine;
            body = body + "Thanks and enjoy the Onion Party!";

            new Thread(() => 
            {
                OnionWalletEntities threadEntities = new OnionWalletEntities();

                try
                {
                    OnionUser threadUser = threadEntities.OnionUsers.FirstOrDefault(x => x.GUID == user.GUID);

                    OnionHandler onionHandler = new OnionHandler();
                    threadUser.OnionAddress = onionHandler.CreateAccount(user.GUID.ToString());
                    threadEntities.SaveChanges();
                }
                catch (Exception ex)
                {
                    Log log = new Log();
                    log.CreateDate = DateTime.Now;
                    log.Level = 1;
                    log.Message = ex.Message;
                    log.Type = (int)LogTypeEnum.Error;
                    log.UserID = 0;
                    threadEntities.Logs.Add(log);
                    threadEntities.SaveChanges();
                }
            }).Start();

            new Thread(() =>
            {
                try
                {
                    GmailHandler.SendMail(user.Email, subject, body);
                }
                catch (Exception ex)
                {
                    OnionWalletEntities threadEntities = new OnionWalletEntities();
                    Log log = new Log();
                    log.CreateDate = DateTime.Now;
                    log.Level = 1;
                    log.Message = ex.Message;
                    log.Type = (int)LogTypeEnum.Error;
                    log.UserID = 0;
                    threadEntities.Logs.Add(log);
                    threadEntities.SaveChanges();
                }
            }).Start();

            TempData["SuccessMessage"] = "Party ticket booked! Please click link in confirmation email and log in to access your wallet. Check your spam folder, if you can't find the email.";

            return RedirectToAction("Index");
        }

        [Route("logout")]
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("index", "home");
        }

        [AllowAnonymous]
        [Route("mailconfirmation/{guid}")]
        public ActionResult MailConfirmation(Guid? guid)
        {
            if (guid.HasValue)
            {
                OnionWalletEntities entities = new OnionWalletEntities();
                OnionUser onionUser = entities.OnionUsers.FirstOrDefault(x => x.EmailConfirmationGUID == guid.Value);

                if (onionUser != null)
                {
                    onionUser.IsEmailConfirmed = true;
                    entities.SaveChanges();
                    TempData["SuccessMessage"] = "Email confirmed, enjoy the party!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Account not found!";

                }
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Main Page
        [Route("dancefloor")]
        public ActionResult Dancefloor()
        {
            DancefloorModel data = new DancefloorModel();
            data.Load(this.CurrentUser.AccountName, this.CurrentUser.OnionAddress);

            return View(data);
        }
        #endregion

        #region Account
        [Route("account")]
        public ActionResult Account()
        {
            AccountModel data = new Models.AccountModel();
            data.Load(Guid.Parse(this.CurrentUser.AccountName));

            return View(data);
        }
        #endregion

        #region Static pages
        [AllowAnonymous]
        [Route("tos")]
        public ActionResult TermsOfService()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("privacy")]
        public ActionResult PrivacyPolicy()
        {
            ViewBag.Email = ConfigurationManager.AppSettings["SiteEmail"].ToString();
            return View();
        }

        #endregion

        #region Helper
        private bool SignIn(OnionUser user)
        {
            new Thread(() =>
            {
                OnionWalletEntities threadEntities = new OnionWalletEntities();
                VisitorLog logEntry = new VisitorLog();

                if (user.DoLogIpAddresses)
                {
                    logEntry.IpAddress = Request.UserHostAddress;
                }

                logEntry.CreateDate = DateTime.Now;
                logEntry.OnionUserID = user.OnionUserID;
                threadEntities.VisitorLogs.Add(logEntry);
                threadEntities.SaveChanges();
            }).Start();

            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, user.GUID.ToString()),
                new Claim("AccountName", user.GUID.ToString()),
                new Claim("Email", user.Email),
                new Claim("OnionAddress", user.OnionAddress)
            },
                "ApplicationCookie");

            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignIn(identity);

            return true;
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("dancefloor", "home");
            }

            return returnUrl;
        }
        #endregion

    }
}
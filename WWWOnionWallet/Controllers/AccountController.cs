using Google.Authenticator;
using OnionWallet.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WWWOnionWallet.BaseClasses;

namespace WWWOnionWallet.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        [HttpPost]
        public ActionResult StoreIP(string value)
        {
            Guid accountGuid = Guid.Parse(this.CurrentUser.AccountName);
            OnionWalletEntities entities = new OnionWalletEntities();
            OnionUser user = entities.OnionUsers.FirstOrDefault(x => x.GUID == accountGuid);

            if (user == null)
            {
                TempData["ErrorMessage"] = "A general error occured. Please contact support at " + ConfigurationManager.AppSettings["SiteEmail"].ToString() + ".";
            }
            else
            {
                user.DoLogIpAddresses = (value == "on");
                entities.SaveChanges();
                TempData["SuccessMessage"] = "Log IP settings updated.";
            }

            return RedirectToAction("Account", "Home");
        }

        [HttpPost]
        public ActionResult ChangePassword(string currentpassword, string newpassword, string retypedpassword)
        {
            if (string.IsNullOrEmpty(currentpassword) || string.IsNullOrEmpty(currentpassword) || string.IsNullOrEmpty(currentpassword))
            {
                TempData["ErrorMessage"] = "All password fields must be filled to change the password.";
            }
            else
            {
                Guid accountGuid = Guid.Parse(this.CurrentUser.AccountName);
                OnionWalletEntities entities = new OnionWalletEntities();
                OnionUser user = entities.OnionUsers.FirstOrDefault(x => x.GUID == accountGuid);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "A general error occured. Please contact support at " + ConfigurationManager.AppSettings["SiteEmail"].ToString() + ".";
                }
                else
                {
                    if (!user.CheckPassword(currentpassword))
                    {
                        TempData["ErrorMessage"] = "Current password does not match.";
                    }
                    else if (newpassword != retypedpassword)
                    {
                        TempData["ErrorMessage"] = "New passwords are not the same.";
                    }
                    else
                    {
                        user.SetPassword(newpassword);
                        entities.SaveChanges();
                        TempData["SuccessMessage"] = "Password updated.";
                    }
                }
            }

            return RedirectToAction("Account", "Home");
        }

        public ActionResult EnableTwoFactorAuthentication()
        {
            Guid accountGuid = Guid.Parse(this.CurrentUser.AccountName);
            OnionWalletEntities entities = new OnionWalletEntities();
            OnionUser user = entities.OnionUsers.FirstOrDefault(x => x.GUID == accountGuid);

            if (user == null)
            {
                TempData["ErrorMessage"] = "A general error occured. Please contact support at " + ConfigurationManager.AppSettings["SiteEmail"].ToString() + ".";
                return RedirectToAction("Account", "Home");
            }
            else if (user.TwoFactorGUID.HasValue)
            {
                TempData["ErrorMessage"] = "Two Factor Authentication already enabled!";
                return RedirectToAction("Account", "Home");
            }
            else
            {
                Guid secret = Guid.NewGuid();
                Session["secret"] = secret;

                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                var setupInfo = tfa.GenerateSetupCode(ConfigurationManager.AppSettings["SiteName"].ToString(), user.Email, secret.ToString(), 300, 300);

                ViewBag.KeyImage = setupInfo.QrCodeSetupImageUrl;
                ViewBag.KeyText = setupInfo.ManualEntryKey;
            }

            return View();
        }

        [HttpPost]
        public ActionResult EnableTwoFactorAuthentication(string code)
        {
            Guid secret = Guid.Empty;
            if (Session["secret"] != null)
            {
                if (Guid.TryParse(Session["secret"].ToString(), out secret))
                {
                    Guid accountGuid = Guid.Parse(this.CurrentUser.AccountName);
                    OnionWalletEntities entities = new OnionWalletEntities();
                    OnionUser user = entities.OnionUsers.FirstOrDefault(x => x.GUID == accountGuid);

                    if (user == null || string.IsNullOrEmpty(code))
                    {
                        TempData["ErrorMessage"] = "A general error occured. Please contact support at " + ConfigurationManager.AppSettings["SiteEmail"].ToString() + ".";
                    }
                    else
                    {
                        TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                        if (tfa.ValidateTwoFactorPIN(secret.ToString(), code.Replace(" ", "")))
                        {
                            user.TwoFactorGUID = secret;
                            entities.SaveChanges();
                            TempData["SuccessMessage"] = "Two Factor Authentication enabled.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Wrong code. Authentication failed.";
                        }
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "A general error occured. Please contact support at " + ConfigurationManager.AppSettings["SiteEmail"].ToString() + ".";
            }

            return RedirectToAction("Account", "Home");
        }

        public ActionResult DisableTwoFactorAuthentication()
        {
            Guid accountGuid = Guid.Parse(this.CurrentUser.AccountName);
            OnionWalletEntities entities = new OnionWalletEntities();
            OnionUser user = entities.OnionUsers.FirstOrDefault(x => x.GUID == accountGuid);

            if (user == null)
            {
                TempData["ErrorMessage"] = "A general error occured. Please contact support at " + ConfigurationManager.AppSettings["SiteEmail"].ToString() + ".";
            }
            else
            {
                user.TwoFactorGUID = null;
                entities.SaveChanges();
                TempData["SuccessMessage"] = "Two Factor Authentication is disabled.";
            }

            return RedirectToAction("Account", "Home");
        }
    }
}
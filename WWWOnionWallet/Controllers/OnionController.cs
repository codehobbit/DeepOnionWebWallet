using OnionWallet.Blockchain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WWWOnionWallet.BaseClasses;
using WWWOnionWallet.Helper;

namespace WWWOnionWallet.Controllers
{
    public class OnionController : BaseController
    {
        [HttpPost]
        public ActionResult Send(string address, string amount)
        {
            decimal amountParsed = 0;

            if (string.IsNullOrEmpty(this.CurrentUser.AccountName))
            {
                TempData["ErrorMessage"] = "Authentication Error!";
                return RedirectToAction("Dancefloor", "Home");
            }

            if (string.IsNullOrEmpty(address))
            {
                TempData["ErrorMessage"] = "Did you enter a receiver address?";
                return RedirectToAction("Dancefloor", "Home");
            }

            if (string.IsNullOrEmpty(amount))
            {
                TempData["ErrorMessage"] = "Did you enter an amount?";
                return RedirectToAction("Dancefloor", "Home");
            }

            if (!decimal.TryParse(amount, out amountParsed))
            {
                amount = amount.Replace(".", ",");
                if (!decimal.TryParse(amount, out amountParsed))
                {
                    TempData["ErrorMessage"] = "Did you enter a correct amount?";
                    return RedirectToAction("Dancefloor", "Home");
                }
            }

            OnionHandler handler = new OnionHandler();

            decimal amountSpendable = handler.GetAccountBalance(this.CurrentUser.AccountName);
            // TODO Calculate actual fees
            amountSpendable = (amountSpendable > (decimal)0.001) ? amountSpendable - (decimal)0.001 : 0;

            if (amountParsed > amountSpendable)
            {
                TempData["ErrorMessage"] = "Insufficient funds! You cannot send more than " + amountSpendable + " Onions!";
                return RedirectToAction("Dancefloor", "Home");
            }

            try
            {
                string transactionID = handler.SendOnions(this.CurrentUser.AccountName, address, amountParsed);
                TempData["SuccessMessage"] = "Onions successfully sent!";
            }
            catch (Exception ex)
            {
                Logger.Error("OnionController->Send: " + ex.Message);
                TempData["ErrorMessage"] = "Oops, something went wrong! Please try again or contact support.";
            }

            return RedirectToAction("Dancefloor", "Home");
        }

        public ActionResult Donation(decimal? amount)
        {
            if (string.IsNullOrEmpty(this.CurrentUser.AccountName) || !amount.HasValue && amount.Value > 0)
            {
                TempData["ErrorMessage"] = "Please enter a valid donation amount.";
                return RedirectToAction("Dancefloor", "Home");
            }

            OnionHandler handler = new OnionHandler();

            decimal amountSpendable = handler.GetAccountBalance(this.CurrentUser.AccountName);
            
            if (amount.Value > amountSpendable)
            {
                TempData["ErrorMessage"] = "Insufficient funds! You cannot donate more than " + amountSpendable + " Onions!";
                return RedirectToAction("Dancefloor", "Home");
            }

            try
            {
                if (handler.Move(this.CurrentUser.AccountName, ConfigurationManager.AppSettings["DonationAccountName"].ToString(), amount.Value))
                {
                    TempData["SuccessMessage"] = "We received your donation of " + amount.Value + " Onions. Thank you so much!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ooops, something went wrong. Thanks for the good intentions.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("OnionController->Donation: " + ex.Message);
                TempData["ErrorMessage"] = "Ooops, something went wrong. Thanks for the good intentions.";
            }

            return RedirectToAction("Dancefloor", "Home");
        }
    }
}
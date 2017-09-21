using OnionWallet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WWWOnionWallet.Models
{
    public class AccountModel
    {

        #region Properties

        public DateTime SignupDate { get; set; }

        public Guid PasswordRecovery { get; set; }

        public string Email { get; set; }

        public Dictionary<DateTime, string> LastLogins { get; set; }

        public bool DoLogIpAddresses { get; set; }

        public Guid? TwoFactorGUID { get; set; }

        #endregion

        #region Constructor

        public AccountModel()
        {
        }

        #endregion

        #region Public Functions

        public bool Load(Guid accountName)
        {
            OnionWalletEntities entities = new OnionWalletEntities();
            OnionUser user = entities.OnionUsers.FirstOrDefault(x => x.GUID == accountName);

            if (user != null)
            {
                this.PasswordRecovery = user.RecoveryGUID;
                this.Email = user.Email;
                this.DoLogIpAddresses = user.DoLogIpAddresses;
                this.SignupDate = user.CreateDate;
                this.TwoFactorGUID = user.TwoFactorGUID;
                this.LastLogins = entities.VisitorLogs.Where(x => x.OnionUserID == user.OnionUserID).OrderByDescending(x => x.CreateDate).Take(20).ToDictionary(x => x.CreateDate, y => y.IpAddress);

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

    }
}
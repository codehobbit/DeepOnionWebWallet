using OnionWallet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WWWOnionWallet.Helper
{
    public static class Logger
    {

        #region Public Functions

        public static void Error(string message)
        {
            OnionWalletEntities entities = new OnionWalletEntities();

            Log log = new Log();
            log.CreateDate = DateTime.Now;
            log.Level = 3;
            log.Type = (int)LogTypeEnum.Error;

            entities.Logs.Add(log);
            entities.SaveChanges();
        }

        #endregion

    }
}
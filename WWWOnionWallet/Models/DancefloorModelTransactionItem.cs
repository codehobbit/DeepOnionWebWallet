using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WWWOnionWallet.Models
{
    public class DancefloorModelTransactionItem
    {

        #region Properties

        public string Type { get; set; }

        public DateTime Date { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public decimal Amount { get; set; }

        public string TransactionID { get; set; }

        #endregion

        #region Constructor

        public DancefloorModelTransactionItem()
        {

        }
        
        #endregion

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionWallet.Blockchain.JsonResponses
{
    public class ListTransactionsResponse
    {

        #region Properties

        public string Account { get; set; }

        public string Address { get; set; }

        public string Category { get; set; }

        public decimal Amount { get; set; }

        public int Confirmations { get; set; }

        public bool Generated { get; set; }

        public string BlockHash { get; set; }

        public int BlockIndex { get; set; }

        public double BlockTime { get; set; }

        public string TxId { get; set; }

        public double Time { get; set; }

        public double TimeReceived { get; set; }

        public bool Pending { get { return (Confirmations < 5 && Category == "receive") ; } }

        #endregion

        #region Constructor

        public ListTransactionsResponse()
        {

        }

        #endregion

    }
}

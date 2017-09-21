using OnionWallet.Blockchain;
using OnionWallet.Blockchain.JsonResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WWWOnionWallet.Helper;

namespace WWWOnionWallet.Models
{
    public class DancefloorModel
    {

        #region Private Members

        private string _accountName = "";

        #endregion

        #region Properties

        public string OnionAddress { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal DollarValue { get; set; }

        public decimal MaxAmountSpendable { get; set; }

        public decimal Fees { get; set; }

        public List<ListTransactionsResponse> Transactions { get; set; }

        #endregion

        #region Constructor

        public DancefloorModel()
        {

        }

        #endregion

        #region Public Functions

        public void Load(string accountName, string onionAddress)
        {
            _accountName = accountName;
            this.OnionAddress = onionAddress;

            OnionHandler handler = new OnionHandler();

            // seems that rpc getbalance delivers wrong amounts.. Check explorer.deeponion.org instead.
            //this.TotalAmount = handler.GetAccountBalance(_accountName);
            this.TotalAmount = OnionPriceChecker.GetAddressBalance(onionAddress);
            
            this.DollarValue = OnionPriceChecker.GetOnionUSD() * this.TotalAmount;

            this.Fees = (decimal)0.001;
            this.MaxAmountSpendable = (this.TotalAmount > this.Fees) ? this.TotalAmount - this.Fees : 0;
            
            this.Transactions = handler.ListTransactions(_accountName, 20, 0);

            this.Transactions = this.Transactions.OrderByDescending(x => x.Time).ToList();

            foreach (var item in this.Transactions)
            {
                if (item.Category == "send")
                {
                    // TODO: Calculate the actual fees
                    item.Amount = item.Amount - (decimal)0.001;
                }
            }
        }

        #endregion
    }
}
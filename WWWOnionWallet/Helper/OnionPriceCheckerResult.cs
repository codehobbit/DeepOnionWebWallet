using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WWWOnionWallet.Helper
{
    public class OnionPriceCheckerResult
    {

        #region Properties

        public string ID { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public int Rank { get; set; }

        public decimal Price_USD { get; set; }

        public decimal Price_BTC { get; set; }

        #endregion 

        #region Constructor

        public OnionPriceCheckerResult()
        {

        }

        #endregion

    }
}
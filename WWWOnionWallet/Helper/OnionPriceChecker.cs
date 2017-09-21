using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WWWOnionWallet.Helper
{
    public static class OnionPriceChecker
    {

        #region Public Functions

        public static decimal GetOnionUSD()
        {
            string json = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.coinmarketcap.com/v1/ticker/deeponion/");
            request.ContentType = "application/json; charset=utf-8";
            OnionPriceCheckerResult[] result;

            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    json = reader.ReadToEnd();
                }

                result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<OnionPriceCheckerResult[]>(json);

                return result[0].Price_USD;
            }
            catch (Exception ex)
            {
                Logger.Error("OnionPriceChecker: " + ex.Message);
            }

            return 0;
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionWallet.Blockchain.RPC
{
    public enum RpcMethodsEnum
    {
        /// <summary>
        /// walletpassphrase <passphrase> <timeout> [stakingonly]
        /// </summary>
        WalletPassPhrase = 0,

        /// <summary>
        /// getbalance [account] [minconf=1]
        /// </summary>
        GetBalance = 1,

        /// <summary>
        /// listtransactions [account] [count=10] [from=0]
        /// </summary>
        ListTransactions = 2,

        /// <summary>
        /// sendfrom <fromaccount> <toDeepOnionaddress> <amount> [minconf=1] [comment]
        /// </summary>
        SendFrom = 3,

        /// <summary>
        /// move <fromaccount> <toaccount> <amount> [minconf=1] [comment]
        /// </summary>
        Move = 4,

        /// <summary>
        /// getaccountaddress <account>
        /// </summary>
        GetAccountAddress = 5,

        WalletLock = 6,
    }
}

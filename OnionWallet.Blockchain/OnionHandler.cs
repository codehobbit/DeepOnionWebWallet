using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using OnionWallet.Blockchain.RPC;
using OnionWallet.Blockchain.JsonResponses;

namespace OnionWallet.Blockchain
{
    public class OnionHandler
    {

        private const int MIN_TX_FEE = 100000;
        private const int MAX_BLOCK_SIZE = 1500000;
        private const int MAX_BLOCK_SIZE_GEN = MAX_BLOCK_SIZE / 2;
        private const Int64 COIN = 100000000;
        private const Int64 MAX_MONEY = 25000000 * COIN;

        private string _passphrase;

        
        private RpcHandler _rpcHandler = null;

        public OnionHandler()
        {
            _rpcHandler = new RPC.RpcHandler();
            _passphrase = ConfigurationManager.AppSettings["OnionWalletPassword"].ToString();
        }

        public decimal GetAccountBalance(string account)
        {
            return _rpcHandler.MakeRequest<decimal>(RpcMethodsEnum.GetBalance, account);
        }

        public List<ListTransactionsResponse> ListTransactions(string account, int count, int from)
        {
            return _rpcHandler.MakeRequest<List<ListTransactionsResponse>>(RpcMethodsEnum.ListTransactions, account, count, from);
        }

        public string SendOnions(string account, string receiverAddress, decimal amount)
        {
            UnlockWallet(false);

            string transactionID = _rpcHandler.MakeRequest<string>(RpcMethodsEnum.SendFrom, account, receiverAddress, amount);

            UnlockWallet(true);

            return transactionID;
        }

        public bool Move(string fromAccount, string toAccount, decimal amount)
        {
            UnlockWallet(false);

            bool result = _rpcHandler.MakeRequest<bool>(RpcMethodsEnum.Move, fromAccount, toAccount, amount);
            
            UnlockWallet(true);

            return result;
        }


        private void UnlockWallet(bool stakingOnly)
        {
            LockWallet();

            try
            {
                _rpcHandler.MakeRequest<string>(RpcMethodsEnum.WalletPassPhrase, _passphrase, 10, stakingOnly);
            }
            catch (Exception)
            {
                // It will fail if wallet is already unlocked, we should not get here.
            }
        }

        private void LockWallet()
        {
            try
            {
                _rpcHandler.MakeRequest<string>(RpcMethodsEnum.WalletLock);
            }
            catch (Exception)
            {
                // It will fail if wallet is already locked, no need to worry.
            }
        }

        /// <summary>
        /// Creates an Account in Wallet
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns>OnionAddress</returns>
        public string CreateAccount(string accountName)
        {
            return _rpcHandler.MakeRequest<string>(RpcMethodsEnum.GetAccountAddress, accountName);
        }

        #region Helper

        // Not yet working, to be done.
        //private decimal GetMinTransactionFee(int blockSize, int bytes, decimal amount)
        //{
        //    int newBlockSize = blockSize + bytes;
        //    decimal result = (1 + bytes / 1000) * MIN_TX_FEE;

        //    // To limit dust spam, require MIN_TX_FEE/MIN_RELAY_TX_FEE if any output is less than 0.01
        //    if (amount <= (decimal)0.01)
        //    {
        //        result = MIN_TX_FEE;
        //    }

        //    // Raise the price as the block approaches full
        //    if (blockSize != 1 && newBlockSize >= MAX_BLOCK_SIZE_GEN / 2)
        //    {
        //        if (newBlockSize >= MAX_BLOCK_SIZE_GEN)
        //        {
        //            return MAX_MONEY;
        //        }

        //        result *= MAX_BLOCK_SIZE_GEN / (MAX_BLOCK_SIZE_GEN - newBlockSize);
        //    }

        //    if (!(result >= 0 && result <= MAX_MONEY))
        //    {
        //        result = MAX_MONEY;
        //    }

        //    return result;
        //}

        #endregion
    }
}

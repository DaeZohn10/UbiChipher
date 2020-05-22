﻿using BitcoinLib.Requests.CreateRawTransaction;
using BitcoinLib.Services.Coins.Base;
using BitcoinLib.Services.Coins.Bitcoin;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbiChipher.Data;

namespace UbiChipher.Infrastructure.Blockchain
{
    public class BlockchainEnrollmentProcessor
    {
        private readonly ICoinService CoinService = new BitcoinService(useTestnet: true);

        public void Enroll(Enrollment enrollment)
        {
            var userAdd = enrollment.Claims[0].PubKey; // Obviously bad code, it's late and I am just testing something.

            var transactionToScanForClaim = CoinService.ListTransactions();
            var txid = transactionToScanForClaim.Last().TxId; // But it may not actually be the last one

            var tx = CoinService.GetTxOut(txid, 0);
            var txx = CoinService.GetTransaction(txid);
            var raw = CoinService.GetRawTransaction(txid, 1);

            var txs = CoinService.ListUnspent();

            // BitcoinLib to NBitcoin 
            //Coin(uint256 fromTxHash, uint fromOutputIndex, Money amount, Script scriptPubKey);
            this.unspentCoins = txs.Select(x => new Coin(uint256.Parse(x.TxId), (uint)x.Vout, new Money(x.Amount,MoneyUnit.BTC), new Script(x.ScriptPubKey))).ToList();

            var createRawTransactionInput = new CreateRawTransactionInput();
            var createRawTransactionRequest = new CreateRawTransactionRequest();

            //// BitcoinLib to NBitcoin
            //List<OutPoint> outPoints =  txx.Select(x => new OutPoint(uint256.Parse(txid),1)).

            //CoinService.CreateRawTransaction()



        }

        private void SendBTC(object obj)
        {
            if (SendAmount > balance) return;
            //List<Coin> toSpend = MinimumCoinsToCoverTransaction(); // Bitcoin doesn't allow spending just the inputs you need from a previous transaction?
            List<Coin> toSpend = this.unspentCoins;

            // For the payment you will need to reference this outpoint in the transaction. You create a transaction as follows:
            var transaction = Transaction.Create(network);
            foreach (var item in toSpend)
            {
                transaction.Inputs.Add(item.Outpoint);
            }


            // To where?
            var hallOfTheMakersAddress = BitcoinAddress.Create(ToAddress, network);


            // How much?
            Money minerFee = new Money(this.Fee, MoneyUnit.BTC);
            Money amoundToSpend = new Money(this.SendAmount, MoneyUnit.BTC);

            transaction.Outputs.Add(amoundToSpend, hallOfTheMakersAddress.ScriptPubKey);
            // Send the change back
            transaction.Outputs
                .Add(new Money(toSpend.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC)) - amoundToSpend.ToDecimal(MoneyUnit.BTC) - minerFee.ToDecimal(MoneyUnit.BTC), MoneyUnit.BTC),
                this.wallets[0].ScriptPubKey);

            // message
            //var message = "Long live NBitcoin and its makers!";
            var bytes = Encoding.UTF8.GetBytes(this.Message);
            transaction.Outputs.Add(Money.Zero, TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes));

            // sign it
            // Get it from the public address
            //transaction.Inputs[0].ScriptSig = address.ScriptPubKey;
            // OR we can also use the private key 
            //transaction.Inputs[0].ScriptSig = bitcoinPrivateKey.ScriptPubKey;

            foreach (var item in transaction.Inputs)
            {
                item.ScriptSig = this.wallets[0].ScriptPubKey;
            }


            transaction.Sign(this.wallets[0].Key.GetBitcoinSecret(network), toSpend.Select(x => (ICoin)x).AsEnumerable());

            transaction // ToHex, ToBytes, ToString


            //// Propagate your transactions
            //// With QBitNinja
            //BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;

            //if (!broadcastResponse.Success)
            //{
            //    MessageBox.Show("ErrorCode: " + broadcastResponse.Error.ErrorCode);

            //    MessageBox.Show("Error message: " + broadcastResponse.Error.Reason);
            //}
            //else
            //{
            //    MessageBox.Show("Success! You can check out the hash of the transaciton in any block explorer:");
            //    MessageBox.Show(transaction.GetHash().ToString());
            //}

            //TransactionBuilderTransaction(toSpend);

        }

        //private string walletImportFormat;
        //public string WalletImportFormat { get { return walletImportFormat; } set { walletImportFormat = value;/* OnPropertyChanged();*/ } }

        private string toAddress;
        public string ToAddress { get { return toAddress; } set { toAddress = value; /*OnPropertyChanged();*/ } }//=> wallets[1].PublicKey; //"mnDJL3XEYNKCNwYoJg3RNK56ikTp9nQCVU";
        public string Message { get; set; } = "Stuur Abie al my Bitcoins.";

        //string historyDebug = string.Empty;
        //public string HistoryDebug { get { return historyDebug; } set { historyDebug = value; OnPropertyChanged(); } }

        public decimal SendAmount { get; set; } = 0.005m;
        public decimal Fee { get; private set; } = 0.00000300m;

        decimal balance = 0m;
        //public decimal Balance { get { return balance; } set { balance = value; OnPropertyChanged(); } }

        Network network = null;

        //QBitNinjaClient client = null;
        
        List<Coin> unspentCoins = new List<Coin>();
        
        //BalanceModel balanceModel = null;


    }
}

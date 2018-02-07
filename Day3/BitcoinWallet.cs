using System;
using System.Globalization;
using System.Text;
using HBitcoin.KeyManagement;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace Create_Bitcoin_Wallet
{
    class Startup
    {
        private const string EndCommand = "exit";
        private const string CreateCommand = "create";
        private const string RecoverCommand = "recover";
        private const string ReceiveCommand = "receive";
        private const string BalanceCommand = "balance";
        private const string GetHistoryCommand = "history";
        private const string SendCommand = "send";

        private const string WalletFilePath = @"..\Wallets\";
        private const string WalletFileExtension = ".json";
        private const string RecoveryDateFormat = "yyyy-MM-dd";

        private static readonly Network CurrentNetwork = Network.TestNet;

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("Enter operation [\"Create\", \"Recover\", \"Balance\", \"History\", \"Receive\", \"Send\", \"Exit\"]: ");

                string input = GetInput();

                if (input == EndCommand)
                {
                    break;
                }

                switch (input)
                {
                    case CreateCommand:
                        {
                            CreateWallet();
                            break;
                        }

                    case RecoverCommand:
                        {
                            Recover();
                            break;
                        }

                    case ReceiveCommand:
                        {
                            Receive();
                            break;
                        }

                    case BalanceCommand:
                        {
                            Balance();
                            break;
                        }

                    case GetHistoryCommand:
                        {
                            GetHistory();
                            break;
                        }

                    case SendCommand:
                        {
                            Send();
                            break;
                        }

                    default:
                        {
                            Console.WriteLine("Wrong command. Please try again!");
                            continue;
                        }
                }
            }
        }

        private static void Send()
        {
            Console.WriteLine("Wallet name:");
            string walletName = GetInput();

            Console.WriteLine("Password:");
            string pass = GetInput();

            Console.WriteLine("Enter wallet address:");
            string address = Console.ReadLine();

            Console.WriteLine("Select outpoint(transaction ID):");
            string outPoint = GetInput();

            BitcoinExtKey privateKey = null;
            try
            {
                string walletFile = $"{WalletFilePath}{walletName}{WalletFileExtension}";

                Safe loadedSafe = Safe.Load(pass, walletFile);

                for (int i = 0; i < 10; i++)
                {
                    if (loadedSafe.GetAddress(i).ToString() == address)
                    {
                        Console.WriteLine("Enter private key:");
                        privateKey = new BitcoinExtKey(Console.ReadLine());
                        if (!privateKey.Equals(loadedSafe.FindPrivateKey(loadedSafe.GetAddress(i))))
                        {
                            Console.WriteLine("Wrong private key!");
                            return;
                        }
                        break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Wrong wallet or password");
                return;
            }

            QBitNinjaClient client = new QBitNinjaClient(CurrentNetwork);

            BalanceModel balance = client.GetBalance(BitcoinAddress.Create(address), false).Result;

            OutPoint outPointToSpend = null;

            foreach (BalanceOperation balanceOperation in balance.Operations)
            {
                foreach (ICoin balanceOperationReceivedCoin in balanceOperation.ReceivedCoins)
                {
                    string coin = balanceOperationReceivedCoin.Outpoint.ToString();

                    if (coin.Substring(0, coin.Length - 2) == outPoint)
                    {
                        outPointToSpend = balanceOperationReceivedCoin.Outpoint;
                        break;
                    }
                }

                if (outPointToSpend != null)
                {
                    break;
                }
            }

            if (outPointToSpend == null)
            {
                Console.WriteLine("Invalid out point");
                return;
            }

            var transaction = new Transaction();
            transaction.Inputs.Add(new TxIn()
            {
                PrevOut = outPointToSpend
            });

            Console.WriteLine("Enter address to send to:");
            string reciever = Console.ReadLine();

            BitcoinAddress processedRecieverAddress = BitcoinAddress.Create(reciever);

            Console.WriteLine("Enter amount to send:");
            decimal amountToSend = decimal.Parse(Console.ReadLine());
            TxOut txOut = new TxOut()
            {
                Value = new Money(amountToSend, MoneyUnit.BTC),
                ScriptPubKey = processedRecieverAddress.ScriptPubKey
            };

            Console.WriteLine("Enter amount to get back:");
            decimal amountToGetBack = decimal.Parse(Console.ReadLine());
            TxOut txOutReturn = new TxOut()
            {
                Value = new Money(amountToSend, MoneyUnit.BTC),
                ScriptPubKey = privateKey.ScriptPubKey
            };

            transaction.Outputs.Add(txOut);
            transaction.Outputs.Add(txOutReturn);

            Console.WriteLine("Enter transaction message:");
            string message = Console.ReadLine();
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            transaction.Outputs.Add(new TxOut()
            {
                Value = Money.Zero,
                ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes)
            });

            transaction.Inputs[0].ScriptSig = privateKey.ScriptPubKey;
            transaction.Sign(privateKey, false);

            BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;
            if (broadcastResponse.Success)
            {
                Console.WriteLine("Transaction send!");
            }
            else
            {
                Console.WriteLine("Failed transaction!");
            }
        }

        private static void GetHistory()
        {
            Console.WriteLine("Wallet name:");
            string walletName = GetInput();

            Console.WriteLine("Password:");
            string pass = GetInput();

            Console.WriteLine("Enter wallet address:");
            string address = Console.ReadLine();

            try
            {
                string walletFile = $"{WalletFilePath}{walletName}{WalletFileExtension}";

                Safe.Load(pass, walletFile);
            }
            catch
            {
                Console.WriteLine("Wallet with such name does not exist");
                return;
            }

            QBitNinjaClient client = new QBitNinjaClient(CurrentNetwork);

            BalanceModel balance = client.GetBalance(BitcoinAddress.Create(address), true).Result;

            string received = "-----COINS RECEIVED-----";
            Console.WriteLine(received);

            foreach (BalanceOperation balanceOperation in balance.Operations)
            {
                foreach (ICoin balanceOperationReceivedCoin in balanceOperation.ReceivedCoins)
                {
                    Money amount = balanceOperationReceivedCoin.Amount as Money;
                    Console.WriteLine($"Transaction ID: {balanceOperationReceivedCoin.Outpoint}; Received coins: {amount?.ToDecimal(MoneyUnit.BTC) ?? 0}");
                }
            }

            Console.WriteLine(new string('-', received.Length));

            BalanceModel balanceWithSpent = client.GetBalance(BitcoinAddress.Create(address), false).Result;

            string spent = "-----COINS SPENT-----";
            Console.WriteLine(spent);

            foreach (BalanceOperation balanceOperation in balanceWithSpent.Operations)
            {
                foreach (ICoin balanceOperationReceivedCoin in balanceOperation.SpentCoins)
                {
                    Money amount = balanceOperationReceivedCoin.Amount as Money;
                    Console.WriteLine($"Transaction ID: {balanceOperationReceivedCoin.Outpoint}; Received coins: {amount?.ToDecimal(MoneyUnit.BTC) ?? 0}");
                }
            }

            Console.WriteLine(new string('-', spent.Length));
        }

        private static void Balance()
        {
            Console.WriteLine("Wallet name:");
            string walletName = GetInput();

            Console.WriteLine("Password:");
            string pass = GetInput();

            Console.WriteLine("Enter wallet address:");
            string address = Console.ReadLine();

            try
            {
                string walletFile = $"{WalletFilePath}{walletName}{WalletFileExtension}";

                Safe.Load(pass, walletFile);
            }
            catch
            {
                Console.WriteLine("Wallet with such name does not exist");
                return;
            }

            QBitNinjaClient client = new QBitNinjaClient(CurrentNetwork);

            decimal totalBalance = 0M;

            BalanceModel balance = client.GetBalance(BitcoinAddress.Create(address), true).Result;

            foreach (BalanceOperation balanceOperation in balance.Operations)
            {
                foreach (ICoin balanceOperationReceivedCoin in balanceOperation.ReceivedCoins)
                {
                    Money amount = balanceOperationReceivedCoin.Amount as Money;
                    decimal currentAmount = amount?.ToDecimal(MoneyUnit.BTC) ?? 0;
                    totalBalance += currentAmount;
                }
            }

            Console.WriteLine($"Balance of wallet: {totalBalance}");
        }

        private static void Receive()
        {
            Console.WriteLine("Wallet name:");
            string walletName = GetInput();

            Console.WriteLine("Password:");
            string pass = GetInput();

            try
            {
                string walletFile = $"{WalletFilePath}{walletName}{WalletFileExtension}";

                Safe loadedWallet = Safe.Load(pass, walletFile);

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(loadedWallet.GetAddress(i));
                }
            }
            catch
            {
                Console.WriteLine("Wallet with such name does not exist");
            }
        }

        private static string GetInput()
        {
            return Console.ReadLine().Trim().ToLower();
        }

        private static void CreateWallet()
        {

            string pass = string.Empty;

            while (true)
            {
                Console.WriteLine("Enter password: ");
                pass = Console.ReadLine();

                Console.WriteLine("Confirm password: ");
                string confirmation = Console.ReadLine();

                if (pass != confirmation)
                {
                    Console.WriteLine("Passwords did not match. Try Again!");
                    continue;
                }

                break;
            }

            while (true)
            {
                try
                {
                    Console.WriteLine("Enter wallet name:");
                    string walletName = GetInput();

                    Mnemonic mnemonic;
                    string walletFile = $"{WalletFilePath}{walletName}{WalletFileExtension}";
                    Safe wallet = Safe.Create(out mnemonic, pass, walletFile, CurrentNetwork);

                    Console.WriteLine($"Successful creation. Mnemonic: {Environment.NewLine}{mnemonic}");

                    BitcoinAddress currentAddress;
                    BitcoinExtKey currentPrivateKey;
                    for (int i = 0; i < 10; i++)
                    {
                        currentAddress = wallet.GetAddress(i);
                        currentPrivateKey = wallet.FindPrivateKey(currentAddress);

                        Console.WriteLine($"Address: {currentAddress} -> Private key: {currentPrivateKey}");
                    }

                    break;
                }
                catch
                {
                    Console.WriteLine("Wallet already exists. Try different name!");
                }
            }
        }

        private static void Recover()
        {
            Console.WriteLine("Enter password:");
            string pass = GetInput();

            Console.WriteLine("Enter mnemonic phrase:");
            string mnemonicIn = GetInput();

            Console.WriteLine($"Enter date({RecoveryDateFormat}):");
            string date = GetInput();

            Mnemonic mnemonic = new Mnemonic(mnemonicIn);

            Random rand = new Random();

            string recoveredWalletFile = $"{WalletFilePath}RecoveredWalletNum{rand.Next()}{WalletFileExtension}";

            DateTime outDate;
            bool success = DateTime.TryParseExact(date, RecoveryDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out outDate);

            if (success)
            {
                Safe.Recover(mnemonic, pass, recoveredWalletFile, CurrentNetwork, outDate);
            }
            else
            {
                Safe.Recover(mnemonic, pass, recoveredWalletFile, CurrentNetwork);
            }

            Console.WriteLine($"{recoveredWalletFile} successfully recovered!");
        }
    }
}

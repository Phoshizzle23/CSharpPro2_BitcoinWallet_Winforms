using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NBitcoin;
using QBitNinja;
using QBitNinja.Client;

namespace Bitcoin_Wallet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var privatekey = new Key();

            var btcprivatekey = privatekey.GetWif(Network.Main).ToString();

            textBox1.Text = btcprivatekey;

            // generate public key from private key
            var btcpublickey = privatekey.PubKey.ToString();

            // generate btc address from our public key
            var btcaddress1 = privatekey.GetAddress(ScriptPubKeyType.Segwit, Network.Main);
            textBox2.Text = btcaddress1.ToString();

            decimal balance1 = Checkbalance(btcaddress1.ToString());
            bal1Lbl.Text = balance1.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var menmo = new Mnemonic(Wordlist.English, WordCount.Twelve);

            Mnemonictxt.Text = menmo.ToString();

            var hdroot = menmo.DeriveExtKey();

            var pkey = hdroot.Derive(new KeyPath("m/84'/0'/0'/0/0'"));

            var address = pkey.PrivateKey.PubKey.GetAddress(ScriptPubKeyType.Segwit, Network.Main);

            addresstxt.Text = address.ToString();

            decimal balance2 = Checkbalance(address.ToString());
            bal2Lbl.Text = balance2.ToString();
        }

        public decimal Checkbalance(string address)
        {
            QBitNinjaClient client = new QBitNinjaClient(Network.Main);

            var balancemodel = client.GetBalance(address, false).Result; // error here

            decimal addressbalance = 0;
            if (balancemodel.Operations.Count == 0)
            {
                addressbalance = 0;
            }
            else
            {
                var unspentcoins = new List<Coin>();
                foreach (var operation in balancemodel.Operations)
                {
                    unspentcoins.AddRange(operation.ReceivedCoins.Select(Coin => Coin as Coin));

                    addressbalance = unspentcoins.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));
                }
            }
            return addressbalance;
        }

    }

}


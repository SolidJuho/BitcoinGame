using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Data;
using System.IO;
using System.Xml.Linq;

namespace CMDGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        public List<string> GameLogs = new List<string>();
        private List<BitcoinServer> BitcoinServers = new List<BitcoinServer>();

        private BitcoinServer selectedBitcoinServer;

        //string commands = new List<String>();
        Commands cmd;
        string FirstCommand;
        string FirstAttribute;
        string oldCommand;
        string tempCommand;

        int selectedBitcoinServerID;

        XmlDocument helpDoc = new XmlDocument();

        public void AddNewLog(string newLog)
        {
            GameLogs.Add(newLog);
            string logMessage = "";
            foreach (string l in GameLogs)
            {
                logMessage += l+ Environment.NewLine;
            }
            TerminalLog.Text = logMessage;
            if(GameLogs.Count > 45)
            {
                GameLogs.RemoveAt(0);
            }
        }

        public void AddNewLog()
        {
            GameLogs.Add("");
            string logMessage = "";
            foreach (string l in GameLogs)
            {
                logMessage += l + Environment.NewLine;
            }
            TerminalLog.Text = logMessage;
            if (GameLogs.Count > 45)
            {
                GameLogs.RemoveAt(0);
            }
        }

        public void ClearLog()
        {
            GameLogs = new List<string>();
            TerminalLog.Text = "";
        }

        public void NewMachine()
        {
            BitcoinServer newServer = new BitcoinServer();
            newServer.ID = BitcoinServers.Count+1;
            BitcoinServers.Add(newServer);
            AddNewLog("You bought new server, ID: " +newServer.ID);
            selectedBitcoinServerID = newServer.ID;
            selectedBitcoinServer = newServer;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            AddNewLog("Init log system");
            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Tick += new EventHandler(timer_tick);
            //timer.Start();
            FileStream fs = new FileStream("HelpInfo.xml", FileMode.Open, FileAccess.Read);
            helpDoc.Load(fs);


            /*
            XmlNodeList helpNode = helpDoc.SelectNodes("/commands");
            for (int i = 0; i < helpNode.Count-1; i++)
            {
                AddNewLog("Found cmd from xml");
                AddNewLog(helpNode[i].Name);
            }*/
        }

        private void timer_tick(object sender, EventArgs e)
        {
            //AddNewLog("Tick, Log amount: "+GameLogs.Count.ToString());
        }
        



        private void TextBox1_Enter(object sender, EventArgs e)
        {
            SendCommand();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SendCommand();
        }


        private void SendCommand()
        {
            oldCommand = textBox1.Text;
            //Make string to status
            string[] cmdsList = textBox1.Text.ToLower().Trim().Split(' ');
            textBox1.Text = "";
            FirstCommand = cmdsList[0];
            if (cmdsList.Length > 1)
            {
                FirstAttribute = cmdsList[1];

            }
            else
            {
                FirstAttribute = "";
            }

            if(FirstCommand == "")
            {
                return;
            }

            if (FirstCommand == "status")
            {
                cmd = Commands.Status;
            }
            else if (FirstCommand == "upgrade")
            {
                cmd = Commands.Updgrade;
            }
            else if (FirstCommand == "sell")
            {
                cmd = Commands.Sell;
            }
            else if (FirstCommand == "help")
            {
                cmd = Commands.Help;
            }
            else if(FirstCommand == "clear")
            {
                cmd = Commands.Clear;
            }else if(FirstCommand == "buy")
            {
                NewMachine();
            }
            else if(FirstCommand == "transfer")
            {
                cmd = Commands.Transfer;
            }else if(FirstCommand == "wallet")
            {
                cmd = Commands.Wallet;
            }
            else if(FirstCommand == "select"||FirstCommand == "switch")
            {
                if(FirstAttribute == "")
                {
                    AddNewLog("You need to enter server ID, Type 'select list' for list of servers.");

                } else if(FirstAttribute == "list")
                {
                    if(BitcoinServers.Count == 0)
                    {
                        AddNewLog("You don't own any servers");
                    }
                    AddNewLog("List of servers: ");
                    foreach (BitcoinServer server in BitcoinServers)
                    {
                        AddNewLog("Server ID: " + server.ID);
                    }
                }
                else
                {
                    int ServerID;
                    try
                    {
                        ServerID = Int32.Parse(FirstAttribute);
                    }
                    catch (Exception)
                    {
                        AddNewLog("Not valid number");
                        AddNewLog("You need to enter valid server ID, Type 'select list' for list of servers.");
                        return;
                    }
                    try
                    {
                        SwitchServer(ServerID);
                    }
                    catch (Exception)
                    {
                        AddNewLog("Invalid ID for server");
                        throw;
                    }

                }
            }
            else
            {
                cmd = Commands.Unknown;
                AddNewLog("Unknown command: " + oldCommand);
                AddNewLog("Type 'help' for commands");
            }

            //Remove old text
            //Refactor this to own function, so it can  be called when pressed enter on textbox
            switch (cmd)
            {
                case Commands.Status:
                    if(selectedBitcoinServer == null)
                    {
                        AddNewLog("You haven't currently selected server");
                    }
                    else
                    {
                        AddNewLog();
                        AddNewLog(String.Format("STATUS: Currently selected server {0}", selectedBitcoinServerID));
                        AddNewLog(String.Format("Stored bitcoins: {0}. Bitcoins each minute: {1}", selectedBitcoinServer.GetBitcoins().ToString(), selectedBitcoinServer.BitcoinsPerMinute().ToString()));
                        AddNewLog(String.Format("Last transfer: {0}. {1} seconds ago.", selectedBitcoinServer.lastSold, selectedBitcoinServer.lastTranferAgo()));
                    }
                    break;
                case Commands.Updgrade:
                    break;
                case Commands.Sell:
                    AddNewLog(string.Format("Sold {0} bitcoins for {1}€.", PlayerStats.BitcoinsInWallet, PlayerStats.BitcoinsInWallet * Bitcoin.value));
                    PlayerStats.PlayerCash += PlayerStats.BitcoinsInWallet * Bitcoin.value;
                    PlayerStats.BitcoinsInWallet = 0;
                    break;
                case Commands.Unknown:
                    break;
                case Commands.Help:
                    if (FirstAttribute == "")
                    {
                        XmlNodeList helpRoot = helpDoc.GetElementsByTagName("Command");
                        AddNewLog("");
                        AddNewLog("List of commands available");
                        foreach (XmlNode node in helpRoot)
                        {
                            if (node.ChildNodes.Item(0).InnerText == "")
                                break;
                            AddNewLog(string.Format(" - {0}  -  {1}", node.ChildNodes.Item(0).InnerText, node.ChildNodes.Item(1).InnerText));
                        }

                    }
                    else
                    {
                        bool FoundCommand = false;
                        XmlNodeList helpRoot = helpDoc.GetElementsByTagName("Command");
                        foreach (XmlNode node in helpRoot)
                        {
                            if (node.Attributes["name"].Value == FirstAttribute)
                            {
                                FoundCommand = true;

                                AddNewLog("");
                                AddNewLog(String.Format("Advanced help for '{0}' command", FirstAttribute));
                                AddNewLog(String.Format("   Usage: {0}", node.ChildNodes.Item(0).InnerText));
                                AddNewLog(String.Format("   Example: {0}", node.ChildNodes.Item(3).InnerText));
                                AddNewLog(String.Format("   Description: {0}", node.ChildNodes.Item(2).InnerText));
                            }
                        }
                        if (!FoundCommand)
                        {
                            AddNewLog(String.Format("Command {0} not found from help docs", FirstAttribute));
                        }

                    }

                    break;

                case Commands.Clear:
                    ClearLog();

                    break;
                case Commands.Transfer:
                    if (FirstAttribute == "")
                    {
                        AddNewLog("You need to specify target.");
                    }
                    else if (FirstAttribute == "wallet")
                    {
                        if(selectedBitcoinServer == null)
                        {
                            AddNewLog("You dont have bitcoin server selected!");
                        }
                        else
                        {
                            AddNewLog(string.Format("{0} bitcoins transferred to wallet.", selectedBitcoinServer.GetBitcoins().ToString()));
                            PlayerStats.BitcoinsInWallet += selectedBitcoinServer.TransferBitcoins();

                        }
                    }
                    else
                    {
                        AddNewLog("Invalid target.");
                    }

                    break;
                case Commands.Wallet:
                    AddNewLog("Bitcoins in wallet: "+PlayerStats.BitcoinsInWallet.ToString());
                    break;
                default:
                    break;
            }

            //Clean everything for next command.
            FirstCommand = null;
            FirstAttribute = null;
            cmd = Commands.Unknown;

        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                SendCommand();
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;

                if (oldCommand != null)
                {
                    tempCommand = textBox1.Text;
                    textBox1.Text = oldCommand;
                    textBox1.SelectionStart = textBox1.Text.Length;
                    textBox1.SelectionLength = 0;
                }
            }
            else if(e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                if(tempCommand != null)
                {
                    textBox1.Text = tempCommand;
                    tempCommand = null;
                    textBox1.SelectionStart = textBox1.Text.Length;
                    textBox1.SelectionLength = 0;

                }
            }
        }

        public void SwitchServer(int ID)
        {
            selectedBitcoinServerID = ID;
            selectedBitcoinServer = BitcoinServers[ID - 1];
        }












    }




    public enum Commands
    {
        Status,
        Updgrade,
        Sell,
        Unknown,
        Help,
        Clear,
        Transfer,
        Wallet
    }


}

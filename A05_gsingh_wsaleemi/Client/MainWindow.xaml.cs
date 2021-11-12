using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Configuration;
using System.Net;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // setting the default ip and port on startup
            Input_Port.Text = ConfigurationManager.AppSettings["serverPort"];
            Input_IP.Text = ConfigurationManager.AppSettings["serverIP_Address"];
        }


        /*
        * FUNCTION : ValidateInput
        *
        * DESCRIPTION : This function is used to validate user input
        *
        * PARAMETERS : none
        *
        */
        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(Input_IP.Text.Trim()) ||
                string.IsNullOrEmpty(Input_Port.Text.Trim()) ||
                string.IsNullOrEmpty(Input_Name.Text.Trim()) ||
                int.TryParse(Input_Port.Text.Trim(), out _) == false)
            {
                string message = "All fields are requied";
                string title = "Missing/invalid connection information";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons);
                return false;
            }
            return true;
        }

        private void Submit_btn_Click(object sender, RoutedEventArgs e)
        {
            if (this.ValidateInput() == true)
            {
                bool connectionOk = false;

                string response = Communicator.ConnectClient(Input_IP.Text, Convert.ToInt32(Input_Port.Text.Trim()), "Connect");

                if (response.StartsWith("Success"))
                {
                    // connection response template is: "Success;guid=guid;maxRange=max;minRange=min"
                    string[] responseArray = response.Split(';');
                    if (responseArray.Length == 4)
                    {
                        try
                        {
                            string guid = responseArray[1].Split('=')[1];    // index 1 of request array would have guid
                            int maxRange = Convert.ToInt32(responseArray[2].Split('=')[1]);  // index 2 of request array would have max range
                            int minRange = Convert.ToInt32(responseArray[3].Split('=')[1]);  // index 3 of request array would have min range

                            connectionOk = true;
                            ClientStatus.Guid = guid;
                            ClientStatus.MaxRange = maxRange;
                            ClientStatus.MinRange = minRange;
                            ClientStatus.Name = Input_Name.Text;
                            ClientStatus.serverIP = Input_IP.Text;
                            ClientStatus.port = Convert.ToInt32(Input_Port.Text.Trim());
                        }
                        catch (Exception)
                        {
                            connectionOk = false;
                        }
                    }
                    else
                    {
                        connectionOk = false;
                    }
                }

                // If connection is made
                if (connectionOk == true)
                {
                    this.Hide();
                    Game obj = new Game();
                    obj.Show();
                }
                else // if their was a connection problem
                {
                    string message = "Do you want to reconnect?";
                    string title = "Problem in connecting";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result = MessageBox.Show(message, title, buttons);
                    if (result == System.Windows.Forms.DialogResult.Yes) //User want to try again
                    {
                        Input_IP.Text = "";
                        Input_Port.Text = "";
                        Input_Name.Text = "";
                    }
                    else // User want to exit
                    {
                        Environment.Exit(1);
                    }
                }
            }
        }
    }
}

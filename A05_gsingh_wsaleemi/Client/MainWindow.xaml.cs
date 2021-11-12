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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool validCheck;
        private String iP;
        private String port_No;
        private String port_Name;


        public static bool ValidCheck
        {
            get { return validCheck; }
            set { validCheck = value; }
        }

        public String IP
        {
            get { return iP; }
            set { iP = value; }
        }
        public String Port_No
        {
            get { return port_No; }
            set { port_No = value; }
        }

        public String Port_Name
        {
            get { return port_Name; }
            set { port_Name = value; }
        }



        public MainWindow()
        {
            InitializeComponent();
        }


        private void VadidateInput()
        {
            //Storing data in local variables to validate by server
            IP = Input_IP.Text;
            Port_No = Input_Port.Text;
            Port_Name = Input_Port_Name.Text;

            //Updating the ValidCheck variable
            //-----Input server conditions and alter ValidCheck
            ValidCheck = true;
        }




        private void Submit_btn_Click(object sender, RoutedEventArgs e)
        {
          //----Only for testing
          ValidCheck = false;

          //If connection is made
          if(ValidCheck == true)
          {
                this.Hide();
                Game obj = new Game();
                obj.ShowDialog();
          }
          else //if their was a connection problem
          {
                string message = "Do you want to reconnect?";
                string title = "Problem in connecting";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes) //User want to try again
                {
                    Input_IP.Text = "";
                    Input_Port.Text = "";
                    Input_Port_Name.Text = "";
                }
                else //User want to exit
                {
                    Environment.Exit(1);
                }
            }

        }
    }
}

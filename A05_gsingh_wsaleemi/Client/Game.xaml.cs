using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Client
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Window
    {

        private static int miniNo;
        private static int maxxNo;
        private static int inputNo;
        private static bool gameComplete;



        public static int MiniNo
        {
            get { return miniNo; }
            set { miniNo = value; }
        }
        public static int MaxxNo
        {
            get { return maxxNo; }
            set { maxxNo = value; }
        }
        public static int InputNo
        {
            get { return inputNo; }
            set { inputNo = value; }
        }


        public static bool GameComplete
        {
            get { return gameComplete; }
            set { gameComplete = value; }
        }



        public Game()
        {
            InitializeComponent();

            // Values comes from client status class
            MiniNo = ClientStatus.MinRange;
            MaxxNo = ClientStatus.MaxRange;
            Title = $"Hello {ClientStatus.Name}, Please make a guess!!!";

            //Displaying range
            Output_Range.Content = MiniNo + "-" + MaxxNo;
        }



        private void Guess_Submit_Click(object sender, RoutedEventArgs e)
        {
            //Calling submit to check the guess number
            Submit();
        }

        /*
        * FUNCTION : Submit
        *
        * DESCRIPTION : This function is used to validate and submit user guess
        *
        * PARAMETERS : none
        *
        */
        private void Submit()
        {
            //check if no nuber is entered
            if (Input_Guess.Text != "")
            {
                if (int.TryParse(Input_Guess.Text.Trim(), out inputNo) == false) // check if imput for guess is not a number
                {
                    Output_Result.Content = "Invalid: Guess number must be a number";
                    Input_Guess.Text = "";
                }
                else if (inputNo < 0) // check if imput number is negative
                {
                    Output_Result.Content = "Invalid: Guess number cannot be negative";
                    Input_Guess.Text = "";
                }
                else if (inputNo > MaxxNo) // check if it is high
                {
                    Output_Result.Content = "Invalid: Guess number must be in the range";
                    Input_Guess.Text = "";
                }
                else if (inputNo < MiniNo) // check if it is low
                {
                    Output_Result.Content = "Invalid: Guess number must be in the range";
                    Input_Guess.Text = "";
                }
                else
                {
                    //  Default template for make a guess is: "MakeAGuess;guid=guid;guess=number"
                    string response = Communicator.ConnectClient(ClientStatus.serverIP, ClientStatus.port, $"MakeAGuess;guid={ClientStatus.Guid};guess={InputNo}");

                    if (response.StartsWith("YouWin")) // check if it is right guess
                    {
                        Output_Result.Content = "-----YOU WIN!!-----";
                        Input_Guess.Text = "";
                        Win(); //calling Win() function
                    }
                    else //if number is not right
                    {

                        // Default template for response to an incorrect guess: "hiLo=high/low;maxRange=MaxRange;minRange=MinRange"
                        string[] responseArray = response.Split(';');

                        if (responseArray.Length == 3)
                        {
                            try
                            {
                                string hiLo = responseArray[0].Split('=')[1];    // index 0 of request array would have high/low
                                int maxRange = Convert.ToInt32(responseArray[1].Split('=')[1]);  // index 1 of request array would have updated max range
                                int minRange = Convert.ToInt32(responseArray[2].Split('=')[1]);  // index 2 of request array would have updated min range

                                Output_Result.Content = $"-----Guess is too {hiLo}-----";

                                ClientStatus.MaxRange = maxRange;
                                ClientStatus.MinRange = minRange;
                            }
                            catch (Exception)
                            {
                                Output_Result.Content = "Invalid response from the server";
                            }
                        }
                        else
                        {
                            Output_Result.Content = "Invalid response from the server";
                        }

                        Update_Range(); //Calling Update_Range function 
                    }
                }
            }
            else //if no number is entered
            {
                Output_Result.Content = "Invalid: No Guess number entered";
                Input_Guess.Text = "";
            }
        }



        private void Update_Range()
        {
            //----values to come from the server
            MiniNo = ClientStatus.MinRange;
            MaxxNo = ClientStatus.MaxRange;

            //Displaying range
            Output_Range.Content = MiniNo + "-" + MaxxNo;
            Input_Guess.Text = "";
        }



        private void Win()
        {
            //update that game is complete
            GameComplete = true;


            string message = "Do you want to play again?";
            string title = "Close Window";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                Output_Result.Content = "";
                string response = Communicator.ConnectClient(ClientStatus.serverIP, ClientStatus.port, $"StartOver;guid={ClientStatus.Guid}");

                // Default template for response to StartOver: "Success;maxRange=MaxRange;minRange=MinRange"
                string[] responseArray = response.Split(';');

                if (responseArray.Length == 3)
                {
                    try
                    {
                        int maxRange = Convert.ToInt32(responseArray[1].Split('=')[1]);  // index 1 of request array would have updated max range
                        int minRange = Convert.ToInt32(responseArray[2].Split('=')[1]);  // index 2 of request array would have updated min range

                        ClientStatus.MaxRange = maxRange;
                        ClientStatus.MinRange = minRange;
                        Update_Range(); //Calling Update_Range function 
                    }
                    catch (Exception)
                    {
                        Output_Result.Content = "Invalid response from the server";
                    }
                }
                else
                {
                    Output_Result.Content = "Invalid response from the server";
                }

            }
            else
            {
                SafeExit();
            }

        }




        private void Game_window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GameComplete = false;

            if (GameComplete == true)
            {
                SafeExit();
            }
            else
            {
                string message = "Do you want to end this game?";
                string title = "Close Window";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    SafeExit();
                }
                else
                {
                    e.Cancel = true;
                }

            }

        }



        private void Input_Guess_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //to clear the feedback message
            Output_Result.Content = "";
        }



        private void Exit_Game_Click(object sender, RoutedEventArgs e)
        {

            string message = "Do you want to end this game?";
            string title = "Exit Game";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                SafeExit();
            }

        }

        private void SafeExit()
        {

            //  Default template for Leave is: "Leave;guid=guid"
            string response = Communicator.ConnectClient(ClientStatus.serverIP, ClientStatus.port, $"Leave;guid={ClientStatus.Guid}");

            // Default template for response to Leave: "Success"
            if (response.StartsWith("Success"))
            {
                Environment.Exit(1);
            }
            else
            {
                Environment.Exit(0);
            }

        }
    }
}

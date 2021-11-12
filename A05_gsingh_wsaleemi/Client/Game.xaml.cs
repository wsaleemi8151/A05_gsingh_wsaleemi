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
        private static int rightGuess;
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

        public static int RightGuess
        {
            get { return rightGuess; }
            set { rightGuess = value; }
        }

        public static bool GameComplete
        {
            get { return gameComplete; }
            set { gameComplete = value; }
        }



        public Game()
        {
            InitializeComponent();

            //---Values to be came from the server
            MiniNo = 1;
            MaxxNo = 100;
            RightGuess = 50;

            //Displaying range
            Output_Range.Content = MiniNo + "-" + MaxxNo;
        }



        private void Guess_Submit_Click(object sender, RoutedEventArgs e)
        {
            //Calling submit to check the guess number
            Submit();
        }



        private void Submit()
        {

            //check if no nuber is entered
            if (Input_Guess.Text != "")
            {
                inputNo = int.Parse(Input_Guess.Text);

                if (inputNo < 0) //check if imput number is negative
                {
                    Output_Result.Content = "Invalid: Guess number cannot be negative";
                    Input_Guess.Text = "";
                }
                else if (inputNo > MaxxNo) //check if it is high
                {
                    Output_Result.Content = "Guess number is too high,";
                    Input_Guess.Text = "";
                }
                else if (inputNo < MiniNo) //check if it is low
                {
                    Output_Result.Content = "Guess number is too low";
                    Input_Guess.Text = "";
                }
                else if (inputNo == RightGuess) //check if it is right guess
                {
                    Output_Result.Content = "-----YOU WIN!!-----";
                    Input_Guess.Text = "";
                    Win(); //calling Win() function
                }
                else //if number is not right
                {
                    Output_Result.Content = "-----NOT RIGHT GUESS-----";
                    Input_Guess.Text = "";
                    Update_Range(); //Calling Update_Range function 
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
            MiniNo = 10;
            MaxxNo = 50;
            RightGuess = 20;

            //Display new range
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
                Update_Range();
            }
            else
            {
                Environment.Exit(1);
            }

        }




        private void Game_window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GameComplete = false;

            if (GameComplete == true)
            {
                Environment.Exit(1);
            }
            else
            {
                string message = "Do you want to end this game?";
                string title = "Close Window";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    Environment.Exit(1);
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
                Environment.Exit(1);
            }
          
        }
    }
}

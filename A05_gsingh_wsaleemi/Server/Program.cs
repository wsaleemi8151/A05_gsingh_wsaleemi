/*
* FILE : Program.cs
* PROJECT : PROG2121-Windows and Mobile Programming - Assignment #5
* PROGRAMMER : Gursharan Singh - Waqar Ali Saleemi
* FIRST VERSION : 2012-11-12
* DESCRIPTION :
* The functions in this file are used for HiLo Game server
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        private static List<GameClient> gameClients = new List<GameClient>();
        private static readonly int defaultMaxRange = Convert.ToInt32(ConfigurationManager.AppSettings["defaultMaxRange"]);
        private static readonly int defaultMinRange = Convert.ToInt32(ConfigurationManager.AppSettings["defaultMinRange"]);

        static void Main(string[] args)
        {
            TcpListener gameListener = null;
            try
            {
                Int32 serverPort = Convert.ToInt32(ConfigurationManager.AppSettings["serverPort"]);
                IPAddress serverIP_Address = IPAddress.Parse(ConfigurationManager.AppSettings["serverIP_Address"]);

                // TcpListener server = new TcpListener(port);
                gameListener = new TcpListener(serverIP_Address, serverPort);

                // Start listening for client requests.
                gameListener.Start();


                // Enter the listening loop.
                while (true)
                {
                    // Perform a blocking call to accept requests.
                    TcpClient client = gameListener.AcceptTcpClient();
                    ParameterizedThreadStart ts = new ParameterizedThreadStart(Worker);
                    Thread clientThread = new Thread(ts);
                    clientThread.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                gameListener.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }


        /*
        * FUNCTION : Worker
        *
        * DESCRIPTION : This function is used to process the client request
        *
        * PARAMETERS : Object o     -       TcpClient
        *
        */
        public static void Worker(Object o)
        {
            TcpClient client = (TcpClient)o;
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            string data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                string response = "";
                if (data.StartsWith("Connect"))
                {
                    // Connection request
                    // Creating a new GameClient session
                    GameClient newClient = new GameClient()
                    {
                        Guid = Guid.NewGuid().ToString(),
                        MaxRange = defaultMaxRange,
                        MinRange = defaultMinRange,
                        TheNumber = GetRandomNumber(defaultMinRange, defaultMaxRange)
                    };
                    gameClients.Add(newClient);

                    // connection response template is: "Success;guid=guid;maxRange=max;minRange=min"
                    response = $"Success;guid={newClient.Guid};maxRange={newClient.MaxRange};minRange={newClient.MinRange}";
                }
                else if (data.StartsWith("MakeAGuess"))
                {
                    // User made a Guess

                    //  Default template for make a guess is: "MakeAGuess;guid=guid;guess=number"
                    string[] requestArray = data.Split(';');
                    if (requestArray.Length == 3)
                    {
                        try
                        {
                            string guid = requestArray[1].Split('=')[1];    // index 1 of request array would have guid
                            int guessedNumber = Convert.ToInt32(requestArray[2].Split('=')[1]);  // index 2 of request array would have guess
                            int clientIndex = gameClients.FindIndex(x => x.Guid == guid);

                            if (clientIndex >= 0)
                            {
                                if (guessedNumber == gameClients[clientIndex].TheNumber)
                                {
                                    response = "YouWin";
                                }
                                else
                                {
                                    string hiLo = "";
                                    int maxRange = gameClients[clientIndex].MaxRange;
                                    int minRange = gameClients[clientIndex].MinRange;
                                    if (guessedNumber > gameClients[clientIndex].TheNumber)
                                    {
                                        hiLo = "high";
                                        gameClients[clientIndex].MaxRange = guessedNumber;
                                    }
                                    else
                                    {
                                        hiLo = "low";
                                        gameClients[clientIndex].MinRange = guessedNumber;
                                    }

                                    // Default template for response to an incorrect guess: "hiLo=high/low;maxRange=MaxRange;minRange=MinRange"
                                    response = $"hiLo={hiLo};maxRange={gameClients[clientIndex].MaxRange};minRange={gameClients[clientIndex].MinRange}";
                                }
                            }
                        }
                        catch (Exception)
                        {
                            response = "InvalidRequest";
                        }
                    }
                    else
                    {
                        response = "InvalidRequest";
                    }
                }
                else if (data.StartsWith("StartOver"))
                {
                    // User wants to start a new game

                    //  Default template for startover is: "StartOver;guid=guid"
                    string[] requestArray = data.Split(';');
                    if (requestArray.Length == 2)
                    {
                        try
                        {
                            string guid = requestArray[1].Split('=')[1];    // index 1 of request array would have guid
                            int clientIndex = gameClients.FindIndex(x => x.Guid == guid);

                            gameClients[clientIndex].MaxRange = defaultMaxRange;
                            gameClients[clientIndex].MinRange = defaultMinRange;
                            gameClients[clientIndex].TheNumber = GetRandomNumber(defaultMinRange, defaultMaxRange);

                            // Default template for response to StartOver: "Success;maxRange=MaxRange;minRange=MinRange"
                            response = $"Success;maxRange={defaultMaxRange};minRange={defaultMinRange}";
                        }
                        catch (Exception)
                        {
                            response = "InvalidRequest";
                        }
                    }

                }
                else if (data.StartsWith("Leave"))
                {
                    // User wants to leave the game

                    //  Default template for Leave is: "Leave;guid=guid"
                    string[] requestArray = data.Split(';');
                    if (requestArray.Length == 2)
                    {
                        try
                        {
                            string guid = requestArray[1].Split('=')[1];    // index 1 of request array would have guid
                            int clientIndex = gameClients.FindIndex(x => x.Guid == guid);
                            gameClients.RemoveAt(clientIndex);
                            response = $"Success";
                        }
                        catch (Exception)
                        {
                            response = "InvalidRequest";
                        }
                    }
                }
                byte[] responseMessage = System.Text.Encoding.ASCII.GetBytes(response);
                stream.Write(responseMessage, 0, responseMessage.Length);
            }

            // Shutdown and end connection
            client.Close();
        }


        // Generates a random number within a range.      
        public static int GetRandomNumber(int min, int max)
        {
            Random _random = new Random();
            return _random.Next(min, max);
        }

    }
}
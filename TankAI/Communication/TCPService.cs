using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using TankAI.Util;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using TankAI.AI;

namespace TankAI.Communication
{
    class TCPService : NetworkService
    {

        #region "Variables"         

        private TcpClient acceptedServer;         //when server initiated messages are recived , server act as a client(sender) for this 
        private TcpClient clientForServer;        //To send Client Initiated messagesto server. Ex:JOIN#,UP#,
        private TcpClient clientForGUI;
        private TcpListener listnerForServer;     //To listen the messages comming from Server asynchronously. Ex:Brodcast messages,
        private static TCPService comm;           //Singlton Communication services
        private NetworkStream inputStream;        //to setup input network streams
        private NetworkStream outputStream;       //to setup output network streams
        private StreamWriter writer;              //to write to output streams
        private BinaryReader reader;              //to read from input streams        
        private static string lastMessage;

      

        #endregion

        public static string LastMessage
        {
            get { return TCPService.lastMessage; }
           
        }




        #region "Construction"

        public TCPService()
        {
              
            listnerForServer = new TcpListener(IPAddress.Any,Constant.CLIENT_PORT); //create a listner for server initiated messages and replies from searver
            listnerForServer.Start();       //start the listner head of the time
        }

        public TCPService getInstance()             //Object construction is overhead. so keep only one instance where it possible
        {
            if (comm == null)
                return comm = new TCPService();
            else
                return comm;
        }


        #endregion 

        

        //initial server connect request is sent from here........

        public void connectServer()
        {
          sendMessage(Constant.C2S_INITIALREQUEST);                          //send join# request
          Thread oThread = new Thread(new ThreadStart(this.reciveMessage));  // listinng to  reply and server intiated messages is handling from separte thread 
          oThread.Start();                                                   //start the listner thread               
        //  Console.WriteLine("hello this is working ");
        }



        public void reciveMessage()
        {
            try
            {
                while (true)
                {

                    acceptedServer = listnerForServer.AcceptTcpClient();        //listn the server and store servre soket
                    inputStream = acceptedServer.GetStream();
                    reader = new BinaryReader(inputStream);
                    List<Byte> inputStr = new List<byte>();                     //reciver buffer incomming messags
                    int asw = 0;
                    while (asw != -1)
                    {
                        try
                        {
                            asw = this.reader.ReadByte();   //read the bytes from the streams
                            inputStr.Add((Byte)asw);
                        }
                        catch (EndOfStreamException) { break; }
                        catch (IOException) { break; }
                    }
                 //   Console.WriteLine();
                    string s = Encoding.UTF8.GetString(inputStr.ToArray());         //convert it to character string
                    if(!Constant.GUIOFF)
                        sendToGUI(s);
                    Decorder decod = new Decorder(s.Substring(0, s.Length - 1));      //Decode the message with escaping '#'
                    Thread oThread = new Thread(new ThreadStart(decod.decode));     //Decoding is occured in independent threads , since random messages may apper

                    oThread.Start();
                   
                    //  using (StreamWriter w = File.AppendText("capture.txt"))
                    //{
                    //  w.WriteLine(s+"\n");
                    //}

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
           
        }

        public void sendMessage(String message)
        {
            try
            {
                lastMessage = message;
                Console.WriteLine(message);
                this.clientForServer = new TcpClient(); 
                clientForServer.Connect(IPAddress.Parse(Constant.SERVER_IP), Constant.SERVER_PORT);
                outputStream = clientForServer.GetStream();
                this.writer = new StreamWriter(outputStream);
                this.writer.Write(message);
                this.writer.Flush();
                this.writer.Close();
                //Console.WriteLine(HighResolutionTime.GetTime());
                Console.WriteLine("Last Response time:"+GameAgent.Timer.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                Console.WriteLine("Send Message Thrown an Exception" + e.StackTrace);

            }

               // this.clientForServer.Close();
            
           

        }
        public void sendToGUI(String message) {
            try
            {
                this.clientForGUI = new TcpClient();
                clientForGUI.Connect(IPAddress.Parse(Constant.GUI_IP), Constant.GUI_PORT);
                outputStream = clientForGUI.GetStream();
                this.writer = new StreamWriter(outputStream);
                this.writer.Write(message);
                this.writer.Flush();
                this.writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }


    }
}

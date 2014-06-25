using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankAI.Util;
using TankAI.AI;
using System.Diagnostics;
namespace TankAI.Communication
{
   public class Decorder
    {
        private String message;
        private GameAgent  agent;   //refernce for the gameAgent to access the territory
        public String Message
        {
            get { return message; }
            set { message = value; }
        }

        public Decorder()
        {
            agent = GameAgent.getInstance();
        }

        public Decorder(String message) {
            agent = GameAgent.getInstance();
            this.message = message;            
        }

        public  void decode()
        {
            if(Message.Contains(':'))
            {
                    switch (message.ElementAt(0))
                    {
                        case 'S': Start(); break;
                        case 'I': Initialize(); break;
                        case 'G': global(); break;
                        case 'C': Coins();break;
                        case 'L':LifePack();break;                    
                    }           
            }
            else{
                
                if(Message.Equals( Constant.S2C_GAMEJUSTFINISHED))
                    S2C_GAMEJUSTFINISHED();
                else                      
               
                switch (TCPService.LastMessage)
                {
                    case Constant.C2S_INITIALREQUEST:joinreply(); break;
                    default :reply(); break;
                }
            }

           // agent.Territory.showTerritory();
        }

        public  void   Start()
        {
            //S:P0;0,0;0:P1;0,9;0:P2;9,0;0:P3;9,9;0:P4;5,5;0
           // Console.WriteLine("S message");
            

            String[] playersDet = Message.Split(':');
            
            Tank[] tanks = new Tank[playersDet.Length - 1];
            agent.Tanks = tanks;

            for (int r = 1; r < playersDet.Length; r++) // cause r=0  => S
            {
                
                String[] divide = playersDet[r].Split(';');
                String[] cordinates = divide[1].Split(',');
               
                Tank newTank=new Tank();
               
                newTank.X = int.Parse(cordinates[0]);
                newTank.Y = int.Parse(cordinates[1]);
                newTank.Direction = int.Parse(divide[2]);
              // agent.Tanks[r - 1].Show();

               
                agent.Tanks[r-1]=newTank;
                agent.Territory.add(newTank, newTank.X, newTank.Y);

                if (r - 1 == agent.Id)
                {
                    agent.OurTank = newTank;
                }
                else
                {
                    //Enymy Cost need to be updated...................................................................................

                    agent.enmyCostUpdate(newTank.X, newTank.Y);
                    
                }

                
            }
            for (int i = 0; i < tanks.Length; i++)
            {
                if (i != agent.Id)
                    agent.lineOfSightUpdate(i);
            }
            
            Console.WriteLine("Started............GO>>>>");
            agent.Decide();
        }

        public Tank start1()
        {    //S:P0;0,0;0
             Console.WriteLine(Message);
              String[] playersData = Message.Split(':');
              Tank mytank = new Tank();           
              mytank.Id= int.Parse(playersData[1].Substring(1));           
              String [] coordinates= playersData[2].Split(',');
              mytank.X=int.Parse(coordinates[0]);;
              mytank.Y=int.Parse(coordinates[1]);;
              mytank.Direction= int.Parse(playersData[3]);
              agent.Territory.add(mytank, mytank.X, mytank.Y);
             return null;
        }
            
        public  void Initialize()
        {
            //I:P4:4,7;5,7;7,1;5,3;7,2;0,4;8,6:0,3;2,3;9,3;6,8;8,7;2,6;8,1;1,4:3,6;0,8;8,4;7,6;1,8;0,2;2,7;4,3;2,4;9,8#
           
            string[] locations = this.Message.Split(':');
            CharEnumerator iEnum = locations[1].GetEnumerator();
            iEnum.MoveNext();
            iEnum.MoveNext();

           agent.Id = int.Parse(char.ToString(iEnum.Current));
           Console.WriteLine("Your Player ID P"+agent.Id);
           //Console.WriteLine(agent.Id);

            string[] bricks = locations[2].Split(';');
            string[] stones = locations[3].Split(';');
            string[] waters = locations[4].Split(';');


            foreach (string brick in bricks)
            {
                string[] brickCordinates = brick.Split(',');
                Brick b = new Brick();
                b.X = int.Parse(brickCordinates[0]);
                b.Y = int.Parse(brickCordinates[1]);
                b.Life= 100;

                agent.BrickCostEvaluation(b);
                //b.Show();
                //Console.WriteLine();
            }
            foreach (string stone in stones)
            {
                string[] stoneCordinates = stone.Split(',');
                Stone s = new Stone();
                s.X = int.Parse(stoneCordinates[0]);
                s.Y = int.Parse(stoneCordinates[1]);
                agent.StoneCostEvalution(s);
             // s.Show();
               // Console.WriteLine();

            }
            foreach (string water in waters)
            {
                string[] waterCordinates = water.Split(',');
                Water w = new Water();
                w.X = int.Parse(waterCordinates[0]);
                w.Y = int.Parse(waterCordinates[1]);
                agent.WaterCostEvaluation(w);
                // w.Show();
                //Console.WriteLine();               
            }

            Console.WriteLine("Game is initialized.......... Be Ready >>>>>>>>>>>");
           
        }

        public   void Coins()
        {
            try{
                string[] s = Message.Split(':');
                string[] cordinates = s[1].Split(',');
                Coin c = new Coin();
                c.X = int.Parse(cordinates[0]);
                c.Y= int.Parse(cordinates[1]);
                c.Life = int.Parse(s[2]);
                c.Value = int.Parse(s[3]);
                agent.prioritizeCoins(c);
            }catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            } 
        }

        public  void LifePack()
        {   //L:4,9:17666#
           
            string[] s = Message.Split(':');
            string[] cordinates = s[1].Split(',');
            LifePack l = new LifePack();
            l.X = int.Parse(cordinates[0]);
            l.Y = int.Parse(cordinates[1]);
            l.Life = int.Parse(s[2]);
          
            //l.Show();           
            agent.prioritizeLifePack(l);
        }

        
       //MOST IMPORTANT MESSAGES SO SHULD put EXTRA CARE................
       
       public  void global() //NEEEEDDDD TO IMPROVE HEAVILY 
        {
         //   HighResolutionTime.Start();

            GameAgent.Timer.Restart();
            //G:P0;0,0;0;0;100;0;0:P1;0,9;0;0;100;0;0:P2;9,0;0;0;100;0;0:P3;9,9;0;0;100;0;0:P4;5,5;0;0;100;0;0:4,7,0;5,7,0;7,1,0;5,3,0;7,2,0;0,4,0;8,6,0#
             string[] updates = this.Message.Split(':');

            //Console.WriteLine(Message);
           
           //update[0]=G, update[1-5]=TankUpdates , update[6-end]=brickUpdates

            
           for (int i = 0; i < agent.Tanks.Length; i++)
           {
               string[] tankUpdates = updates[i + 1].Split(';');
               string[] newCorrdinates = tankUpdates[1].Split(',');


               int tempX = agent.Tanks[i].X;
               int tempY = agent.Tanks[i].Y;
               int currentX = agent.Tanks[i].X = int.Parse(newCorrdinates[0]);
               int currentY = agent.Tanks[i].Y = int.Parse(newCorrdinates[1]);



               agent.Tanks[i].Direction = int.Parse(tankUpdates[2]);
               int isShot = int.Parse(tankUpdates[3]); //player is shot -1 else 0;
              
               if (isShot == 0)
                   agent.Tanks[i].IsShot = false;
               else
                   agent.Tanks[i].IsShot = true;

               agent.Tanks[i].Life = int.Parse(tankUpdates[4]);
               agent.Tanks[i].Coins = int.Parse(tankUpdates[5]);
               agent.Tanks[i].Score = int.Parse(tankUpdates[6]);

//Shoudl be decouple this function with Agent
              
               agent.updateGoalAchivement(currentX, currentY);

               if (agent.Id != i)
               {
                   if (tempX == currentX && tempY == currentY)
                   {

                   }
                   else
                   {
                       agent.enmyMovement(tempX, tempY, currentX, currentY);
                       
                   }

               }
               else{ 
                            //if our player's records are updated .
               }

               // agent.Tanks[i].Show();
               //Console.WriteLine();
           }

          

           List<Brick>.Enumerator bricksList =  agent.Bricks.GetEnumerator(); //since update message has the same order of initilization order

            int iterator=0;
            string[] brickUpdates = updates[agent.Tanks.Length+1].Split(';');
            while (bricksList.MoveNext())
            { 
  //NEED to reacheck the order when adding new items to the list.
 //Thousands of logics are ignored............................................................
                 string []brickDetails=brickUpdates[iterator].Split(',');
                int damageLeve=int.Parse(brickDetails[2]);            

                agent.BrickUpdates(bricksList.Current,damageLeve);
                 //bricksList.Current.Show();
                //Console.WriteLine();
            }



            agent.lineofsight.Clear();
            for (int i = 0; i < agent.Tanks.Length; i++)
            {
                if (i != agent.Id)
                    agent.lineOfSightUpdate(i);
            }


           //Think and execute the next command
           agent.Decide();

          //  Console.WriteLine(Message + '\n');
        }

       #region Replys of #JOIN
       public void joinreply()
       {
           switch (Message)
           {
               case Constant.S2C_ALREADYADDED: S2C_ALREADYADDED(); break;
               case Constant.S2C_CONTESTANTSFULL: S2C_CONTESTANTSFULL(); break;
               case Constant.S2C_GAMESTARTED: S2C_GAMESTARTED(); break;
           }
       }



       public void S2C_ALREADYADDED()
       {
           Console.WriteLine("you are already added!!!");
       }

       public void S2C_CONTESTANTSFULL()
       {
           Console.WriteLine("Contestant full!!");

       }
       public void S2C_GAMESTARTED()
       {
           Console.WriteLine("Game Has Already started");
       }
       
       #endregion

       #region Replys for Left#,Right#,up#,Down#,Shoot#

       public void reply()
       {
           switch (Message)
           {
               case Constant.S2C_HITONOBSTACLE: S2C_HITONOBSTACLE(); break;
               case Constant.S2C_CELLOCCUPIED: S2C_CELLOCCUPIED(); break;
               case Constant.S2C_TOOEARLY: S2C_TOOEARLY(); break;
               case Constant.S2C_INVALIDCELL: S2C_INVALIDCELL(); break;
               case Constant.S2C_NOTACONTESTANT: S2C_NOTACONTESTANT(); break;
               case Constant.S2C_NOTSTARTED: S2C_NOTSTARTED(); break;
               case Constant.S2C_GAMEOVER: S2C_GAMEOVER(); break;
               //  case Constant.S2C_FALLENTOPIT: S2C_FALLENTOPIT();  break;
               case Constant.S2C_NOTALIVE: S2C_NOTALIVE(); break;
           }
       }


       public void S2C_HITONOBSTACLE()
       {
           Console.WriteLine("HIt on obstacle");

       }

       public void S2C_CELLOCCUPIED()
       {
           Console.WriteLine("CEll occupied");
       }
       public void S2C_TOOEARLY()
       {
           Console.WriteLine("Too Early");
       }
       public void S2C_INVALIDCELL()
       {
           Console.WriteLine("Invalid Cell");
       }

       public void S2C_NOTACONTESTANT()
       {
           Console.WriteLine("cont a contestant");
       }

       public void S2C_NOTSTARTED()
       {
           Console.WriteLine("Not Started");
       }

       public void S2C_FALLENTOPIT()
       {
           Console.WriteLine("You have fallen to a pit :( ...");
       }
       
       public void S2C_GAMEOVER()
       {
           Console.WriteLine("Game is over!!");
       }
       public void S2C_NOTALIVE()
       {
           Console.WriteLine("You are killed!!!!");
       }

       #endregion

      

       public void S2C_GAMEJUSTFINISHED()
       {

           Console.WriteLine("\nGame is just finished so stop the program");
       }
    }
}

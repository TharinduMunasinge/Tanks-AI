using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankAI.Util;
using TankAI.Communication;
using System.Drawing;
using System.Diagnostics;
namespace TankAI.AI
{
    public enum direction{
        North =0,
        East=1,
        South=2,
        West=3,
    }

    public enum Strategy
    {
         Achiver =0,
        Attacker = 1,
        Defender = 2,
       
        
    }
    class GameAgent
    {
         public List<int> lineofsight = new List<int>();
        //public List<point> plrlineofsight = new List<point>();


        #region internal variables 
        public static GameAgent staticAgent;
        
        private List<Stone> stones = new List<Stone>();
        int[,] belmanDestribution;  //for belman value itteration algorithm
        private Territory territory;  //to see the snapshots
        private Client communicator;  //to communicate with  server and TankGUI;
        private Tank ourTank;         //to handle our client;
        private List<Brick> bricks = new List<Brick>();
        private List<LifePack> lifepacks = new List<LifePack>();
        private List<Water> pits = new List<Water>();
        private int id;
        private Tank[] tanks;
        private MinPriorityQueue<Goal> feasibleGoals = new MinPriorityQueue<Goal>(70);
        private static byte[,] weightedMap;
        private int goalindex = 0;
        private Strategy currentStrategy=Strategy.Defender;
        private static Queue<int> indexPool = new Queue<int>(20);
        private static Stopwatch timer = new Stopwatch();
       

        

       
        
        #endregion


        #region getters and setters
        public static Stopwatch Timer
        {
            get { return GameAgent.timer; }
            set { GameAgent.timer = value; }
        }

        public static Queue<int> IndexPool
        {
            get { return GameAgent.indexPool; }
            set { GameAgent.indexPool = value; }
        }
        public int Goalindex
        {
            get { return goalindex; }
            set { goalindex = value; }
        }
       
            private static Router router;
       
        public static byte[,] WeightedMap
        {
            get { return GameAgent.weightedMap; }
            set { GameAgent.weightedMap = value; }
        }
        


        public static Router Router
        {
            get { return GameAgent.router; }
            set { GameAgent.router = value; }
        }


        internal MinPriorityQueue<Goal> FeasibleGoals
        {
            get { return feasibleGoals; }
            set { feasibleGoals = value; }
        }

        

        
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private List<Coin> coins = new List<Coin>();

        public Tank[] Tanks
        {
            get { return tanks; }
            set { tanks = value; }
        }


        public List<Water> Pits
        {
            get { return pits; }
            set { pits = value; }
        }

        public List<Stone> Stones
        {
            get { return stones; }
            set { stones = value; }
        }

        public List<Coin> Coins
        {
            get { return coins; }
            set { coins = value; }
        }

        public List<LifePack> Lifepacks
        {
            get { return lifepacks; }
            set { lifepacks = value; }
        }

        public List<Brick> Bricks
        {
            get { return bricks; }
            set { bricks = value; }
        }

        public  int getNewKey()
        {
            int num;
            if (indexPool.Count == 0)
            {
                num = goalindex;
                goalindex++;
            }
            else {
               num=  indexPool.Dequeue();
            }

            return num;
        }


        public Tank OurTank
        {
            get { return ourTank; }
            set { ourTank = value; }
        }
        internal Territory Territory
        {
            get { return territory; }
            set { territory = value; }
        } 

        #endregion



        #region Game Controllers
        public void Up()
        {
            communicator.Up();
        }

        public void Down()
        {
            communicator.Down();
        }

        public void Left()
        {
            communicator.Left();
        }

        public void Right()
        {
            communicator.Right();
        }

        public void Shoot()
        {
            communicator.Shoot();
        }
        
        #endregion



       

        public void lineOfSightUpdate(int index)
        {
            
            int differnce;
            bool isSuitable=true;
            if (OurTank.X == tanks[index].X)
            {
                 differnce =  tanks[index].Y-ourTank.Y;

                if (differnce > 0)
                 {
                     for (int i = ourTank.Y + 1; i < tanks[index].Y; i++)
                     { 
                            if(!isValidCell(weightedMap[i,ourTank.X]))
                            {
                                isSuitable = false;
                                break;
                            }
                     }
                 }else if (differnce < 0) {
                    for (int i = tanks[index].Y+1;i<ourTank.Y ; i++)
                    {
                        if (!isValidCell(weightedMap[i, ourTank.X]))
                        {
                            isSuitable = false;
                            break;
                        }
                    }
                }

            }
            else if (ourTank.Y == tanks[index].Y)
            {
                differnce = tanks[index].X- ourTank.X;
                if (differnce > 0)
                {
                    for (int i = ourTank.X + 1; i < tanks[index].X; i++)
                    {
                        if (!isValidCell(weightedMap[ourTank.Y,i]))
                        {
                            isSuitable = false;
                            break;
                        }
                    }
                }
                else if (differnce < 0)
                {
                    for (int i = tanks[index].X + 1; i < OurTank.X; i++)
                    {
                        if (!isValidCell(weightedMap[ourTank.Y, i]))
                        {
                            isSuitable = false;
                            break;
                        }
                    }
                }
            }
            else 
            {
            return;
            }

            if (isSuitable)
            {
                Console.WriteLine("An Enemy Tank  P"+index+" Moves to Line of sight");
                lineofsight.Add(index);
            }
           
        }

        public void playerStatus()
        {


        }

        public void ChangeStratagy()
        {
            if (ourTank.Life <= 80) // always HealthPack HashSet higher weight thn Coin
            {
                currentStrategy = Strategy.Defender;
                Console.WriteLine("Situation is Critical Changed to to defender Mode" + ourTank.Life); 
            }

            if (ourTank.Life > 80 && ourTank.Life < 110)
            {
                Console.WriteLine("SOMBY MOOD :P ");
            }

            if (ourTank.Life >= 110) //Always Coins weight is High than lifepack and Goals tried to achive without considering risk
            {
                currentStrategy = Strategy.Achiver;
                Console.WriteLine("Situation is OKEY Changed to Achiver Mode" + ourTank.Life);
            }
        }


        #region Dynamic updates with G Message


        public void updateGoalAchivement(int currentX, int currentY)
        {
            GameObjects obj = Territory.objectAt(currentX, currentY);
            
            if (obj != null)
            {
                if (obj.Type == 'C')
                    ((Coin)obj).die(new object());
                if (obj.Type == 'L')
                    ((AI.LifePack)obj).die(new object());
                if (obj.Type == 'S')
                    ((AI.SurviveCell)obj).die();

            }

        }

        public void enmyMovement(int tempX, int tempY, int currentX, int currentY)
        {
            /////////////////////// shoud think of Enimy Cost
            Router.updateCost(tempX, tempY, Constant.defualtCost);
            Router.updateCost(currentX, currentY, Constant.defualtEnemyCost);
            
        }

        public void enmyCostUpdate(int currentX, int currentY)
        {
            /////////////////////// shoud think of Enimy Cost
          
            Router.updateCost(currentX, currentY, Constant.defualtEnemyCost);

        }

        public void BrickUpdates(Brick b, int damageLevel) //damage level update
        {
            b.Life = b.Life - (damageLevel * 25);

            GameAgent.Router.updateCost(b.X, b.Y, (byte)(b.Life + Constant.defualtCost));
        }

        #endregion


        #region SMessage Analysis
        
        #endregion

        #region Initialzation the territory with I message

        public void BrickCostEvaluation(Brick b)
        {
            //Should be optimize.............................................................................. the cost;

            Router.updateCost(b.X, b.Y, Constant.defaultBrickCost);

            Bricks.Add(b);
            Territory.add(b, b.X, b.Y);
        }

        public void WaterCostEvaluation(Water w)
        {
            Pits.Add(w);
            Territory.add(w, w.X, w.Y);
            Router.updateCost(w.X, w.Y, Constant.defaultWaterCost); //Never use +Exption in shooting
        }

        public void StoneCostEvalution(Stone s)
        {
            Router.updateCost(s.X, s.Y, Constant.defaultWaterCost); //NEVER USE
            Stones.Add(s);
            Territory.add(s, s.X, s.Y);
        }
        
        #endregion
     
        
        #region SetPriorityValues and Cost

        public void prioritizeLifePack(LifePack lp)
        {
            int index = getNewKey();
            lp.spawn(index);
            double d = analyseLPPriority(lp);
            Console.WriteLine("index " + index + " " + lp.X + " " + lp.Y + " Pri:" +d+" T:"+lp.Type);
            FeasibleGoals.Insert(index, new Goal(lp,d));
           // Goalindex++;
            Router.updateCost(lp.X, lp.Y, Constant.defaultLPCost);
            Territory.add(lp, lp.X, lp.Y);
        }

        public void prioritizeBrick(Brick br)
        {


        }
      
        public float prioritizeTheGainofShooting()
        {
            return 0;
        }

        public double prioritizeCoins(Coin c)
        {
              
            //Priority of the Coin Based on the value,Lifetime,distance...
            int index = getNewKey();
            Territory.add(c, c.X, c.Y);
            c.spawn(index);
  //Coin priority should be a function(CoinValue,coinLife,Player Current state) shortest Distance to Coin 
            Router.updateCost(c.X, c.Y, Constant.defultCoinCost);
            double weight = analyseCoinPriority(c);
            Console.WriteLine("index " + index + " " + c.X + " " + c.Y + " Pri :" + weight + " type:" + c.Type);
            FeasibleGoals.Insert(index, new Goal(c, weight));
          
            //  Goalindex++;
            //c.Show();
         return 0;

        } 
        #endregion




        #region UpdatePriorities

        public void  updatePlayerPriority()
        { 
        
        }

        public void updateCoinPriority()
        { 
        
        }
        public void updateLifePackPriority()
        { 
        
        }

        public void defendingPriority()
        {
        
        }
        

        #endregion



        //Keep the this function simpleAspossible.So that we can evaluate in most of the events=>AI takes smart  decisions
        //test with overall performance .. 
        //Actual DISTANCE TO Any GOAL IS Important BT DONt Always rely on that=> peformance Really get reduce=> Use Heuristic Cost as much as possible

        #region Priorizing Goals.....


        public float analyseLPPriority(LifePack lp)
        {
            //priority of getting Lifepack based on the game FACTS and Statusstatus
            //Ex: Our Health,distnce,*lifetime of Lp...


            //this is overall priority must be weighted with preDefinded weight based on the currentSTrategy..
            //Ex switch(Current.stratgy)
            //    case achiver: weight=o.2..

            return (float)(lp.Life/400.0);
        }


        public float analyseCoinPriority(Coin c)
        {
            //Weight of the Brick shooting depend on the Strategy.
            Point end=new Point();
            end.X=c.Y;
            end.Y=c.X;
            Point start= new Point(ourTank.Y,ourTank.X);
            List<PathFinderNode> p= Router.FindPath(start, end);
            int distance = p.Count() - 1;
          //  return (float)(c.Value + c.Life / 1000.0- 7*(Math.Abs(c.X-ourTank.X)+Math.Abs(c.Y-ourTank.Y)));

            return (float)(c.Value + c.Life / 1000.0 - 25 * (distance));
        }

        public float analyseBrickPriority()
        {
            //Weight of the Brick shooting depend on the Strategy.
            return 0;
        }

      
        public float analyseShootingPriority()
        {

            return 0;
        }
        
        #endregion

               
        
        public GameAgent()
        {
            weightedMap = new byte[Constant.MAP_SIZE+1, Constant.MAP_SIZE+1];
            router = new Router(weightedMap);
            
            belmanDestribution = new int[Constant.MAP_SIZE, Constant.MAP_SIZE];
            territory = new Territory();            
        }


               

        public void Decide()
        {

            ChangeStratagy();
            switch (currentStrategy)

            {
                case Strategy.Defender: defend(); break;
                case Strategy.Achiver: Achiver(); break;
                case Strategy.Attacker: Attacker(); break;
            }
           
        }



        public void isRisk()
        {
            List<int>.Enumerator enemyLineOfSight = lineofsight.GetEnumerator();

            int i = -1;
            while(enemyLineOfSight.MoveNext())
            {
                i = enemyLineOfSight.Current;

                Tank enemy = tanks[i];
                int absDirection = enemy.Direction;
                int distance = 0;

                int riskDirection = 0;//to whihc direction enemy can attack.
                if (enemy.X == ourTank.X)
                {

                    distance = enemy.Y - ourTank.Y;

                    if (distance > 0)
                    {
                        riskDirection = 0;//riskdirection is best posible in dead end

                    }
                    else
                    {
                        riskDirection = 2;
                    }

                }

                if (enemy.Y == ourTank.Y)
                {
                    distance = enemy.X - ourTank.X;

                    if (distance > 0)
                    {
                        riskDirection = 3;//riskdirection is best posible in dead end

                    }
                    else
                    {
                        riskDirection = 1;
                    }
                }

                if (absDirection == riskDirection) //enemy can attack us  
                {
                    Console.WriteLine("Enemy P" + i + " Can Attack Us");

                    int escapeseSteps = Math.Abs(distance) / 3;
                    if (enemy.IsShot) //enemy difenitly attacks us 
                    {
                        Console.WriteLine("Enemy P" + i + " Deffintly Attack Us");

                        escapeseSteps = escapeseSteps + ourTank.Life / 10 - 1; //worst case defendinch chance
                        Point p = escape(escapeseSteps, riskDirection, Math.Abs(distance), enemy);
                        if (p.Equals(new Point(-1, -1)))
                        {
                            //definitly we should attack Attack Attack Attack ....
                            Console.WriteLine("There is No survival Path + We Should Couter Attack now");

                        }
                        else
                        {
                            //  Survie method was there
                            Console.WriteLine("There is a Surival path Lets take it");

                            GameObjects go = (feasibleGoals.MinKey()).gameObjects;

                            if (go.Type == 'S')
                            {

                                Console.WriteLine("We have added this Survival goal before");
                            }
                            else
                            {

                                SurviveCell s = new SurviveCell(p.X, p.Y);
                                territory.add(s, s.X, s.Y);

                                int key = s.GoAlindex = getNewKey();
                                feasibleGoals.Insert(key, new Goal(s, 100000));// We shoud avoid attacks

                            }
                        }
                    }
                    else
                    {
                        //CAn shoot
                        Console.WriteLine("We have a Chance of damage enemy P" + i);
                    }

                }







            }
                //for(int i=0;i<tanks.Length;i++)
                //{
                  //  int ourplayerDirection=ourTank.Direction;
                   // if(i!=Id)
                    //{
                       
                    //}
                //s}
        }


        public Point escape(int numSteps,int riskDirection,int distance,Tank enemey){
                bool isXstatic=false;
                bool isPropotional=false;
                int ourDirection=ourTank.Direction;
                int staticCoordinate=0;
                int dynamicDoordinate=0;
                int midlinStatic=0;
                int midlineDynamic=0;

            if(riskDirection%2==0){
                staticCoordinate=ourTank.X;      
                dynamicDoordinate=ourTank.Y;
                isXstatic=true;
                midlinStatic=ourTank.X;
                midlineDynamic=ourTank.Y;
            }else{
                staticCoordinate=ourTank.Y;
                dynamicDoordinate=ourTank.X;
                midlineDynamic=ourTank.X;
                midlinStatic=ourTank.Y;
                isXstatic=false;
            }

            if(riskDirection==1 || riskDirection==2)
                isPropotional=true;

          //  if(riskDirection==ourTank.Direction)//Medium Safeness
            //{
                //both bulllet and ourtank in same direction
            
                       
                    for(int iterator=0; dynamicDoordinate>=0 && dynamicDoordinate<Constant.MAP_SIZE;iterator++)
                    {
                        int cellCost;
                      //  int x1,y1,x2,y2;
                        Point [,]availablePlace=new Point[2,2];
                        Point [] endPoint=new Point[2]; 
                        if(isXstatic){                            
                            endPoint[0]=new Point(midlinStatic,dynamicDoordinate);   
                            endPoint[1]=new Point(midlinStatic,-dynamicDoordinate);        
                            availablePlace[0,0]=new Point(midlinStatic+1,dynamicDoordinate);
                            availablePlace[0,1]=new Point(midlinStatic-1,-dynamicDoordinate);
                            availablePlace[1,0]=new Point(midlinStatic+1,+dynamicDoordinate);
                            availablePlace[1,1]=new Point(midlinStatic-1,-dynamicDoordinate);
                        } 
                        else{
                            endPoint[0]=new Point(dynamicDoordinate,midlinStatic);   
                            endPoint[1]=new Point(-dynamicDoordinate,midlinStatic);    
                            availablePlace[0,0]=new Point(dynamicDoordinate, midlinStatic+1);
                            availablePlace[0,1]=new Point(dynamicDoordinate, midlinStatic-1);
                            availablePlace[1,0]=new Point(-dynamicDoordinate,midlinStatic+1);
                            availablePlace[1,1]=new Point(-dynamicDoordinate,midlinStatic-1);
                            // x1=x2=dynamicDoordinate;
                            //y1=midlinStatic+1;
                            //y2=midlinStatic-1;            
                        }
                       
                        for(int i=0;i<2;i++)
                        {
                            if(isValidDynamic(endPoint[i].X,endPoint[i].Y))
                            {
                                for(int j=0;j<2;j++){
                                       if(isValidDynamic(availablePlace[i,j].X,availablePlace[i,j].Y))
                                       {
                                              return availablePlace[i,j]; //there is a best cell
                                       }else{
                                              continue;
                                       }
                                }       
                            }
                       }
                     
                                              
                       dynamicDoordinate+=iterator;
                    }
            
            //}
            //else if(Math.Abs(riskDirection-ourDirection)==1)//BEST CASE
            //{
                //bullet direction and our direction is perpendicular

            //}

            //else if(Math.Abs(riskDirection-ourDirection)==2) //MOST RISKY
            //{
                //bullet direction and our direction opposite
                          
            
            return new Point(-1,-1); 
        }



        public bool isValidDynamic(int x,int y)
        {
            int size=Constant.MAP_SIZE;
            
                if(x<size && y<size && x>=0 && y>=0)
                {
                    int cost = weightedMap[y, x];

                    if(cost==Constant.defualtCost || cost==Constant.defultCoinCost || cost==Constant.defaultLPCost)
                    {
                        return true;
                    }
                    else{
                        return false;
                    }
                }
            return false;
        }      
        
        public bool isValidCell(int cost)
        {            
                    if(cost==Constant.defualtCost || cost==Constant.defultCoinCost || cost==Constant.defaultLPCost)
                    {
                         return true;
                    }
                    else
                    {
                         return false;
                    }
        }

        public void getEscapePath()
        {
                        
        }

        public void defend()
        {
           isRisk();
           Achiver();            
        }

        public void Achiver()
        {
             if (!feasibleGoals.IsEmpty())
            {
                GameObjects go = feasibleGoals.MinKey().gameObjects;
                Point endPoint = new Point(go.Y, go.X);
                Point startPoint = new Point(OurTank.Y, OurTank.X);


                List<PathFinderNode> path = Router.FindPath(startPoint, endPoint);
                if (path != null)
                {
                    List<PathFinderNode>.Enumerator en = path.GetEnumerator();

                    en.MoveNext();
                    en.MoveNext();

                    int newX = en.Current.X;
                    int newY = en.Current.Y;

                    if (startPoint.X > newX)
                    {
                        Up();
                    }
                    else if (startPoint.X < newX)
                    {
                        Down();

                    }
                    else if (startPoint.Y < newY)
                    {
                        Right();
                    }
                    else if (startPoint.Y > newY)
                    {
                        Left();
                    }
                    else
                    {
                        Shoot();
                    }
                    //Console.WriteLine(HighResolutionTime.GetTime());
                }
            }
            else { 
                //Should think of something done when there is no goals...
                //Like shooting bricks
            }
        }
        

        public void Attacker()
        {
                
        }
        
        
        public void takeAMove()
        { 
        
        }


        public static GameAgent getInstance()
        {
            if (staticAgent == null) 
            {
                staticAgent = new GameAgent();
            }
            return staticAgent;
        }


        

        public void initialzation()
        {
            communicator = new Client();
            communicator.initiate();
        }
        
    }
}

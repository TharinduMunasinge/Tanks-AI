using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;
using System.Text;
using TankAI.Communication;
using TankAI.Util;
using TankAI.AI;
using System.Drawing;
namespace TankAI
{
    static class Program
    {
        /*
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           // Console.WriteLine(ConfigurationManager.AppSettings.Get("MapSize"));
            //Console.ReadLine();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
         * /
         */
        static void Main(string[] args)
        {
           // TCPService t = new TCPService();
            //t.connectServer();


              GameAgent ga = GameAgent.getInstance();
         ga.initialzation();
        //    testForPriorityQ();
            //ManualMenu(ga);
          //  test2();
            
        }

        /*
         
         */
        static void test2()
        {
            Coin c = new Coin();
            c.X=1;
            c.Y=1;
            Territory t = new Territory();
            t.add(c, c.X, c.Y);
            method2(t);
            if (t.objectAt(1,1) == null)
                Console.WriteLine("done putha");
            Console.ReadLine();
        }

        static void method2(Territory t)
        {
            ((Coin)t.objectAt(1, 1)).testDie(t);
        }

        static void testForPriorityQ() {
            MinPriorityQueue<Goal> q = new MinPriorityQueue<Goal>(100);
              Goal g1=new Goal(new Coin(),6);
            Goal g2=new Goal(new Coin(),4);
            Goal g3=new Goal(new Coin(),3);

            q.Insert(0,g1);
            q.Insert(1,g2);
            q.Insert(2, g3);

            Console.WriteLine(q.MinKey().priority);
            q.Delete(1);


            Console.WriteLine(q.Contains(2));
            Console.WriteLine(q.Contains(1));
            Console.WriteLine(q.Contains(0));
           

            Console.ReadLine();

            //g1.priority = 1;
            //q.ChangeKey(1, g1);
            //q.Delete(
        }

        static void test()
        {

            byte [,] grid=new byte[21,21];

            grid[0, 0] = 3;
            grid[0, 1] = 4;
            grid[0, 2] = 5;
            grid[1, 0] = 2;
            grid[1, 1] = 0;
            grid[1, 2] = 7;
            grid[2, 0] = 1;
            grid[2, 1] = 8;
            grid[2, 2] = 9;           

           Router router= new Router(grid);
           List<PathFinderNode>  root = router.FindPath(new Point(0, 0), new Point(1, 2));
                   
                 
                 List<PathFinderNode>.Enumerator num = root.GetEnumerator();


                 while (num.MoveNext())
                 {
                     Console.WriteLine(num.Current.X+" "+num.Current.Y);
                 }


            //MinPriorityQueue<Goal> q = new MinPriorityQueue<Goal>(100);
            //Goal g1=new Goal("tharindu",3);
            //Goal g2=new Goal("chamali",4);
            //Goal g3=new Goal("ABC",3);

            //q.Insert(1,g1);
            //q.Insert(2,g2);   
            
            //g1.priority=(111);
            //g1.priority = 1;
            //q.ChangeKey(1, g1);
            //q.Delete(
         //   Console.WriteLine(q.MinKey().ToString());
            Console.ReadLine();
          
        }

        static void ManualMenu(GameAgent ga)
        { 
            bool exit=false;
            while (!exit)
            {
                Console.WriteLine("Next Command :");
                char choice =char.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 'w': ga.Up(); break;
                    case 'd': ga.Right(); break;
                    case 'a': ga.Left(); break;
                    case 's': ga.Down(); break;
                    case ' ': ga.Shoot(); break;
                    case 'p': exit = true; break;
                }
            }
        }
        
    }
}

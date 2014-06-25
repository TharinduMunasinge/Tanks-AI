using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TankAI.Util;

namespace TankAI.AI
{

    public delegate void voidFunc();

   public  class Coin :GameObjects
    {
       public delegate void Del();

        int value;
        Timer t;
        int goAlindex;

        public Coin()
        {
            this.Type = 'C';
            // voidFunc func=die;
           //  TimerCallback tcb = this.die();
            
       }
        
        
      
       public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
       public void testDie(Territory obj)
       {
         obj.removeObject(this.X, this.Y);
           Console.WriteLine("Coin at " + this.X + "," + this.Y + " is removed Frome Terriotary");
       }

       public void spawn(int goalindex)
       {
           TimerCallback tcb = new TimerCallback(die);

           t = new Timer(tcb, null, this.Life, 0);
           this.goAlindex = goalindex;
       }

        public void die(Object args)
        {
            GameAgent.Router.updateCost(X, Y,Constant.defualtCost);
            GameAgent ga = GameAgent.getInstance();
            if(ga.FeasibleGoals.Contains(this.goAlindex)){
                      ga.FeasibleGoals.Delete(goAlindex);
                      GameAgent.IndexPool.Enqueue(goAlindex);
            }
            else{

                Console.WriteLine("unexpected deletion in GoalIndex="+this.goAlindex+" x="+this.X+",y="+this.Y+" type "+this.Type);

        }
            ga.Territory.removeObject(this.X, this.Y);
            string msg = "";
            if (args == null)
                msg = "is vanished ";
            else
                msg = "is obtained by a Tank";

            Console.WriteLine("Coin at "+this.X+","+this.Y+" "+msg);
        
        }

        public String Show()
        {
            base.Show();
            Console.Write("_V" + Value);
            return "_V" + Value;
        }


        

        public override bool Equals(object obj)
        {

            // Check for null values and compare run-time types.
            

            Coin p = (Coin)obj;
            return base.Equals(obj) && p.Value == Value;

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}

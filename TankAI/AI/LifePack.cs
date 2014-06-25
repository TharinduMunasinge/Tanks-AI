using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TankAI.Util;
namespace TankAI.AI
{
    public class LifePack :GameObjects

    {
        Timer t;
        int goAlindex;
      
        public LifePack()
        {
            this.Type = 'L';
        }

        public void spawn(int goalindex)
        {
            TimerCallback tcb = new TimerCallback(die);

            t = new Timer(tcb, null, this.Life, 0);
            this.goAlindex = goalindex;
        }

        public void die(Object args)
        {
            GameAgent.Router.updateCost(X, Y, Constant.defualtCost);

            GameAgent ga = GameAgent.getInstance();
            if (ga.FeasibleGoals.Contains(this.goAlindex))
            {
                ga.FeasibleGoals.Delete(goAlindex);
                GameAgent.IndexPool.Enqueue(goAlindex);
            }
            else
            {

                Console.WriteLine("unexpected deletion in GoalIndex=" + this.goAlindex + " x=" + this.X + ",y=" + this.Y + " type " + this.Type);
            }
            ga.Territory.removeObject(this.X, this.Y);
            
            string msg = "";
            if (args == null)
                msg = "is vanished ";
            else
                msg = "is obtained by a Tank";
            Console.WriteLine("LifePack at " + this.X + "," + this.Y + " " + msg);
        }
       

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            LifePack p = (LifePack)obj;
            return base.Equals(obj) && p.Type == Type;
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

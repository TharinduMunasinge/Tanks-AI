using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankAI.AI
{
   public class SurviveCell :GameObjects
    {

        private int goAlindex;


        public SurviveCell()
        {
            this.Type = 'S';
            // voidFunc func=die;
           //  TimerCallback tcb = this.die();
            
        }


        public int GoAlindex
        {
            get { return goAlindex; }
            set { goAlindex = value; }
        }

        public SurviveCell(int x, int y)
        {
            this.Type = 'S';
            this.X = x;
            this.Y = y;
        }
      
       public void die()
       {
             GameAgent ga = GameAgent.getInstance();
             if(ga.FeasibleGoals.Contains(this.goAlindex)){
                      ga.FeasibleGoals.Delete(goAlindex);
                      GameAgent.IndexPool.Enqueue(goAlindex);
             }else{
                   Console.WriteLine("unexpected deletion in GoalIndex="+this.goAlindex+" x="+this.X+",y="+this.Y+" type "+this.Type);
                }
            Console.WriteLine("Our Tank is survived from an Attack");
       }
               

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            SurviveCell p = (SurviveCell)obj;
            return base.Equals(obj);
            
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

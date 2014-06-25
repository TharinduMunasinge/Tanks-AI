using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankAI.AI
{
    class Goal : IComparable<Goal>
    {
        public  GameObjects gameObjects;
        public double priority; 
        
        // smaller values are higher priority

        public Goal(GameObjects go, double priority)
        {
            this.gameObjects = go;
            this.priority = priority;
           
        }

        public override string ToString()
        {
            return gameObjects.Show();
        }

        public int CompareTo(Goal other)
        {
           // if (this.priority < other.priority) return -1;
            //else if (this.priority > other.priority) return 1;
            //else return 0;

            if (this.priority < other.priority) return 1;
            else if (this.priority > other.priority) return -1;
            else return 0;
        }       

    }
}
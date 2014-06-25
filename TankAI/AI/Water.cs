using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankAI.AI
{
    class Water : GameObjects
    {
        public Water()
        {
            this.Type = 'W';
        }

        public override bool Equals(object obj)
        {

            // Check for null values and compare run-time types.
            

            Water p = (Water)obj;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankAI.AI
{
   public  class GameObjects
    {
        int x;
        int y;
        int life = 100;
        char type='O';

        public char Type
        {
          get { return type; }
          set { type = value; }
        }
        


        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        public int Life
        {
            get { return life; }
            set { life = value; }
        }
       

        public  String  Show(){
            Console.Write("T" + type + "_X" + X + "Y" + Y + "_L" + Life);
            return "T" + type + "_L" + Life;
        }

        public override bool Equals(object obj)
        {
    
              // Check for null values and compare run-time types.
              if (obj == null) 
                 return false;
      
                   GameObjects p = (GameObjects)obj;
                 return (X == p.x) && (Y == p.y) && (Life==p.Life);
        
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return Show();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankAI.AI
{
   public  class Tank :GameObjects
    {
        
       private int id;
      private  int score = 0;
      private int direction;
      private  int coins = 0;
      private bool isShot = true;

      public bool IsShot
      {
          get { return isShot; }
          set { isShot = value; }
      }


    
        public Tank() {
            this.Type = 'P';
        }

        public int Coins
        {
            get { return coins; }
            set { coins = value; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }       

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        public  String  Show()
        {
            base.Show();
            Console.Write("_I" +Id + "_D" + direction + "_S" + score);
            return "_I" + Id + "_D" + direction + "_S" + score;
        }

        public override bool Equals(object obj)
        {

            // Check for null values and compare run-time types.
            Tank p = (Tank)obj;
            return base.Equals(obj) && p.Type == Type &&  p.Life==Life && p.Id==Id && p.score==Score;

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

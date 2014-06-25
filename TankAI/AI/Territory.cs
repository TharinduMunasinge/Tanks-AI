using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankAI.Util;
using TankAI.Communication;

namespace TankAI.AI
{
    public class Territory
    {
        private  GameObjects[,] territory;
        private int yrange;
        private int xrange;

        public  Territory()
        { 
            territory=new  GameObjects[Constant.MAP_SIZE, Constant.MAP_SIZE];
            xrange = yrange = Constant.MAP_SIZE;
        }


        public void add(GameObjects gobj, int x, int y) {
            territory[y, x] = gobj;
        }

        public GameObjects objectAt(int x, int y)
        {
            return territory[y, x];
        }

        public void setCell(int x,int y,String lable)
        {
            
        }
        public void showTerritory()
        {
            for (int i = 0; i < yrange; i++)
            {
                for (int j = 0; j < xrange; j++)
                {
                    if (territory[i, j] != null)
                        switch (territory[i, j].Type)
                        {

                            case 'P': ((Tank)territory[i, j]).Show(); break;

                            default: territory[i, j].Show(); break;
                        }
                    else
                        Console.Write("BLANK");
                    Console.Write("\t");

                }
                Console.WriteLine();
            }
        }


        public void removeObject(int x, int y)
        {
            this.territory[y, x] = null;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using TankAI.Communication;
using TankAI.Util;
namespace TankAI.Communication
{
   public  class Client
    {
        private NetworkService nw;
        public void initiate(){
            nw = new TCPService();
            nw.connectServer();
        }

        public void Up()
        {
            nw.sendMessage(Constant.UP);
        }
        
         public void Down()
        {
            nw.sendMessage(Constant.DOWN);
        }

         public void Left()
         {
             nw.sendMessage(Constant.LEFT);
         }

         public void Right()
         {
             nw.sendMessage(Constant.RIGHT);
         }

         public void Shoot()
         {
             nw.sendMessage(Constant.SHOOT);
         }
            
       
    }
}



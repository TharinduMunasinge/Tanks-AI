using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankAI.Communication
{
     interface NetworkService
    {
         //to decoupe the network network funtionality => to shift between pubnub nd TCP
        void connectServer();

       void  reciveMessage();

       void sendMessage(String message) ;
        
        
    }
}

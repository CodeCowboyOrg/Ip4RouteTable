﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Networking
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Print all the Network adaptors
                NicInterface.PrintAllNetworkInterface();
                //Print the Routing Table
                Ip4RouteTable.RoutePrint();

                //Demostrate Deleting routes and Adding routes
                int interfaceIndex = 22; // Int32.Parse(args[0]);
                NetworkAdaptor na = NicInterface.GetNetworkAdaptor(interfaceIndex);
                if (na != null && na.PrimaryGateway.Address.ToString().Length > 0)
                {
                    Ip4RouteTable.DeleteRoute(interfaceIndex);
                    if (!Ip4RouteTable.RouteExists("202.0.0.0"))
                    {
                        Ip4RouteTable.CreateRoute("202.0.0.0", "255.0.0.0", interfaceIndex, 100);
                        //Ip4RouteTable.CreateRoute("172.31.0.0", "255.255.0.0", interfaceIndex, 100);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
         
    }


}

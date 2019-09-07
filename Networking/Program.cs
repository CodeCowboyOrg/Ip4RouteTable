using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetworkRoute
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Get Parameter
                int interfaceIndex = 0;
                if (args.Count() > 0) interfaceIndex = Int32.Parse(args[0]);

 
                //Print all the Network adaptors
                NicInterface.PrintAllNetworkInterface();
                //Print the Routing Table
                Ip4RouteTable.RoutePrint();

                if (!Ip4RouteTable.InterfaceIndexExists(interfaceIndex))
                {
                    Console.WriteLine("InterfaceIndex '{0}' does not exists.", interfaceIndex);
                    return;
                }

                //Determine if the Route Table has multiple 1.1.1.1 routes, indicating a VPN is active
                if (!Ip4RouteTable.RouteExists("1.1.1.1")) return;
                //Demostrate Deleting routes and Adding routes

                Ip4RouteEntry routeEntry = Ip4RouteTable.GetRouteEntry("1.1.1.1").First();
                //NetworkAdaptor na = NicInterface.GetNetworkAdaptor(interfaceIndex);
                if (routeEntry != null && routeEntry.GatewayIP.ToString().Length > 0)
                {
                    Console.WriteLine("Deleting VPN existing routes.");
                    Ip4RouteTable.DeleteRoute(interfaceIndex);
                    Ip4RouteTable.DeleteRoute("202.0.0.0");

                    Console.WriteLine("Adding VPN new route.");
                    Ip4RouteTable.CreateRoute("202.0.0.0", "255.0.0.0", interfaceIndex, 100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace.ToString());
                throw;
            }
        }
         
    }


}

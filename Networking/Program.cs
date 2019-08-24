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
                int interfaceIndex = Int32.Parse(args[0]);

                //Print all the Network adaptors
                NicInterface.PrintAllNetworkInterface();
                //Print the Routing Table
                Ip4RouteTable.RoutePrint();

                //Determine if the Route Table has multiple 1.1.1.1 routes, indicating a VPN is active
                if (!Ip4RouteTable.RouteExists("1.1.1.1")) return;
                //Demostrate Deleting routes and Adding routes

                NetworkAdaptor na = NicInterface.GetNetworkAdaptor(interfaceIndex);
                if (na != null && na.PrimaryGateway.Address.ToString().Length > 0)
                {
                    Console.WriteLine("Deleting VPN routes and adding new route.");
                    Ip4RouteTable.DeleteRoute(interfaceIndex);
                    Ip4RouteTable.DeleteRoute("202.0.0.0");
                    Ip4RouteTable.CreateRoute("202.0.0.0", "255.0.0.0", interfaceIndex, 100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
         
    }


}

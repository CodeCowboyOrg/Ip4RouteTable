using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Networking
{
    public class NicInterface
    {

        public static NetworkAdaptor GetNetworkAdaptor(int interfaceIndex)
        {
            NetworkAdaptor na = null;
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                if (properties.GetIPv4Properties().Index == interfaceIndex)
                {
                    na = new NetworkAdaptor();
                    na.Name = adapter.Name;
                    na.Description = adapter.Description;
                    na.MACAddress = adapter.GetPhysicalAddress().ToString();
                    na.PrimaryIpAddress = properties.UnicastAddresses.Where(i => i.Address.AddressFamily == AddressFamily.InterNetwork).First().Address;
                    na.SubnetMask = properties.UnicastAddresses.Where(i => i.Address.AddressFamily == AddressFamily.InterNetwork).First().IPv4Mask;
                    if (properties.GatewayAddresses.Count > 0)
                        na.PrimaryGateway = properties.GatewayAddresses.Where(i => i != null && i.Address != null).First().Address;

                }
            }
            return na;
        }

        public static List<NetworkAdaptor> GetAllNetworkAdaptor()
        {
            List<NetworkAdaptor> naList = new List<NetworkAdaptor>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                NetworkAdaptor na = new NetworkAdaptor();
                na.Name = adapter.Name;
                na.Description = adapter.Description;
                na.MACAddress = adapter.GetPhysicalAddress().ToString();
                na.InterfaceIndex = properties.GetIPv4Properties().Index;
                na.PrimaryIpAddress = properties.UnicastAddresses.Where(i => i.Address.AddressFamily == AddressFamily.InterNetwork).First().Address;
                na.SubnetMask = properties.UnicastAddresses.Where(i => i.Address.AddressFamily == AddressFamily.InterNetwork).First().IPv4Mask;
                if (properties.GatewayAddresses.Count > 0)
                    na.PrimaryGateway = properties.GatewayAddresses.Where(i => i != null && i.Address != null).First().Address;

                naList.Add(na);
            }
            return naList;
        }

        public static void PrintAllNetworkInterface()
        {
            List<NetworkAdaptor> naList = NicInterface.GetAllNetworkAdaptor();
            Console.WriteLine("{0,18} {1,18} {2,18} {3,20} {4,6} {5}", "IP Address", "Subnet Mask", "Gateway", "MAC", "IF", "Name");
            foreach (NetworkAdaptor na in naList)
            {
                Console.WriteLine("{0,18} {1,18} {2,18} {3,20} {4,6} {5}", na.PrimaryIpAddress, na.SubnetMask, na.PrimaryGateway, na.MACAddress, na.InterfaceIndex, na.Name);
            }
        }


    }

    public class NetworkAdaptor
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string MACAddress { get; set; }
        public int InterfaceIndex { get; set; }
        public IPAddress PrimaryIpAddress { get; set; }
        public IPAddress SubnetMask { get; set; }
        public IPAddress PrimaryGateway { get; set; }
    }
}

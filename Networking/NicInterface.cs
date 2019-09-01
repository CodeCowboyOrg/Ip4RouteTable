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
    public class NicInterface
    {

        public static NetworkAdaptor GetNetworkAdaptor(int interfaceIndex)
        {
            NetworkAdaptor na = null;
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                IPv4InterfaceProperties ip4Properties = properties.GetIPv4Properties();
                if (properties.GetIPv4Properties().Index == interfaceIndex)
                {
                    na = new NetworkAdaptor();
                    na.Name = adapter.Name;
                    na.Description = adapter.Description;
                    na.MACAddress = adapter.GetPhysicalAddress().ToString();
                    na.InterfaceIndex = ip4Properties.Index;
                    na.PrimaryIpAddress = properties.UnicastAddresses.Where(i => i.Address.AddressFamily == AddressFamily.InterNetwork).First().Address;
                    na.SubnetMask = properties.UnicastAddresses.Where(i => i.Address.AddressFamily == AddressFamily.InterNetwork).First().IPv4Mask;
                    if (properties.GatewayAddresses.Count > 0)
                    {
                        na.PrimaryGateway = null;
                        foreach (GatewayIPAddressInformation gatewayInfo in properties.GatewayAddresses)
                        {
                            if (gatewayInfo.Address != null && gatewayInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                na.PrimaryGateway = gatewayInfo.Address;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //if the gateways on the Network adaptor properties is null, then get it from the routing table, especially the case for VPN routers
                        List<Ip4RouteEntry> routeTable = Ip4RouteTable.GetRouteTable();
                        if (routeTable.Where(i => i.InterfaceIndex == na.InterfaceIndex).Count() > 0)
                        {
                            na.PrimaryGateway = routeTable.Where(i => i.InterfaceIndex == na.InterfaceIndex).First().GatewayIP;

                        }
                    }
                    //not ideal and incorrect, but hopefully it doesn't execute this as the gateways are defined elsewhere
                    //the correct way is to locate the primary gateway in some other property other than the 3 methods here
                    if (na.PrimaryGateway == null && properties.DhcpServerAddresses.Count > 0)
                    {
                        na.PrimaryGateway = properties.DhcpServerAddresses.First();
                    }
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
                IPv4InterfaceProperties ip4Properties = null;
                if (!HasIp4Support(adapter))
                {
                    continue;
                }
                else
                {
                    ip4Properties = properties.GetIPv4Properties();
                }

                NetworkAdaptor na = new NetworkAdaptor();
                na.Name = adapter.Name;
                na.Description = adapter.Description;
                na.MACAddress = adapter.GetPhysicalAddress().ToString();
                na.InterfaceIndex = ip4Properties != null ? ip4Properties.Index : 0;
                na.PrimaryIpAddress = properties.UnicastAddresses.Where(i => i.Address.AddressFamily == AddressFamily.InterNetwork).First().Address;
                na.SubnetMask = properties.UnicastAddresses.Where(i => i.Address.AddressFamily == AddressFamily.InterNetwork).First().IPv4Mask;
                if (properties.GatewayAddresses.Count > 0)
                {
                    na.PrimaryGateway = null;
                    foreach (GatewayIPAddressInformation gatewayInfo in properties.GatewayAddresses)
                    {
                        if (gatewayInfo.Address != null && gatewayInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            na.PrimaryGateway = gatewayInfo.Address;
                            break;
                        }
                    }
                }
                else
                {
                    //if the gateways on the Network adaptor properties is null, then get it from the routing table
                    List<Ip4RouteEntry> routeTable = Ip4RouteTable.GetRouteTable();
                    if (routeTable.Where(i => i.InterfaceIndex == na.InterfaceIndex).Count() > 0)
                    {
                        na.PrimaryGateway = routeTable.Where(i => i.InterfaceIndex == na.InterfaceIndex).First().GatewayIP;
                        
                    }
                }
                if (na.PrimaryGateway == null && properties.DhcpServerAddresses.Count > 0)
                {
                    na.PrimaryGateway = properties.DhcpServerAddresses.First();
                }
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

        private static bool HasIp4Support(NetworkInterface adapter)
        {
            {
                try
                {
                    adapter.GetIPProperties().GetIPv4Properties();
                    return true;
                }
                catch 
                {
                    return false;
                }
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

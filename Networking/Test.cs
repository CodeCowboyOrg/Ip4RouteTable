using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetworkRoute.Test
{
    class TestProgram
    { 


        public static void Test()
        {
            NicInterface.PrintAllNetworkInterface();
            Console.Read();
            ShowNetworkInfo();
            Console.Read();
            Ip4RouteTable.RoutePrint(Ip4RouteTable.GetRouteTable());
            Console.Read();

            Ip4RouteEntry routeEntry = new Ip4RouteEntry
            {
                DestinationIP = IPAddress.Parse("202.0.0.0"),
                SubnetMask = IPAddress.Parse("255.0.0.0"),
                GatewayIP = IPAddress.Parse("10.33.55.1"),
                InterfaceIndex = 11
            };
            //Ip4RouteTable.CreateRoute(routeEntry);
            Ip4RouteTable.DeleteRoute(42);
            Ip4RouteTable.CreateRoute("202.0.0.0", "255.0.0.0", 42, 100);
            Console.Read();
            Ip4RouteTable.RoutePrint(Ip4RouteTable.GetRouteTable());
            Console.Read();
            //Ip4RouteTable.DeleteRoute(routeEntry);
        }

        public static void ShowNetworkInfo()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                Console.WriteLine(adapter.Name);
                Console.WriteLine(adapter.Description);
                Console.WriteLine(adapter.GetPhysicalAddress().ToString());
                Console.WriteLine(properties.GetIPv4Properties().Index.ToString());
                Console.WriteLine(properties.UnicastAddresses.Where(i => i.Address.AddressFamily == AddressFamily.InterNetwork).ToList().Select(i => i.Address.ToString()).Aggregate((i, j) => i + "," + j));
                if (properties.GatewayAddresses.Count > 0)
                    Console.WriteLine(properties.GatewayAddresses.Where(i => i != null && i.Address != null).ToList().Select(i => i.Address.ToString()).Aggregate((i, j) => i + "," + j));
                Console.WriteLine("=======================================================================");
            }

        }

        public static void DisplayGatewayAddresses()
        {
            Console.WriteLine("Gateways");
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
                if (addresses.Count > 0)
                {
                    Console.WriteLine(adapter.Description);
                    foreach (GatewayIPAddressInformation address in addresses)
                    {
                        Console.WriteLine("  Gateway Address ......................... : {0}",
                            address.Address.ToString());
                    }
                    Console.WriteLine();
                }
            }
        }

        public static void ShowIPAddresses(IPInterfaceProperties adapterProperties)
        {
            IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
            if (dnsServers != null)
            {
                foreach (IPAddress dns in dnsServers)
                {
                    Console.WriteLine("  DNS Servers ............................. : {0}",
                        dns.ToString()
                   );
                }
            }
            IPAddressInformationCollection anyCast = adapterProperties.AnycastAddresses;
            if (anyCast != null)
            {
                foreach (IPAddressInformation any in anyCast)
                {
                    Console.WriteLine("  Anycast Address .......................... : {0} {1} {2}",
                        any.Address,
                        any.IsTransient ? "Transient" : "",
                        any.IsDnsEligible ? "DNS Eligible" : ""
                    );
                }
                Console.WriteLine();
            }

            MulticastIPAddressInformationCollection multiCast = adapterProperties.MulticastAddresses;
            if (multiCast != null)
            {
                foreach (IPAddressInformation multi in multiCast)
                {
                    Console.WriteLine("  Multicast Address ....................... : {0} {1} {2}",
                        multi.Address,
                        multi.IsTransient ? "Transient" : "",
                        multi.IsDnsEligible ? "DNS Eligible" : ""
                    );
                }
                Console.WriteLine();
            }
            UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
            if (uniCast != null)
            {
                string lifeTimeFormat = "dddd, MMMM dd, yyyy  hh:mm:ss tt";
                foreach (UnicastIPAddressInformation uni in uniCast)
                {
                    DateTime when;

                    Console.WriteLine("  Unicast Address ......................... : {0}", uni.Address);
                    Console.WriteLine("     Prefix Origin ........................ : {0}", uni.PrefixOrigin);
                    Console.WriteLine("     Suffix Origin ........................ : {0}", uni.SuffixOrigin);
                    Console.WriteLine("     Duplicate Address Detection .......... : {0}",
                        uni.DuplicateAddressDetectionState);

                    // Format the lifetimes as Sunday, February 16, 2003 11:33:44 PM
                    // if en-us is the current culture.

                    // Calculate the date and time at the end of the lifetimes.    
                    when = DateTime.UtcNow + TimeSpan.FromSeconds(uni.AddressValidLifetime);
                    when = when.ToLocalTime();
                    Console.WriteLine("     Valid Life Time ...................... : {0}",
                        when.ToString(lifeTimeFormat, System.Globalization.CultureInfo.CurrentCulture)
                    );
                    when = DateTime.UtcNow + TimeSpan.FromSeconds(uni.AddressPreferredLifetime);
                    when = when.ToLocalTime();
                    Console.WriteLine("     Preferred life time .................. : {0}",
                        when.ToString(lifeTimeFormat, System.Globalization.CultureInfo.CurrentCulture)
                    );

                    when = DateTime.UtcNow + TimeSpan.FromSeconds(uni.DhcpLeaseLifetime);
                    when = when.ToLocalTime();
                    Console.WriteLine("     DHCP Leased Life Time ................ : {0}",
                        when.ToString(lifeTimeFormat, System.Globalization.CultureInfo.CurrentCulture)
                    );
                }
                Console.WriteLine();
            }
        }
    }




}

# Ip4RouteTable
Windows IP4 Routing Table using WinAPI Calls
Mimicking the functionality of "route add" and "route delete" in C#

# Sample Usage
```C#
        static void Main(string[] args)
        {
            try
            {
                //Print all the Network adaptors
                NicInterface.PrintAllNetworkInterface();
                //Print the Routing Table
                Ip4RouteTable.RoutePrint();

                //Demostrate Deleting routes and Adding routes
                int interfaceIndex = 42; // Int32.Parse(args[0]);
                NetworkAdaptor na = NicInterface.GetNetworkAdaptor(interfaceIndex);
                if (na != null && na.PrimaryGateway.Address.ToString().Length > 0)
                {
                    Ip4RouteTable.DeleteRoute(interfaceIndex);
                    Ip4RouteTable.DeleteRoute("10.100.0.0");
                    Ip4RouteTable.CreateRoute("202.0.0.0", "255.0.0.0", interfaceIndex, 100);
                }
            }
            catch
            {

            }
        }
```

# Uses the WinAPI DLLs 
```C#
        internal struct PMIB_IPFORWARDROW
        {
            internal uint /*DWORD*/ dwForwardDest;
            internal uint /*DWORD*/ dwForwardMask;
            internal uint /*DWORD*/ dwForwardPolicy;
            internal uint /*DWORD*/ dwForwardNextHop;
            internal uint /*DWORD*/ dwForwardIfIndex;
            internal uint /*DWORD*/ dwForwardType;
            internal uint /*DWORD*/ dwForwardProto;
            internal uint /*DWORD*/ dwForwardAge;
            internal uint /*DWORD*/ dwForwardNextHopAS;
            internal uint /*DWORD*/ dwForwardMetric1;
            internal uint /*DWORD*/ dwForwardMetric2;
            internal uint /*DWORD*/ dwForwardMetric3;
            internal uint /*DWORD*/ dwForwardMetric4;
            internal uint /*DWORD*/ dwForwardMetric5;
        };
        
    internal static class NativeMethods
    {
        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        public extern static int GetIpForwardTable(IntPtr /*PMIB_IPFORWARDTABLE*/ pIpForwardTable, ref int /*PULONG*/ pdwSize, bool bOrder);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        //public extern static int CreateIpForwardEntry(ref /*PMIB_IPFORWARDROW*/ Ip4RouteTable.PMIB_IPFORWARDROW pRoute);  Can do by reference or by Pointer
        public extern static int CreateIpForwardEntry(IntPtr /*PMIB_IPFORWARDROW*/ pRoute);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        public extern static int DeleteIpForwardEntry(IntPtr /*PMIB_IPFORWARDROW*/ pRoute);
    }        
''''

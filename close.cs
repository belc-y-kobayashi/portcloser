using System;
using System.Runtime.InteropServices;
using System.Net;

public class TCPResetter
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPROW
    {
        public int dwState;
        public int dwLocalAddr;
        public int dwLocalPort;
        public int dwRemoteAddr;
        public int dwRemotePort;
    }

    [DllImport("iphlpapi.dll")]
    private static extern int SetTcpEntry(ref MIB_TCPROW pTcpRow);

    private const int MIB_TCP_STATE_DELETE_TCB = 12;

    public static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: close.exe <localAddr> <localPort> <remoteAddr> <remotePort>");
            return;
        }

        MIB_TCPROW row = new MIB_TCPROW
        {
            dwState = MIB_TCP_STATE_DELETE_TCB,
            dwLocalAddr = BitConverter.ToInt32(IPAddress.Parse(args[0]).GetAddressBytes(), 0),
            dwLocalPort = ConvertToNetworkOrder(int.Parse(args[1])),
            dwRemoteAddr = BitConverter.ToInt32(IPAddress.Parse(args[2]).GetAddressBytes(), 0),
            dwRemotePort = ConvertToNetworkOrder(int.Parse(args[3]))
        };

        int result = SetTcpEntry(ref row);
        if (result != 0)
        {
            Console.WriteLine($"Failed to reset TCP entry: {result}");
        }
    }

    private static int ConvertToNetworkOrder(int port)
    {
        return (int)IPAddress.HostToNetworkOrder((short)port);
    }
}

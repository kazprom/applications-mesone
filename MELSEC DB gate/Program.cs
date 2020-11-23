using System;
using System.Net.Sockets;

namespace MELSEC_DB_gate
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                TcpClient client = new TcpClient("192.168.0.252", 5000);

                String cmd = "";
                String OutAddress = "0001";
                cmd = "";
                cmd = cmd + "5000";// sub HEAD (NOT)
                cmd = cmd + "00";//   network number (NOT)
                cmd = cmd + "FF";//PLC NUMBER
                cmd = cmd + "03FF";// DEMAND OBJECT MUDULE I/O NUMBER
                cmd = cmd + "00";//  DEMAND OBJECT MUDULE DEVICE NUMBER
                cmd = cmd + "001C";//  Length of demand data
                cmd = cmd + "000A";//  CPU inspector data
                cmd = cmd + "0401";//  Read command
                cmd = cmd + "0000";//  Sub command
                cmd = cmd + "D*";//   device code
                cmd = cmd + "009500"; //adBase 
                cmd = cmd + OutAddress;  //BASE ADDRESS  

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(cmd);


                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", cmd);

                data = new Byte[256];

                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                stream.Close();
                client.Close();

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
    }
}

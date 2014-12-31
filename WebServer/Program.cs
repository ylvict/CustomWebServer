using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace WebServer
{
    /// <summary>      
    /// 实现一个简单的Web服务器
    /// 
    /// 该服务器向请求的浏览器返回一个静态的HTML页面 
    /// </summary>     
    class Program
    {
        static void Main(string[] args)
        {
            // 获得本机的Ip地址，即127.0.0.1    
            IPAddress localaddress = IPAddress.Loopback;
            // 创建可以访问的断点，49155表示端口号，如果这里设置为0，表示使用一个由系统分配的空闲的端口号   
            IPEndPoint endpoint = new IPEndPoint(localaddress, 49155);
            // 创建Socket对象,使用IPv4地址，数据通信类型为数据流，传输控制协议TCP协议.   
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //将Socket绑定到断点上           
            socket.Bind(endpoint);
            // 设置连接队列的长度 
            socket.Listen(10);
            while (true)
            {
                Console.WriteLine("Wait an connect Request...");
                // 开始监听，这个方法会堵塞线程的执行，直到接受到一个客户端的连接请求
                Socket clientsocket = socket.Accept();
                // 输出客户端的地址         
                Console.WriteLine("Client Address is: {0}", clientsocket.RemoteEndPoint);
                // 把客户端的请求数据读入保存到一个数组中        
                byte[] buffer = new byte[2048];
                int receivelength = clientsocket.Receive(buffer, 2048, SocketFlags.None);
                string requeststring = Encoding.UTF8.GetString(buffer, 0, receivelength);
                // 在服务器端输出请求的消息            
                Console.WriteLine(requeststring);
                // 服务器端做出相应内容                
                // 响应的状态行               
                string statusLine = "HTTP/1.1 200 OK\r\n";
                byte[] responseStatusLineBytes = Encoding.UTF8.GetBytes(statusLine);
                string responseBody = "<html><head><title>Default Page</title></head><body><p style='font:bold;font-size:24pt'>Welcome you</p></body></html>";
                string responseHeader = string.Format("Content-Type: text/html; charset=UTf-8\r\nContent-Length: {0}\r\n", responseBody.Length);
                byte[] responseHeaderBytes = Encoding.UTF8.GetBytes(responseHeader);
                byte[] responseBodyBytes = Encoding.UTF8.GetBytes(responseBody);
                // 向客户端发送状态行                
                clientsocket.Send(responseStatusLineBytes);
                // 向客户端发送回应头信息             
                clientsocket.Send(responseHeaderBytes);
                // 发送头部和内容的空行                 
                clientsocket.Send(new byte[] { 13, 10 });
                // 想客户端发送主体部分                
                clientsocket.Send(responseBodyBytes);
                // 断开连接               
                clientsocket.Close();
                Console.ReadKey();
                break;
            }
            // 关闭服务器         
            socket.Close();
        }
    }
}
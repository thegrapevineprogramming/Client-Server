/*
	Injector em C#
	Criado por Marcone em 2022
	Telegram: @thegrapevine
*/

// Importando os Módulos.
using System;
using System.Net;  
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;

class ClientServer {

      static void conecta(Socket c){
         Console.WriteLine("[#] Client Received!");

         var buffer = new byte[8192];
         c.Receive(buffer);
         string rq = Encoding.ASCII.GetString(buffer);
         Console.WriteLine("[#] Client Request: " + rq.Substring(0, rq.IndexOf("\0")));

         var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
         s.Connect("nl-2.hostip.co", 443);

         c.Send(Encoding.ASCII.GetBytes("HTTP/1.1 200 Established\r\n\r\n"));

         try {
            var io = new ArrayList();
            var data = new byte[8192];

            while(true){
               io.Add(c);
               io.Add(s);
               Socket.Select(io, null, null, 1000);
               foreach (var i in io){
                  if (i == s){
                     // Download
                     var tam = s.Receive(data);
                     if (tam==0){ throw new Exception();}
                     c.Send(data, 0, tam, SocketFlags.None);
                  } else if (i == c){
                     // Upload
                     var tam = c.Receive(data);
                     if (tam==0){ throw new Exception();}
                     s.Send(data, 0, tam, SocketFlags.None);
                  }
               }
               io.Clear();
            }

         } catch (Exception) {
            Console.WriteLine("[#] Client Disconnected!");
         } finally {
            c.Close();
            s.Close();
         }
      }

   static void Main(){
      // Listen
      var l = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      l.Bind(new IPEndPoint(0, 8088));
      l.Listen(0);
      Console.WriteLine("[#] Listen on IP and Port: 127.0.0.1:8088");

      while(true){
         var c = l.Accept();
         var t = new Thread(() => conecta(c));
         t.Start();
      }
   }
}
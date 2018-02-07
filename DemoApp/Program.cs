using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Channels.Serial;
using ComCommunication.Messages;
using ComCommunication.Messages.Simple;
using Logger;
using System.Threading;
using Interfaces;
using ComCommunication.Messages.Line;

namespace ComCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            Random r = new Random();

            p.ListPorts();
            var portname = p.GetPortName();


            //var channel = new SerialChannel<SimpleMessage>(
            //    portname, 
            //    new ConsoleLogger(), 
            //    new SimpleMessageSerialConverter(),
            //    new SimpleMessageSerialFinder(new SimpleMessageSerialConverter()));

            var channel = new SerialChannel<LineMessage>(
                portname, 
                new ConsoleLogger(), 
                new LineMessageSerialConverter(),
                new LineMessageSerialFinder(new LineMessageSerialConverter()));

            channel.MessageReceived += HandleIncomingMessageEvent;
            channel.Open();

            var key = Console.ReadKey();

            while (key.KeyChar != 'q')
            {
                switch(key.KeyChar)
                {
                    case 's':
                        for (int i = 0; i < 20; i++)
                        {
                            Thread.Sleep(Math.Max((r.Next(1000)-500), 0));
                            //channel.Send(new SimpleMessage() { Data = new byte[] { 65, (byte)(i), 66 } });
                            channel.Send(new LineMessage() { Data = $"Hello World {i}!" });
                        }
                        break;
                    case 'o':
                        channel.Open();
                        break;
                    case 'c':
                        channel.Close();
                        break;

                }
                key = Console.ReadKey();
            }

            Console.ReadKey();
        }

        private static void HandleIncomingMessageEvent(object sender, dynamic e)
        {
            Console.WriteLine($"Got Msg: {e.Data} ");
        }

        public void ListPorts()
        {
            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }
        }

        public string GetPortName()
        {
            Console.Write("Enter COM port value: " );
            var portName = Console.ReadLine();
            if (!portName.ToLower().StartsWith("com"))
            {
                portName = "COM" + portName;
            }
            return portName;
        }

    }
}

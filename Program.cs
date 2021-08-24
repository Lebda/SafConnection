using System;

namespace SafConnection
{
    class Program
    {
        static bool isDebug = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello from SAF Connection!");
            if (args.Length == 0)
            {
                Console.WriteLine("No args provided !");
                if (isDebug)
                {
                    Console.WriteLine("Press any key for EXIT !");
                    Console.ReadKey();
                }
                return;
            }
            if (isDebug)
            {
                Console.WriteLine("Press any key for connection !");
                Console.ReadKey();
            }
            var communicator = new ThirdPartyCommunicator();
            communicator.Connect(args[0]);
            var fileInfo = communicator.ReadAllSafCommand_Execute();

            if (isDebug)
            {
                Console.WriteLine("Press any key for watching !");
                Console.ReadKey();
            }
            var safConnector = new ThirdPartySafConnector();
            safConnector.ConnectSAF(fileInfo, communicator.WriteAllSafCommand_Execute);


            Console.WriteLine("Press any key for EXIT FINAL !");
            Console.ReadKey();
        }
    }
}

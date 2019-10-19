using System;
using System.Threading.Tasks;

namespace RPCClient
{
    class Rpc
    {
        static void Main(string[] args)
        {
            Console.WriteLine("RPC Client");
            string n = args.Length > 0 ? args[0] : "10";
            Task t = InvokeAsync(n);
            t.Wait();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static async Task InvokeAsync(string n)
        {
            var rpcClient = new RpcClient();

            Console.WriteLine(" [x] Requesting Fibonacci({0})", n);
            var response = await rpcClient.CallAsync(n.ToString());
            Console.WriteLine(" [.] Got '{0}'", response);

            rpcClient.Close();
        }
    }
}

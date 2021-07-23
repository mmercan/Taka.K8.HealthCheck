using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using k8s;

namespace Taka.K8s
{
    public interface IKubernetesClient
    {
        Kubernetes Client { get; }
    }

    public static class KubernetesClientHelper
    {
        public static void SetTcpKeepAlives(k8s.Kubernetes client)
        {
            var realHandler = client.HttpMessageHandlers.FirstOrDefault(h => !(h is DelegatingHandler));
            if (!(realHandler is HttpClientHandler))
            {
                throw new ArgumentException("Expected HttpClientHandler");
            }

            var underlyingHandlerProperty = realHandler.GetType().GetField("_underlyingHandler", BindingFlags.NonPublic | BindingFlags.Instance);
            if (underlyingHandlerProperty == null)
            {
                throw new NullReferenceException("Expected _underlyingHandler property not found.");
            }

            var underlyingHandler = underlyingHandlerProperty.GetValue(realHandler);
            if (underlyingHandler == null)
            {
                throw new NullReferenceException("_underlyingHandler is null.");
            }

            if (underlyingHandler is SocketsHttpHandler socketHandler)
            {
                // we reached the SocketsHttpHandler, enable the keepalive delay.
                socketHandler.KeepAlivePingDelay = TimeSpan.FromSeconds(10);
            }
            else
            {
                throw new ArgumentException($"Expected to find SocketsHttpHandler, but found: {underlyingHandler.GetType().Name}");
            }
        }
    }

    public class KubernetesClientFromConfigFile : IKubernetesClient
    {
        public Kubernetes Client { get; }
        public KubernetesClientFromConfigFile()
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            Client = new Kubernetes(config);
            KubernetesClientHelper.SetTcpKeepAlives(Client);
        }


    }


    public class KubernetesClientInClusterConfig : IKubernetesClient
    {
        public Kubernetes Client { get; }
        public KubernetesClientInClusterConfig()
        {
            var config = KubernetesClientConfiguration.InClusterConfig();
            Client = new Kubernetes(config);
            KubernetesClientHelper.SetTcpKeepAlives(Client);

            // Client = new Kubernetes(config);
            // {
            //     var hf = typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.Instance | BindingFlags.NonPublic);
            //     var h = hf.GetValue(Client.HttpClient);
            //     while (!(h is HttpClientHandler))
            //     {
            //         h = ((DelegatingHandler)h).InnerHandler;
            //     }
            //     var sh = new SocketsHttpHandler();
            //     sh.ConnectCallback = async (context, token) =>
            //     {
            //         var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
            //         {
            //             NoDelay = true,
            //         };
            //         socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            //         var ip = Dns.GetHostEntry(context.DnsEndPoint.Host).AddressList.First();
            //         var ep = new IPEndPoint(ip, context.DnsEndPoint.Port);
            //         // Console.WriteLine(ep);
            //         await socket.ConnectAsync(ep, token).ConfigureAwait(false);
            //         return new NetworkStream(socket, ownsSocket: true);
            //     };
            //     var p = h.GetType().GetField("_underlyingHandler", BindingFlags.NonPublic | BindingFlags.Instance);
            //     p.SetValue(h, (sh));
            // }

            // {
            //     var m = Client.GetType().GetMethod("InitializeFromConfig", BindingFlags.NonPublic | BindingFlags.Instance);
            //     m.Invoke(Client, new[] { config });
            // }

        }
    }
}
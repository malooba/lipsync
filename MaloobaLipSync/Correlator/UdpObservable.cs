using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;

namespace MaloobaLipSync.Correlator
{
    static class UdpObservable
    {
        public static IObservable<byte[]> Create(IPEndPoint ep)
        {
            return Observable.Create<byte[]>(async (observer, token) =>
            {
                using(var listener = new UdpClient(ep))
                {
                    UdpReceiveResult result;
                    while((result = await listener.ReceiveAsync()) != null)
                    {
                        if(token.IsCancellationRequested)
                            break;
                        observer.OnNext(result.Buffer);
                    }
                    observer.OnCompleted();
                }
            });
        } 
    }
}

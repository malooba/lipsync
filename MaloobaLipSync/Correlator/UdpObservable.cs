//Copyright 2016 Malooba Ltd

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

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

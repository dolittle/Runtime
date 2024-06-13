// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;

namespace Integration.Shared;

public static class Portable
{
    public static (int,int,int) FindFreePorts()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        var listener2 = new TcpListener(IPAddress.Loopback, 0);
        var listener3 = new TcpListener(IPAddress.Loopback, 0);
        try
        {
            // Start the listener to obtain the port.
            listener.Start();
            listener2.Start();
            listener3.Start();
            // Get the port number assigned by the system.
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            var port2 = ((IPEndPoint)listener2.LocalEndpoint).Port;
            var port3 = ((IPEndPoint)listener3.LocalEndpoint).Port;
            return (port, port2, port3);
        }
        finally
        {
            // Stop the listener.
            listener.Stop();
            listener2.Stop();
            listener3.Stop();
        }
    }
}

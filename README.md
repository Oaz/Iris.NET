# Iris.NET
[![Iris.NET on NuGet](https://img.shields.io/badge/source-browser-2B9CC6.svg?style=flat-square)](http://sourcebrowser.io/Browse/tommasobertoni/Iris.NET)
[![Iris.NET on NuGet](https://img.shields.io/badge/nuget-v0.1.0-blue.svg?style=flat-square)](https://www.nuget.org/packages/Iris.NET/)
<br><br>
Iris.NET is a TCP-based, Pub/Sub C# library. It was developed to allow an easy-to-use, channel-separated communication on a LAN infrastructure.
<br><br>
The library provides a server and two client types: one for network communication and one for local communication.
<br>
The client types are also called *nodes* and they implement the [**IIrisNode**](/Iris.NET/Iris.NET.Common/Nodes/IIrisNode.cs) interface, which defines the pub/sub methods (basically: *Subscribe*, *Unsubscribe* and *Send*).
<br><br>
The **IrisClientNode** type is responsible for the network communication and talks to the **IrisServer** through a socket connection. It can be found in the *Iris.NET.Client* namespace.
<br><br>
The **IrisServerLocalNode** type, instead, is used to communicate locally (without sockets) with the same pub/sub infrastructure.
<br>
The **IrisServer** type handles the connections coming from the client nodes. This and IrisServerLocalNode can be found in the *Iris.NET.Server* namespace.
<br><br>
## *How To*
- Create and start a server:<br>
```csharp
IrisServer server = new IrisServer();
server.Start(22000); // Start on port 22000
```
- Create and connect a network node:<br>
```csharp
IrisClientConfig config = new IrisClientConfig() { Hostname = "127.0.0.1", Port = 22000 };
IrisClientNode node = new IrisClientNode();
node.Connect(config);
```
- Subscribe to a channel
```csharp
IDisposableSubscription subscription = node.Subscribe("worldnews", MyContentHandler);
```
- Send a message
```csharp
node.Send("worldnews", "something good happened");
```
<br>
A broader documentation is available in the ***[How-To](https://github.com/tommasobertoni/Iris.NET/wiki/How-To)*** page in the wiki.
<br>
To see the new features that are being developed and the breaking changes head over to the ***[Changelog](https://github.com/tommasobertoni/Iris.NET/wiki/Changelog)*** in the wiki.
<br><br><br>
## Technology info
This project was developed using *Visual Studio 2015* and *C# 6*.<br>
**The library targets the .NET Framework 4.0.**
<br><br><br>
## License
**Iris.NET** is distributed under the [MIT License](https://github.com/tommasobertoni/Iris.NET/blob/master/LICENCE).

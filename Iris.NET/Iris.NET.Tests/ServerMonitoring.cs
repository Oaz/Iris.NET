using System;
using NUnit.Framework;
using Iris.NET.Server;
using System.Threading;
using System.Net;
using System.Reflection;

namespace Iris.NET.Tests
{
	[TestFixture]
    public class ServerMonitoring
    {
        IrisServer _theServer;

		[SetUp]
        public void Initialize()
        {
            _theServer = new IrisServer();
        }

        [TearDown]
        public void Cleanup()
        {
            if(_theServer.IsRunning)
                _theServer.Stop();
        }

		[Test]
        public void ServerProperties()
        {
            Assert.AreEqual(false, _theServer.IsRunning);
            Assert.AreEqual(null, _theServer.Port);
            Assert.AreEqual(null, _theServer.Address);
            _theServer.Start(56666);
            Assert.AreEqual(true, _theServer.IsRunning);
            Assert.AreEqual(56666, _theServer.Port);
            Assert.AreEqual(new IPAddress(0), _theServer.Address);
            _theServer.Stop();
            Assert.AreEqual(false, _theServer.IsRunning);
            Assert.AreEqual(null, _theServer.Port);
            Assert.AreEqual(null, _theServer.Address);
        }

		[Test]
        public void ServerIdAreUnique()
        {
            var otherServer = new IrisServer();
            Assert.AreNotEqual(otherServer.Id, _theServer.Id);
        }

		[Test]
        public void ServerStartCanBeMonitored()
        {
            var monitorStart = new AutoResetEvent(false);
            _theServer.OnStarted += () => monitorStart.Set();
            _theServer.Start(56666);
            Assert.IsTrue(monitorStart.WaitOne(1000), "Server was not started");
        }

		[Test]
        public void ServerStopCanBeMonitored()
        {
            var monitorStop = new AutoResetEvent(false);
            _theServer.OnStopped += () => monitorStop.Set();
            _theServer.Start(56666);
            _theServer.Stop();
            Assert.IsTrue(monitorStop.WaitOne(1000), "Server was not stopped");
        }

		[Test]
        public void ServerFailureCanBeMonitored()
        {
            var monitorFailure = new AutoResetEvent(false);
            _theServer.OnServerException += ex =>
            {
                Assert.IsNotNull(ex.Message);
                monitorFailure.Set();
            };
            _theServer.Start(56666);
            SimulateFailureByClosingTheServerTcpListener();
            Assert.IsTrue(monitorFailure.WaitOne(2000), "Server failure was not detected");
        }

        private void SimulateFailureByClosingTheServerTcpListener()
        {
            var listener = (System.Net.Sockets.TcpListener) typeof(IrisServer).GetField(
                "_serverSocket",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField
            ).GetValue(_theServer);
            listener.Stop();
        }
    }
}

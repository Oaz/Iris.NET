using System;
using NUnit.Framework;
using Iris.NET.Server;
using System.Threading;
using System.Threading.Tasks;
using Iris.NET.Client;

namespace Iris.NET.Tests
{
	[TestFixture]
    public class SimplePublicationSubscription
    {
		[Test]
        public async Task BetweenLocalNodes()
        {
            await PubSub<IrisLocalNode, IrisServerConfig, IrisLocalNode, IrisServerConfig>(
                _theServer.GetServerConfig(),
                _theServer.GetServerConfig());
        }

		[Test]
        public async Task BetweenClientNodes()
        {
            await PubSub<IrisClientNode, IrisClientConfig, IrisClientNode, IrisClientConfig>(
                _theClientConfig,
                _theClientConfig);
        }
		[Test]
        public async Task FromClientToLocalNode()
        {
            await PubSub<IrisClientNode, IrisClientConfig, IrisLocalNode, IrisServerConfig>(
                _theClientConfig,
                _theServer.GetServerConfig());
        }

        [Test]
        public async Task FromLocalToClientNode()
        {
            await PubSub<IrisLocalNode, IrisServerConfig, IrisClientNode, IrisClientConfig>(
                _theServer.GetServerConfig(),
                _theClientConfig);
        }

        public async Task PubSub<PUBNODE, PUBCONF,SUBNODE, SUBCONF>(PUBCONF pubconf, SUBCONF subconf)
            where SUBNODE : AbstractIrisNode<SUBCONF>, new()
            where SUBCONF : IrisBaseConfig
            where PUBNODE : AbstractIrisNode<PUBCONF>, new()
            where PUBCONF : IrisBaseConfig
        {
            var received = new AutoResetEvent(false);
            var subnode = new SUBNODE();
            subnode.Connect(subconf);
            using (var subscription = await subnode.Subscribe("TheFooChannel", (content, hook) =>
            {
                Assert.AreEqual("Foo ==> Bar", content);
                received.Set();
            }))
            {
                Assert.IsNotNull(subscription);
                var pubnode = new PUBNODE();
                pubnode.Connect(pubconf);
                var publicationIsOk = await pubnode.Publish("TheFooChannel", "Foo ==> Bar");
                Assert.IsTrue(publicationIsOk);
                Assert.IsTrue(received.WaitOne(5000), "Content was not received on subscription");
            }
        }

        IrisServer _theServer;
        IrisClientConfig _theClientConfig;

		[SetUp]
        public void Initialize()
        {
            _theServer = new IrisServer();
            _theServer.Start(56666);
            _theClientConfig = new IrisClientConfig() { Hostname = "127.0.0.1", Port = 56666 };
        }

        [TearDown]
        public void Cleanup()
        {
            _theServer.Stop();
        }
    }
}

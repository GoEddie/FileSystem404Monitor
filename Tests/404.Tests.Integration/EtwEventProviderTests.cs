using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Diagnostics.Tracing;
using Diagnostics.Tracing.Parsers;
using Moq;
using NUnit.Framework;
using _404.Etw;

namespace _404.Tests.Integration
{
    
    [TestFixture]
    class EtwEventProviderTests
    {
        [Test]
        public void Provides_Events()
        {
            // I don't normally liked times tests but am struggling a little as the etw stuff hangs on the calling thread and sends events on a seperate thread
            
            var done = false;

            var aggregator = new Mock<EventAggregator>();
            aggregator.Setup(p => p.TraceEventAvailable(It.IsAny<TraceEvent>())).Callback((TraceEvent te) =>
            {
                done = true;
            });

            const string sessionName = "FileMon404Tests";

            var provider = new EtwEventProvider(sessionName, "Microsoft-Windows-Kernel-File", 0x10c0, aggregator.Object);
            
            var task = new Task(() => {
                 Assert.True(provider.Start());
            });

            task.Start();

            var start = DateTime.Now;

            while (!done && start > DateTime.Now.Subtract(Seconds(10)))
            {
                Thread.Sleep(Seconds(1));
            }

            provider.Stop();

            Assert.False(task.IsFaulted);

            Assert.True(done, "Did not receive an event before the timeout");

            
        }

        private static TimeSpan Seconds(int number)
        {
            return new TimeSpan(0,0, 0, number);
        }
    }
    
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using _404.ProcessStore;

namespace _404.Tests.Integration
{
    [TestFixture]
    class ProcessLookupTests
    {

        [Test]
        public void Retrieves_Correct_Process_Name_Before_Trace_Was_Started()
        {
            var process = Process.Start("cmd", "/c waitfor /T 10 kkkkkkk");
            var eventTime = DateTime.Now;
           
            var processStore = new ProcessNameStore();
            var name = processStore.ProcessName(process.Id, eventTime);

            process.Kill();

            Assert.AreEqual("cmd", name);
        }


        [Test]
        public void Retrieves_Correct_Process_Name_During_Trace()
        {
            var processStore = new ProcessNameStore();
            

            var process = Process.Start("waitfor", "/T 10 tttttt");
            var eventTime = DateTime.Now;
            
            Thread.Sleep(5000);

            var name = processStore.ProcessName(process.Id, eventTime);
            
            Assert.AreEqual("conhost", Path.GetFileNameWithoutExtension(name));
        }

    }
}

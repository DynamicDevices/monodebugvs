using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Debugger.Soft;

namespace DynamicDevices.MonoDebugVS.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void LaunchMonoDebugProcess()
        {
            var processStartInfo = new ProcessStartInfo("..\\..\\..\\HelloWorldConsole.exe", null);


            var opt = new LaunchOptions
                          {
                              CustomProcessLauncher = myProcessLauncher
                          };

            var vm = VirtualMachineManager.Launch(processStartInfo, opt);

            Thread.Sleep(1000);

            vm.Exit(0);
        }

        private void LaunchCallback(IAsyncResult o)
        {
        
        }

        private Process myProcessLauncher(ProcessStartInfo info)
        {
            var cmd = "C:\\Program Files (x86)\\Mono-2.10.9\\bin\\mono";

            var args = info.Arguments + " " + info.FileName;

            var process = Process.Start(cmd, args);

            return process;
        }
    }

}

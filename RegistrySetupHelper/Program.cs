using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDevices.MonoDebugVS;

namespace RegistrySetupHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            Connect.SetupRegistry();
        }
    }
}

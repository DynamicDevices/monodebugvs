using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDevices.MonoDebugVS
{
    public class ComponentException : Exception
    {
        public ComponentException(int hResult)
        {
            HResult = hResult;
        }

        public int HResult { get; private set; }
    }
}

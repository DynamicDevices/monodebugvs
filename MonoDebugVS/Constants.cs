using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDevices.MonoDebugVS
{
    public class Constants
    {
        public const int S_OK = 0;
        public const int S_FALSE = 1;
        
        public const int E_NOTIMPL = unchecked((int)0x80004001L);
        public const int E_FAIL = unchecked((int)0x80004005L);

        public static readonly int E_WIN32_INVALID_NAME = unchecked((int)HRESULT_FROM_WIN32(123));
        public static readonly int E_WIN32_ALREADY_INITIALIZED = unchecked((int)HRESULT_FROM_WIN32(1247));
        
        public const int RPC_E_SERVERFAULT = unchecked((int)0x80010105L);

        private const int FACILITY_WIN32 = 7;

        private static uint HRESULT_FROM_WIN32(uint x) { return (uint)(x) <= 0 ? (uint)(x) : (uint) (((x) & 0x0000FFFF) | (FACILITY_WIN32 << 16) | 0x80000000);}

    }
}

namespace DebuggerInterop.Core
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack=8)]
    public struct _ULARGE_INTEGER
    {
        public ulong QuadPart;
    }
}


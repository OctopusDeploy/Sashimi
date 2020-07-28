using System;

namespace Sashimi.Server.Contracts.Calamari
{
    /// <summary>
    /// If Min and Max WindowsNetFramework are null then only NetCore is supported.
    /// </summary>
    public class SupportedWindowsNetFrameworks
    {
        public SupportedWindowsNetFrameworks(WindowsNetFramework? minWindowsNetFramework, WindowsNetFramework? maxWindowsNetFramework, NetCoreSupport netCoreSupport)
        {
            if (minWindowsNetFramework == null && maxWindowsNetFramework == null && netCoreSupport == NetCoreSupport.NotSupported)
                throw new ArgumentException("Null min and max frameworks and no NetCore support is invalid. Please use the CalamariFlavour constructor that only requires an Id.");
            MinWindowsNetFramework = minWindowsNetFramework;
            MaxWindowsNetFramework = maxWindowsNetFramework;
            NetCoreSupport = netCoreSupport;
        }

        public SupportedWindowsNetFrameworks(NetCoreSupport netCoreSupport) : this (null, null, netCoreSupport)
        {
        }

        public WindowsNetFramework? MinWindowsNetFramework { get; }
        public WindowsNetFramework? MaxWindowsNetFramework { get; }
        public NetCoreSupport NetCoreSupport { get; }
    }
}
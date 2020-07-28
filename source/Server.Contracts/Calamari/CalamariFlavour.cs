namespace Sashimi.Server.Contracts.Calamari
{
    public class CalamariFlavour
    {
        /// <summary>
        /// This constructs a CalamariFlavour that does not support running on Windows
        /// </summary>
        public CalamariFlavour(string id)
        {
            Id = id;
        }

        public CalamariFlavour(string id, WindowsNetFramework minWindowsNetFramework, WindowsNetFramework maxWindowsNetFramework, NetCoreSupport netCoreSupport)
        {
            Id = id;
            SupportedWindowsNetFrameworks = new SupportedWindowsNetFrameworks(minWindowsNetFramework, maxWindowsNetFramework, netCoreSupport);
        }

        public CalamariFlavour(string id, NetCoreSupport netCoreSupport)
        {
            Id = id;
            SupportedWindowsNetFrameworks = new SupportedWindowsNetFrameworks(netCoreSupport);
        }

        public string Id { get; }
        public SupportedWindowsNetFrameworks? SupportedWindowsNetFrameworks { get; }
    }
}
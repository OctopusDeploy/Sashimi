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

        public string Id { get; }
    }
}
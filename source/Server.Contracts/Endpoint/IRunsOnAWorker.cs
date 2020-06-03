namespace Sashimi.Server.Contracts.Endpoint
{
    public interface IRunsOnAWorker
    {
        string DefaultWorkerPoolId { get; set; }
    }
}
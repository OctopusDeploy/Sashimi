using System.Collections.Generic;
using Newtonsoft.Json;
using Sashimi.Server.Contracts.Variables;

namespace Sashimi.Server.Contracts.Endpoint
{
    public abstract class Endpoint
    {
        //TODO: Remove this when we've done the migration
        [JsonIgnore]
        public abstract CommunicationStyle CommunicationStyle { get; }

        [JsonIgnore]
        public abstract DeploymentTargetType DeploymentTargetType { get; }

        [JsonIgnore]
        public abstract string Description { get; }

        [JsonIgnore]
        public virtual bool ScriptConsoleSupported => false;

        public abstract IEnumerable<(string id, DocumentType documentType)> GetRelatedDocuments();

        [JsonIgnore]
        public bool AlwaysRequiresAWorker => this is IRunsOnAWorker;

        public abstract IEnumerable<Variable> ContributeVariables();
    }
}

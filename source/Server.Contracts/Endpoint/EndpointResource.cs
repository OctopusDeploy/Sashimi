﻿using Octopus.Data.Resources;

 namespace Sashimi.Server.Contracts.Endpoint
{
    public abstract class EndpointResource : Resource
    {
        public abstract CommunicationStyle CommunicationStyle { get; }
    }
}
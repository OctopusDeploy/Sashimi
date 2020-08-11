﻿using System;
using System.Threading.Tasks;
using Microsoft.Rest.Azure;
using Octopus.Diagnostics;

namespace Octopus.Server.Web.Api.Actions
{
    public abstract class AzureWebSiteActionBase
    {
        protected AzureWebSiteActionBase(ILog log)
        {
            Log = log;
        }

        ILog Log { get; }

        protected async Task<TReturn> ThrowIfNotSuccess<TResponse, TReturn>(Func<Task<AzureOperationResponse<TResponse>>> azureResponse, Func<AzureOperationResponse<TResponse>, TReturn> onSuccess, string errorMessage)
        {
            AzureOperationResponse<TResponse> operationResponse;
            try
            {
                operationResponse = await azureResponse().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Warn(e, errorMessage);
                throw new Exception(errorMessage);
            }

            if (operationResponse.Response.IsSuccessStatusCode)
            {
                return onSuccess(operationResponse);
            }

            Log.Warn($"{errorMessage}{Environment.NewLine}Response status code does not indicate success: {operationResponse.Response.StatusCode} ({operationResponse.Response.ReasonPhrase}).");
            throw new Exception(errorMessage);
        }

        protected class AzureWebSiteResource
        {
            private AzureWebSiteResource(string name, string region)
            {
                Name = name;
                Region = region;
            }

            public string Name { get; }

            // ReSharper disable once MemberCanBePrivate.Local this must be public or the serialization to the client doesn't work
            public string? WebSpace { get; set; }

            // ReSharper disable once MemberCanBePrivate.Local this must be public or the serialization to the client doesn't work
            public string? ResourceGroup { get; set; }

            public string Region { get; }

            public static AzureWebSiteResource ForServiceManagement(string name, string webSpace, string region)
            {
                // Try and imply the resource group from the WebSpace.
                var resourceNameIndex = webSpace.LastIndexOf("-", StringComparison.Ordinal);
                var resourceGroupName = resourceNameIndex != -1 ? webSpace.Substring(0, resourceNameIndex) : null;
                return new AzureWebSiteResource(name, region) { WebSpace = webSpace, ResourceGroup = resourceGroupName };
            }

            public static AzureWebSiteResource ForResourceManagement(string name, string resourceGroup, string region)
            {
                return new AzureWebSiteResource(name, region) { ResourceGroup = resourceGroup };
            }
        }
    }
}
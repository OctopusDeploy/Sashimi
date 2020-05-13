using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CloudFormation;
using Amazon.CloudFormation.Model;
using Amazon.Runtime;
using Calamari.Aws.Exceptions;
using Calamari.Aws.Integration.CloudFormation;
using Octopus.CoreUtilities;
using Octopus.CoreUtilities.Extensions;

namespace Calamari.Aws.Deployment.CloudFormation
{
    public abstract class CloudFormationServiceBase
    {
        readonly StackEventLogger stackEventLogger;

        protected readonly ILog log;

        protected CloudFormationServiceBase(ILog log)
        {
            this.log = log;

            stackEventLogger = new StackEventLogger(log);
        }

        /// <summary>
        /// The AmazonServiceException can hold additional information that is useful to include in
        /// the log.
        /// </summary>
        /// <param name="exception">The exception</param>
        protected void LogAmazonServiceException(AmazonServiceException exception)
        {
            exception.GetWebExceptionMessage()
                .Tee(message => DisplayWarning("AWS-CLOUDFORMATION-ERROR-0014", message));
        }

        /// <summary>
        /// Run an action and log any AmazonServiceException detail.
        /// </summary>
        /// <param name="func">The exception</param>
        protected T WithAmazonServiceExceptionHandling<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (AmazonServiceException exception)
            {
                LogAmazonServiceException(exception);
                throw;
            }
        }

        /// <summary>
        /// Run an action and log any AmazonServiceException detail.
        /// </summary>
        /// <param name="action">The action to invoke</param>
        protected void WithAmazonServiceExceptionHandling(Action action)
        {
            WithAmazonServiceExceptionHandling<ValueTuple>(() => default(ValueTuple).Tee(x => action()));
        }


        /// <summary>
        /// Display an warning message to the user (without duplicates)
        /// </summary>
        /// <param name="errorCode">The error message code</param>
        /// <param name="message">The error message body</param>
        /// <returns>true if it was displayed, and false otherwise</returns>
        protected bool DisplayWarning(string errorCode, string message)
        {
            return stackEventLogger.Warn(errorCode, message);
        }

        /// <summary>
        /// Creates a handler which will log stack events and throw on common rollback events
        /// </summary>
        /// <param name="clientFactory">The client factory</param>
        /// <param name="stack">The stack to query</param>
        /// <param name="filter">The filter for stack events</param>
        /// <param name="expectSuccess">Whether we expected a success</param>
        /// <param name="missingIsFailure"></param>
        /// <returns>Stack event handler</returns>
        protected Action<Maybe<StackEvent>> LogAndThrowRollbacks(IAmazonCloudFormation client, StackArn stack, bool expectSuccess = true, bool missingIsFailure = true, Func<StackEvent, bool> filter = null)
        {
            return @event =>
            {
                try
                {
                    stackEventLogger.Log(@event);
                    stackEventLogger.LogRollbackError(@event, x =>
                            WithAmazonServiceExceptionHandling(() => client.GetLastStackEvent(stack, x).GetAwaiter().GetResult()),
                        expectSuccess,
                        missingIsFailure);
                }
                catch (PermissionException exception)
                {
                    Log.Warn(exception.Message);
                }
            };
        }

        static (IList<string> valid, IList<string> excluded) ExcludeUnknownIamCapabilities(
            IEnumerable<string> capabilities)
        {
            return capabilities.Aggregate((new List<string>(), new List<string>()), (prev, current) =>
            {
                var (valid, excluded) = prev;

                if (current.IsKnownIamCapability())
                    valid.Add(current);
                else
                    excluded.Add(current);

                return prev;
            });
        }

        protected static (IList<string> valid, IList<string> excluded) ExcludeAndLogUnknownIamCapabilities(IEnumerable<string> values)
        {
            var (valid, excluded) = ExcludeUnknownIamCapabilities(values);
            if (excluded.Count > 0)
            {
                Log.Warn($"The following unknown IAM Capabilities have been removed: {String.Join(", ", excluded)}");
            }
            return (valid, excluded);
        }

        protected void SetOutputVariable(IVariables variables, string name, string value)
        {
            Log.SetOutputVariable($"AwsOutputs[{name}]", value ?? "", variables);
            Log.Info($"Saving variable \"Octopus.Action[{variables["Octopus.Action.Name"]}].Output.AwsOutputs[{name}]\"");
        }

        protected Task<Stack> QueryStackAsync(IAmazonCloudFormation client, StackArn stack)
        {
            try
            {
                return client.DescribeStackAsync(stack);
            }
            catch (AmazonServiceException ex) when (ex.ErrorCode == "AccessDenied")
            {
                throw new PermissionException(
                    "The AWS account used to perform the operation does not have the required permissions to describe the CloudFormation stack. " +
                    "This means that the step is not able to generate any output variables.\n " +
                    "Please ensure the current account has permission to perform action 'cloudformation:DescribeStacks'.\n" +
                    ex.Message + "\n" +
                    ex);
            }
            catch (AmazonServiceException ex)
            {
                throw new Exception("An unrecognised exception was thrown while querying the CloudFormation stacks.", ex);
            }
        }
    }
}
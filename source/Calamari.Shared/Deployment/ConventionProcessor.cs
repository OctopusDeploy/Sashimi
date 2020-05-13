using System;
using System.Collections.Generic;
using System.Linq;
using Calamari.Commands.Support;
using Calamari.Deployment.Conventions;
using Octostache.Templates;

namespace Calamari.Deployment
{
    public class ConventionProcessor
    {
        readonly RunningDeployment deployment;
        readonly List<IConvention> conventions;

        public ConventionProcessor(RunningDeployment deployment, List<IConvention> conventions)
        {
            this.deployment = deployment;
            this.conventions = conventions;
        }

        public void RunConventions()
        {
            try
            {
                // Now run the "conventions", for example: Deploy.ps1 scripts, XML configuration, and so on
                RunInstallConventions();
            }
            catch (Exception installException)
            {
                if (installException is CommandException || installException is RecursiveDefinitionException)
                    Console.Error.WriteLine(installException.Message);
                else
                    Console.Error.WriteLine(installException);

                Console.Error.WriteLine("Running rollback conventions...");

                deployment.Error(installException);

                throw;
            }
        }
        

        void RunInstallConventions()
        {
            foreach (var convention in conventions.OfType<IInstallConvention>())
            {
                convention.Install(deployment);

                if (deployment.Variables.GetFlag(SpecialVariables.Action.SkipRemainingConventions))
                {
                    break;
                }
            }
        }
    }
}
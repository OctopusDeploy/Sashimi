using System;
using Amazon.IdentityManagement;
using Amazon.S3;
using Amazon.SecurityToken;
using Autofac;
using Calamari.Aws.Integration.S3;
using Calamari.Aws.Util;
using Calamari.CloudAccounts;

namespace Calamari.Aws
{
    public class Program : CalamariFlavourProgram
    {

        public Program(ILog log) : base(log)
        {
        }
        
        public static int Main(string[] args)
        {
            return new Program(ConsoleLog.Instance).Run(args);
        }

        protected override void ConfigureContainer(ContainerBuilder builder, CommonOptions options)
        {
            builder.RegisterType<AwsEnvironmentFactory>().As<IAwsEnvironmentFactory>().InstancePerLifetimeScope();
            builder.RegisterType<AmazonClientFactory>().As<IAmazonClientFactory>().InstancePerLifetimeScope();
            builder.RegisterType<VariableS3TargetOptionsProvider>().As<IProvideS3TargetOptions>().SingleInstance();

            base.ConfigureContainer(builder, options);
        }
    }
}
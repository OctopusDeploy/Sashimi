using Amazon.CloudFormation;
using Amazon.IdentityManagement;
using Amazon.S3;
using Amazon.SecurityToken;
using Autofac;
using Calamari.Aws.Deployment.CloudFormation;
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

        protected override void ConfigureContainer(ContainerBuilder builder, CommonOptions options)
        {
            builder.Register(
                    c => AwsEnvironmentGeneration.Create(log, c.Resolve<IVariables>()).GetAwaiter().GetResult())
                .AsSelf()
                .SingleInstance();

            builder.Register(c =>
            {
                var environment = c.Resolve<AwsEnvironmentGeneration>();

                return new AmazonS3Client(environment.AwsCredentials,
                    environment.AsClientConfig<AmazonS3Config>());
            }).As<IAmazonS3>();

            builder.Register(c =>
            {
                var environment = c.Resolve<AwsEnvironmentGeneration>();

                return new AmazonIdentityManagementServiceClient(environment.AwsCredentials,
                    environment.AsClientConfig<AmazonIdentityManagementServiceConfig>());
            }).As<IAmazonIdentityManagementService>();

            builder.Register(c =>
            {
                var environment = c.Resolve<AwsEnvironmentGeneration>();

                return new AmazonSecurityTokenServiceClient(environment.AwsCredentials,
                    environment.AsClientConfig<AmazonSecurityTokenServiceConfig>());
            }).As<IAmazonSecurityTokenService>();

            builder.Register(c =>
            {
                var environment = c.Resolve<AwsEnvironmentGeneration>();

                return new AmazonCloudFormationClient(environment.AwsCredentials,
                    environment.AsClientConfig<AmazonCloudFormationConfig>());
            }).As<IAmazonCloudFormation>();

            builder.RegisterType<CloudFormationService>()
                .As<ICloudFormationService>();

            builder.RegisterType<VariableS3TargetOptionsProvider>()
                .As<IProvideS3TargetOptions>()
                .SingleInstance();

            base.ConfigureContainer(builder, options);
        }

        public static int Main(string[] args)
        {
            return new Program(ConsoleLog.Instance).Run(args);
        }

        protected override void ConfigureContainer(ContainerBuilder builder, CommonOptions options)
        {
            builder.Register(
                    c => AwsEnvironmentGeneration.Create(log, c.Resolve<IVariables>()).GetAwaiter().GetResult())
                .AsSelf()
                .SingleInstance();

            builder.Register(c =>
            {
                var environment = c.Resolve<AwsEnvironmentGeneration>();

                return new AmazonS3Client(environment.AwsCredentials,
                    environment.AsClientConfig<AmazonS3Config>());
            }).As<IAmazonS3>();

            builder.Register(c =>
            {
                var environment = c.Resolve<AwsEnvironmentGeneration>();

                return new AmazonIdentityManagementServiceClient(environment.AwsCredentials,
                    environment.AsClientConfig<AmazonIdentityManagementServiceConfig>());
            }).As<IAmazonIdentityManagementService>();

            builder.Register(c =>
            {
                var environment = c.Resolve<AwsEnvironmentGeneration>();

                return new AmazonSecurityTokenServiceClient(environment.AwsCredentials,
                    environment.AsClientConfig<AmazonSecurityTokenServiceConfig>());
            }).As<IAmazonSecurityTokenService>();

            builder.RegisterType<VariableS3TargetOptionsProvider>()
                .As<IProvideS3TargetOptions>()
                .SingleInstance();

            base.ConfigureContainer(builder, options);
        }
    }
}
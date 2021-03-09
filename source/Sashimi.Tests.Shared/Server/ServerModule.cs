﻿using System;
using Autofac;
using Calamari.Tests.Shared;
using Octopus.Diagnostics;

namespace Sashimi.Tests.Shared.Server
{
    public class ServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<TestCloudTemplateHandlerFactory>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TestFormatIdentifier>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterInstance(new ServerInMemoryLog()).As<ITaskLog>().SingleInstance();
        }
    }
}

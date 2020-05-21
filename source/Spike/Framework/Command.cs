using System;
using System.Collections.Generic;

namespace Spike
{
    abstract class Command
    {
        public Queue<Type> Steps { get; } = new Queue<Type>();

        public abstract void Build(IVariables variable);

        protected void Add<TStep>() where TStep : IStep
        {
            Steps.Enqueue(typeof(TStep));
        }
    }
}
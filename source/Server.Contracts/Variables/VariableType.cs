﻿using System;
using Octopus.TinyTypes;

namespace Sashimi.Server.Contracts.Variables
{
    public class VariableType : CaseInsensitiveStringTinyType
    {
        public static readonly VariableType String = new VariableType("String");
        public static readonly VariableType Sensitive = new VariableType("Sensitive");
        public static readonly VariableType Certificate = new VariableType("Certificate");
        public static readonly VariableType WorkerPool = new VariableType("WorkerPool");

        public VariableType(string value) : base(value)
        {
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.IO;
using Calamari.Common.Plumbing.Extensions;
using Octostache;

namespace Calamari.Tests.Shared
{
    public class CommandTestBuilderContext
    {
        public List<(string? filename, Stream contents)> Files = new();
        internal bool withStagedPackageArgument;

        public VariableDictionary Variables { get; } = new();

        public CommandTestBuilderContext WithStagedPackageArgument()
        {
            withStagedPackageArgument = true;
            return this;
        }

        public CommandTestBuilderContext AddVariable(string key, string value)
        {
            Variables.Add(key, value);
            return this;
        }

        public CommandTestBuilderContext WithDataFile(string fileContents, string? fileName = null)
        {
            WithDataFile(fileContents.EncodeInUtf8Bom(), fileName);
            return this;
        }

        public CommandTestBuilderContext WithDataFileNoBom(string fileContents, string? fileName = null)
        {
            WithDataFile(fileContents.EncodeInUtf8NoBom(), fileName);
            return this;
        }

        public CommandTestBuilderContext WithDataFile(byte[] fileContents, string? fileName = null)
        {
            WithDataFile(new MemoryStream(fileContents), fileName);
            return this;
        }

        public CommandTestBuilderContext WithDataFile(Stream fileContents, string? fileName = null, Action<int>? progress = null)
        {
            Files.Add((fileName, fileContents));
            return this;
        }
    }
}
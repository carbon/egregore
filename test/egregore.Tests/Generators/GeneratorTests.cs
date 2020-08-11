﻿// Copyright (c) The Egregore Project & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.IO;
using egregore.Generators;
using egregore.Ontology;
using Xunit;
using Xunit.Abstractions;

namespace egregore.Tests.Generators
{
    public class GeneratorTests
    {
        [Fact]
        public void Can_generate_models_from_schema()
        {
            var schema = new Schema {Name = "Customer"};
            schema.Properties.Add(new SchemaProperty {Name = "Name", Type = "string"});

            var ns = new Namespace(Constants.DefaultNamespace);

            var workingDir = Directory.CreateDirectory("Generated");

            var sb = new IndentAwareStringBuilder();
            var program = new ProgramGenerator();
            program.Generate(sb, ns);
            sb.InsertAutoGeneratedHeader();
            File.WriteAllText("Generated\\Program.cs", sb.ToString());
            
            sb = new IndentAwareStringBuilder();
            var startup = new StartupGenerator();
            startup.Generate(sb, ns);
            sb.InsertAutoGeneratedHeader();
            File.WriteAllText("Generated\\Startup.cs", sb.ToString());

            sb = new IndentAwareStringBuilder();
            var models = new ModelGenerator();
            models.Generate(sb, ns, 1, schema);
            sb.InsertAutoGeneratedHeader();
            File.WriteAllText("Generated\\Models.cs", sb.ToString());

            sb = new IndentAwareStringBuilder();
            var dependencies = new DependenciesGenerator();
            dependencies.Generate(sb, ns);
            sb.InsertAutoGeneratedHeader();
            File.WriteAllText("Generated\\Dependencies.cs", sb.ToString());

            sb = new IndentAwareStringBuilder();
            var project = new ProjectGenerator();
            project.Generate(sb);
            File.WriteAllText("Generated\\Generated.csproj", sb.ToString());

            Environment.CurrentDirectory = workingDir.FullName;
            Process.Start(new ProcessStartInfo("dotnet", "build -c Release"));
        }
    }
}
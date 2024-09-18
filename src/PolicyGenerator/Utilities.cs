﻿using System.IO;
using System.Reflection;
using Scriban;

namespace PolicyGenerator;

internal static class Utility
{
    public static Template GetTemplate(string name)
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream(
                $"PolicyGenerator.Templates.{name}.sbntxt"
            )!;

        using var reader = new StreamReader(stream);
        return Template.Parse(reader.ReadToEnd());
    }
}
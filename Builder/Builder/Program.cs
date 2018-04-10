using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder
{
    class Program
    {
        static void Main(string[] args)
        {
            // Locates all of the instances of Visual Studio 2017 on the machine with MSBuild.
            var instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            if (!instances.Any())
            {
                Console.WriteLine("No Visual Studio instances found.");
            }

            Console.WriteLine("Visual Studio intances:");

            foreach (var instance in instances)
            {
                Console.WriteLine($"  - {instance.Name} - {instance.Version}");
                Console.WriteLine($"    {instance.MSBuildPath}");
                Console.WriteLine();
            }

            // We register the first instance that we found. This will cause MSBuildWorkspace to use the MSBuild installed in that instance.
            // Note: This has to be registered *before* creating MSBuildWorkspace. Otherwise, the MEF composition used by MSBuildWorkspace will fail to compose.
            var registeredInstance = instances.First();
            MSBuildLocator.RegisterInstance(registeredInstance);

            Console.WriteLine($"Registered: {registeredInstance.Name} - {registeredInstance.Version}");
            Console.WriteLine();

            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(@"..\..\..\..\ClassLibrary1\ClassLibrary1.sln").Result;

            foreach (var diag in workspace.Diagnostics)
                Console.WriteLine("Diagnostic: " + diag);

            foreach (var project in solution.Projects)
            {
                Console.WriteLine("Building project " + project.Name + "...");
                var comp = project.GetCompilationAsync().Result;
                var buildResult = comp.Emit(new MemoryStream());
                if (buildResult.Success == false)
                    throw new Exception("Build failed. " + buildResult.Diagnostics.Count() + " errors: " + string.Join("", buildResult.Diagnostics.Select(o => "\n  " + o.ToString())));
            }
        }
    }
}

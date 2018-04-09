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

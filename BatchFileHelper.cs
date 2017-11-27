using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WrightCover
{
    public class BatchFileHelper
    {
        public static void CreateRunAllBatchFile(IEnumerable<LoadedProject> projects, string filePath)
        {
            var filter = projects.Aggregate(string.Empty, (current, project) => current + $"+[{project.Name}]* ");

            var openCoverArguments = $@"%CD%\\lib\\opencover\\OpenCover.Console.exe -target:runtests.bat -register:user -excludebyattribute:*.ExcludeFromCodeCoverage* -filter:""{filter}""";
            var reportGeneratorArguments = "%CD%\\lib\\reportgenerator\\reportgenerator.exe -reports:results.xml -targetdir:coverage";
            var runChromeArguments = "start chrome %CD%\\coverage\\index.htm";

            using (var file = new StreamWriter(File.Create(filePath)))
            {
                file.WriteLine(openCoverArguments);
                file.WriteLine(reportGeneratorArguments);
                file.WriteLine(runChromeArguments);
            }
        }

        public static void CreateRunTestsBatchFile(IEnumerable<LoadedProject> projects, string packageDirectory)
        {
            using (var streamWriter = new StreamWriter(packageDirectory))
            {
                var counter = 1;
                var projectsPaths = string.Empty;
                var batchVariables = string.Empty;
                foreach (var project in projects)
                {
                    projectsPaths += $@"set ""filePath{counter}={project.AssemblyFilePath}"" " + Environment.NewLine;
                    batchVariables += $"%filePath{counter}% ";
                    counter++;
                }

                streamWriter.WriteLine(projectsPaths);
                streamWriter.Write($"%CD%\\lib\\nunit\\nunit3-console.exe {batchVariables}");
            }
        }

        public static void CreateDotNetCoreBatchFile(IEnumerable<LoadedProject> projects, string filePath)
        {
            using (var streamWriter = new StreamWriter(filePath))
            {
                var counter = 1;
                var projectsPaths = string.Empty;
                var runningCommands = string.Empty;

                foreach (var project in projects)
                {
                    var projectName = project.Name.Substring(0, project.Name.IndexOf(".Test."));
                    projectsPaths += $@"set ""filePath{counter}={project.AssemblyFilePath}"" " + Environment.NewLine;
                    counter++;

                    var path = project.AssemblyFilePath.Substring(0, project.AssemblyFilePath.LastIndexOf("\\"));
                    projectsPaths += $@"set ""filePath{counter}={path}\bin\Release\netcoreapp2.0"" " + Environment.NewLine;
                    
                    projectsPaths += Environment.NewLine;
                    runningCommands += @"%CD%\\lib\\opencover\\OpenCover.Console.exe -target:""c:\Program Files\dotnet\dotnet.exe"" ";
                    runningCommands += $@"-targetargs:""test -f netcoreapp2.0 -c Release %filePath{counter - 1}%"" -hideskipped:File -output:coverage.xml -searchdirs:%filePath{counter}% ";
                    runningCommands += $@"-mergeoutput -register:user -oldstyle -excludebyattribute:*.ExcludeFromCodeCoverage* -filter:""+[{projectName}]* "" " + Environment.NewLine;
                    counter++;
                }

                streamWriter.WriteLine(projectsPaths);
                streamWriter.WriteLine(runningCommands);
                streamWriter.WriteLine(@"%CD%\lib\reportgenerator\reportgenerator.exe -reports:coverage.xml -targetdir:coverage");
                streamWriter.WriteLine(@"start chrome %CD%\coverage\index.htm");
            }
        }

        public static void RunBatchFile(string runAllTestsBatchFilePath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(runAllTestsBatchFilePath)
                {
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = Path.GetDirectoryName(runAllTestsBatchFilePath)
                }
            };

            process.Start();
        }
    }
}

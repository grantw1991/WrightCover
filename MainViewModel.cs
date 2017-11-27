using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using EnvDTE;

namespace WrightCover
{
    public class MainViewModel
    {
        public enum ProjectType
        {
            DotNetCore,
            DotNetFramework
        }

        public ObservableCollection<LoadedProject> TestProjects { get; set; }
        public ObservableCollection<LoadedProject> AssemblyProjects { get; set; }

        public ObservableCollection<LogMessage> LogMessages { get; set; }
        public ICommand RunCommand => new RelayCommand(RunOpenCover, CanRunCodeCover);
        public ICommand LoadProjectsCommand => new RelayCommand(LoadProjects);
        public ICommand ClearLogCommand => new RelayCommand(() => LogMessages.Clear());
        public ICommand CopyLogCommand => new RelayCommand(CopyLog, () => LogMessages.Any());

        public MainViewModel()
        {
            TestProjects = new ObservableCollection<LoadedProject>();
            AssemblyProjects = new ObservableCollection<LoadedProject>();
            LogMessages = new ObservableCollection<LogMessage>();
            LoadProjects();
        }

        private ProjectType FindProjectType()
        {
            return Common.Instance.Dte2Object.Solution.Projects
                .Cast<Project>()
                .Any(project => project.Properties.Item("TargetFrameworkMoniker").Value.ToString().ToLower().Contains("core")) ?
                    ProjectType.DotNetCore :
                    ProjectType.DotNetFramework;
        }

        private void LoadProjects()
        {
            try
            {
                LogMessages.Add(LogMessage.Log("Loading projects", LogMessage.LogType.Info));
                TestProjects.Clear();
                AssemblyProjects.Clear();

                var projects = Common.Instance.Dte2Object.Solution.Projects;
                
                foreach (Project project in projects)
                {
                    //var projectFramework = project.Properties.Item("TargetFrameworkMoniker").Value;

                    if (!project.Name.Contains(".Test"))
                    {
                        if (project.Name.StartsWith("BeagleStreet."))
                        {
                            LogMessages.Add(LogMessage.Log($"Loaded test project {project.Name}", LogMessage.LogType.Success));
                            AssemblyProjects.Add(new LoadedProject { IsSelected = true, Name = project.Name });
                        }

                        continue;
                    }

                    var filePath = $"{Path.GetDirectoryName(project.FileName)}\\bin\\debug\\{project.Name}.dll";

                    if (File.Exists(filePath))
                    {
                        LogMessages.Add(LogMessage.Log($"Loaded test project {filePath}", LogMessage.LogType.Success));
                        TestProjects.Add(new LoadedProject { IsSelected = false, Name = project.Name, AssemblyFilePath = filePath });
                    }
                    else
                    {
                        LogMessages.Add(LogMessage.Log($"Could not find the file {filePath}", LogMessage.LogType.Error));
                    }
                }

                LogMessages.Add(TestProjects.Any()
                    ? LogMessage.Log($"Loaded {TestProjects.Count} test projects", LogMessage.LogType.Success)
                    : LogMessage.Log("Could not test load projects, click reload projects to try again", LogMessage.LogType.Error));
            }
            catch (Exception ex)
            {
                TestProjects.Clear();
                LogMessages.Add(LogMessage.Log($"Failed to load the projects, click reload to try again. ** {ex.Message} **", LogMessage.LogType.Error));
            }
        }

        private bool CanRunCodeCover()
        {
            return TestProjects.Any(t => t.IsSelected) && AssemblyProjects.Any(a => a.IsSelected);
        }

        private void RunOpenCover()
        {
            try
            {
                if (!TestProjects.Any(t => t.IsSelected))
                {
                    LogMessages.Add(LogMessage.Log("No selected projects found", LogMessage.LogType.Warning));
                    MessageBox.Show("Select the projects you want to run", "No projects selected", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                var runAllBatchFileLocation = Utilities.GetAssemblyDirectory + "\\runall.bat";
                var runTestsBatchFileLocation = Utilities.GetAssemblyDirectory + "\\runtests.bat";

                LogMessages.Add(LogMessage.Log($"Generating runtests.bat file to: {runTestsBatchFileLocation}", LogMessage.LogType.Info));
                BatchFileHelper.CreateRunTestsBatchFile(TestProjects.Where(a => a.IsSelected), runTestsBatchFileLocation);
                LogMessages.Add(LogMessage.Log("Generated runtests.bat", LogMessage.LogType.Success));

                LogMessages.Add(LogMessage.Log($"Generating runall.bat to: {runAllBatchFileLocation}", LogMessage.LogType.Info));
                BatchFileHelper.CreateRunAllBatchFile(AssemblyProjects.Where(a => a.IsSelected), runAllBatchFileLocation);
                LogMessages.Add(LogMessage.Log("Generated runall.bat", LogMessage.LogType.Success));

                BatchFileHelper.RunBatchFile(runAllBatchFileLocation);
                LogMessages.Add(LogMessage.Log("Successfully ran main batch file", LogMessage.LogType.Success));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogMessages.Add(LogMessage.Log(e.Message, LogMessage.LogType.Error));
            }
        }

        private void CopyLog()
        {
            var messageContent = string.Empty;
            foreach (var message in LogMessages)
            {
                messageContent += message.Message + Environment.NewLine;
            }

            Clipboard.SetText(messageContent);
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Web.Compilation;
using System.Web.Hosting;
using Orchard.Management.PsProvider.Agents;

namespace Orchard.Management.PsProvider.Host {
    public class OrchardHostContextProvider {
        private readonly string _orchardPath;
        private static ClientBuildManager _clientBuildManager;

        public OrchardHostContextProvider(string orchardPath) {
            _orchardPath = orchardPath;
        }

        [SecurityCritical]
        public OrchardHostContext CreateContext() {
            var context = new OrchardHostContext { RetryResult = ReturnCodes.Retry };
            Initialize(context);
            return context;
        }

        [SecurityCritical]
        public void Shutdown(OrchardHostContext context) {
            try {
                if (context.OrchardHost != null) {
                    LogInfo("Shutting down Orchard session...");
                    context.OrchardHost.StopSession();
                }
            }
            catch (AppDomainUnloadedException) {
                LogInfo("AppDomain already unloaded.");
            }

            if (context.OrchardHost != null) {
                LogInfo("Shutting down ASP.NET AppDomain...");
                ApplicationManager.GetApplicationManager().ShutdownAll();
            }
        }

        public T CreateAgent<T>() where T : AgentProxy {
            return (T)_clientBuildManager.CreateObject(typeof(T), false);
        }

        private static void LogInfo(string format, params object[] args) {
            Trace.WriteLine(string.Format(format, args));
        }

        private void Initialize(OrchardHostContext context) {
            context.WorkingDirectory = string.IsNullOrEmpty(_orchardPath) 
                ? System.Environment.CurrentDirectory 
                : _orchardPath;
            LogInfo("Working directory: \"{0}\"", context.WorkingDirectory);

            context.VirtualPath = "/";
            LogInfo("Virtual path: \"{0}\"", context.VirtualPath);

            LogInfo("Detecting orchard installation root directory...");
            context.OrchardDirectory = GetOrchardDirectory(context.WorkingDirectory).FullName;
            LogInfo("Orchard root directory: \"{0}\"", context.OrchardDirectory);

            LogInfo("Creating ASP.NET AppDomain for command agent...");
            context.OrchardHost = CreateOrchardHost(context.VirtualPath, context.OrchardDirectory);

            LogInfo("Starting Orchard session");
            context.StartSessionResult = context.OrchardHost.StartSession();
        }

        private DirectoryInfo GetOrchardDirectory(string directory) {
            var directoryInfo = new DirectoryInfo(directory);

            while (directoryInfo != null) {
                if (!directoryInfo.Exists) {
                    throw new ApplicationException(string.Format("Directory \"{0}\" does not exist", directoryInfo.FullName));
                }

                bool webConfigExists = File.Exists(Path.Combine(directoryInfo.FullName, "web.config"));
                bool binDirectoryExists = Directory.Exists(Path.Combine(directoryInfo.FullName, "bin"));
                bool orchardFrameworkExists = File.Exists(Path.Combine(directoryInfo.FullName, "bin\\Orchard.Framework.dll"));

                if (webConfigExists && binDirectoryExists && orchardFrameworkExists) {
                    return directoryInfo;
                }
             
                directoryInfo = directoryInfo.Parent;   
            }

            throw new ApplicationException(
                string.Format("Directory \"{0}\" doesn't seem to contain an Orchard installation", new DirectoryInfo(directory).FullName));
        }

        private static OrchardHost CreateOrchardHost(string virtualPath, string physicalPath) {
            _clientBuildManager = new ClientBuildManager(virtualPath, physicalPath);
            _clientBuildManager.CompileApplicationDependencies();
            return (OrchardHost)_clientBuildManager.CreateObject(typeof(OrchardHost), false);
        }
    }
}
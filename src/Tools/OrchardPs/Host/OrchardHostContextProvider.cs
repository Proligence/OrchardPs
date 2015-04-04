namespace OrchardPs.Host
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Web.Compilation;
    using System.Web.Hosting;
    using Proligence.PowerShell.Provider;

    /// <summary>
    /// Implements starting and shutting down the Orchard host object which runs inside Orchard web application's
    /// AppDomain.
    /// </summary>
    public class OrchardHostContextProvider 
    {
        private static ClientBuildManager clientBuildManager;
        private readonly string orchardDir;

        public OrchardHostContextProvider()
        {
        }

        public OrchardHostContextProvider(string orchardDir)
        {
            this.orchardDir = orchardDir;
        }

        [SecurityCritical]
        public OrchardHostContext CreateContext(IConsoleConnection connection)
        {
            var context = new OrchardHostContext();
            this.Initialize(context, connection);
            
            return context;
        }

        [SecurityCritical]
        public void Shutdown(OrchardHostContext context) 
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            try 
            {
                if (context.OrchardHost != null) 
                {
                    LogInfo("Shutting down Orchard session...");
                    context.OrchardHost.StopSession();
                }
            }
            catch (AppDomainUnloadedException) 
            {
                LogInfo("AppDomain already unloaded.");
            }

            if (context.OrchardHost != null) 
            {
                LogInfo("Shutting down ASP.NET AppDomain...");
                ApplicationManager.GetApplicationManager().ShutdownAll();
            }
        }

        private static void LogInfo(string format, params object[] args) 
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        [SecurityCritical]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private static OrchardHost CreateOrchardHost(string virtualPath, string physicalPath) 
        {
            clientBuildManager = new ClientBuildManager(virtualPath, physicalPath);
            clientBuildManager.CompileApplicationDependencies();
            return (OrchardHost)clientBuildManager.CreateObject(typeof(OrchardHost), false);
        }

        private void Initialize(OrchardHostContext context, IConsoleConnection connection)
        {
            context.WorkingDirectory = Environment.CurrentDirectory;
            LogInfo("Working directory: \"{0}\"", context.WorkingDirectory);

            context.VirtualPath = "/";
            LogInfo("Virtual path: \"{0}\"", context.VirtualPath);

            LogInfo("Detecting orchard installation root directory...");
            context.OrchardDirectory = this.orchardDir ?? GetOrchardDirectory(context.WorkingDirectory).FullName;
            LogInfo("Orchard root directory: \"{0}\"", context.OrchardDirectory);

            LogInfo("Creating ASP.NET AppDomain for command agent...");
            context.OrchardHost = CreateOrchardHost(context.VirtualPath, context.OrchardDirectory);

            LogInfo("Starting Orchard session");
            context.Session = context.OrchardHost.StartSession(connection);
        }
        
        /// <summary>
        /// Gets the <see cref="DirectoryInfo"/> object of Orchard's root directory.
        /// </summary>
        private static DirectoryInfo GetOrchardDirectory(string directory) 
        {
            var directoryInfo = new DirectoryInfo(directory);

            while (directoryInfo != null) 
            {
                if (!directoryInfo.Exists)
                {
                    string message = string.Format(
                        CultureInfo.CurrentCulture,
                        "Directory \"{0}\" does not exist.",
                        directoryInfo.FullName);
                    
                    throw new InvalidOperationException(message);
                }

                bool webConfigExists = 
                    File.Exists(Path.Combine(directoryInfo.FullName, "web.config"));
                bool binDirectoryExists = 
                    Directory.Exists(Path.Combine(directoryInfo.FullName, "bin"));
                bool orchardFrameworkExists = 
                    File.Exists(Path.Combine(directoryInfo.FullName, "bin\\Orchard.Framework.dll"));

                if (webConfigExists && binDirectoryExists && orchardFrameworkExists) 
                {
                    return directoryInfo;
                }
             
                directoryInfo = directoryInfo.Parent;   
            }

            string msg = string.Format(
                CultureInfo.CurrentCulture,
                "Directory \"{0}\" doesn't seem to contain an Orchard installation",
                new DirectoryInfo(directory).FullName);
            
            throw new InvalidOperationException(msg);
        }
    }
}
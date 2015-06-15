namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;

    public static class PsSessionManagerExtensions
    {
        public static IPsSession CreateSession(this IPsSessionManager sessionManager)
        {
            return sessionManager.NewSession(Guid.NewGuid().ToString(), new BufferConsoleConnection());
        }

        public static IPsSession CreateSession(this IPsSessionManager sessionManager, out BufferConsoleConnection connection)
        {
            connection = new BufferConsoleConnection();
            return sessionManager.NewSession(Guid.NewGuid().ToString(), connection);
        }

        public static string Execute(this IPsSessionManager sessionManager, string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentNullException("command");
            }

            BufferConsoleConnection connection;
            using (var session = CreateSession(sessionManager, out connection))
            {
                session.ProcessInput(command);
            }

            if (connection.ErrorOutput.Length > 0)
            {
                throw new PowerShellException(connection.ErrorOutput.ToString(), command);
            }

            return connection.Output.ToString();
        }

        public static string Execute(this IPsSessionManager sessionManager, IEnumerable<string> commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }

            BufferConsoleConnection connection;
            using (var session = CreateSession(sessionManager, out connection))
            {
                foreach (string command in commands)
                {
                    if (!string.IsNullOrWhiteSpace(command))
                    {
                        session.ProcessInput(command);

                        if (connection.ErrorOutput.Length > 0)
                        {
                            throw new PowerShellException(connection.ErrorOutput.ToString(), command);
                        }
                    }
                }
            }

            return connection.Output.ToString();
        }

        public static ICollection<PSObject> ExecutePipeline(
            this IPsSessionManager sessionManager,
            Action<Pipeline> action,
            params object[] input)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            BufferConsoleConnection connection;
            using (var session = CreateSession(sessionManager, out connection))
            {
                lock (session.RunspaceLock)
                {
                    using (var pipeline = session.Runspace.CreatePipeline())
                    {
                        action(pipeline);

                        ICollection<PSObject> result;
                        if ((input != null) && (input.Length > 0))
                        {
                            result = pipeline.Invoke(input);
                        }
                        else
                        {
                            result = pipeline.Invoke();
                        }

                        if (pipeline.HadErrors)
                        {
                            var error = (ErrorRecord)pipeline.Error.Read();
                            throw new PowerShellException(error);
                        }

                        return result;
                    }
                }
            }
        }
    }
}
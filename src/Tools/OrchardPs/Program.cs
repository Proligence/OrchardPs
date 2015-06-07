namespace OrchardPs 
{
    using System;
    using Autofac.Core.Registration;
    using OrchardPs.Console;
    using OrchardPs.Host;

    public static class Program
    {
        private static bool running = true;
        private static OrchardHostContextProvider hostContextProvider;
        private static OrchardHostContext hostContext;

        public static int Main(string[] args)
        {
            System.Console.WriteLine(Banner.GetBanner());

            hostContextProvider = new OrchardHostContextProvider();

            try
            {
                hostContext = InitializeOrchardHost();
            }
            catch (Exception ex)
            {
                // NOTE: Currently Autofac.Core.Registration.ComponentNotRegisteredException is not serializable,
                // so we need to workaround this issue by reading the exception's message.
                if ((ex is ComponentNotRegisteredException) || ex.Message.Contains("ComponentNotRegisteredException"))
                {
                    ConsoleHelper.WriteToConsole(
                       "Failed to initialize PowerShell engine host. " +
                       "Please make sure that the Proligence.PowerShell.Provider Orchard module is enabled." +
                       Environment.NewLine,
                       ConsoleColor.Red);

                    return -1;   
                }

                throw;
            }

            System.Console.CancelKeyPress += OnCancelKeyPress;

            try
            {
                var session = hostContext.Session;
                var connection = (DirectConsoleConnection)hostContext.OrchardHost.Connection;
                connection.CommandCompletionProvider = new CommandCompletionProvider(session);
                
                while (running)
                {
                    string input = connection.GetInput();

                    if ((input == "clear") || (input == "cls"))
                    {
                        System.Console.Clear();
                    }
                    else if (input == "exit")
                    {
                        running = false;
                        break;
                    }
                    else
                    {
                        session.ProcessInput(input);
                        session.RunspaceLock.WaitOne();
                        session.RunspaceLock.Set();
                    }
                }

                hostContextProvider.Shutdown(hostContext);
            }
            catch (AppDomainUnloadedException ex)
            {
                ConsoleHelper.WriteToConsole(ex.Message + Environment.NewLine, ConsoleColor.Red);
            }
            
            return 0;
        }

        private static OrchardHostContext InitializeOrchardHost()
        {
            var connection = new DirectConsoleConnection();

            OrchardHostContext context = hostContextProvider.CreateContext(connection);
            if (context.Session == null)
            {
                context = hostContextProvider.CreateContext(connection);
            }
            else if (context.Session == null)
            {
                hostContextProvider.Shutdown(context);
                throw new ApplicationException("Failed to initialize Orchard session.");
            }
            
            return context;
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            running = false;
            hostContextProvider.Shutdown(hostContext);
        }
    }
}
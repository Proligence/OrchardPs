namespace OrchardPs 
{
    using System;
    using OrchardPs.Console;
    using OrchardPs.Host;

    public static class Program
    {
        private static bool running = true;

        public static int Main(string[] args)
        {
            System.Console.WriteLine(Banner.GetBanner());

            var hostContextProvider = new OrchardHostContextProvider();
            OrchardHostContext context = InitializeOrchardHost(hostContextProvider);

            System.Console.CancelKeyPress += OnCancelKeyPress;

            var session = context.Session;
            var connection = context.OrchardHost.Connection;
            while (running)
            {
                string input = connection.GetInput();

                if ((input == "clear") || (input == "cls"))
                {
                    System.Console.Clear();
                }
                else
                {
                    session.ProcessInput(input);                    
                }
            }

            hostContextProvider.Shutdown(context);
            return 0;
        }

        private static OrchardHostContext InitializeOrchardHost(OrchardHostContextProvider hostContextProvider)
        {
            OrchardHostContext context = hostContextProvider.CreateContext();
            if (context.Session == null)
            {
                context = hostContextProvider.CreateContext();
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
        }
    }
}
namespace Proligence.PowerShell.Recipes.RecipeHandlers
{
    using System;
    using System.Linq;
    using Orchard.Logging;
    using Orchard.Recipes.Models;
    using Orchard.Recipes.Services;
    using Proligence.PowerShell.Provider;

    public class PowerShellRecipeHandler : IRecipeHandler 
    {
        private readonly IPsSessionManager sessionManager;

        public PowerShellRecipeHandler(IPsSessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
            this.Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void ExecuteRecipeStep(RecipeContext recipeContext)
        {
            if (string.Equals(recipeContext.RecipeStep.Name, "PowerShellCommand", StringComparison.OrdinalIgnoreCase))
            {
                var commands = recipeContext.RecipeStep.Step.Value
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(cmd => cmd.Trim())
                    .Where(cmd => !string.IsNullOrWhiteSpace(cmd));

                try
                {
                    this.sessionManager.Execute(commands);
                }
                catch (PowerShellException ex)
                {
                    this.Logger.Error(ex.Message);
                    throw;
                }
                
                recipeContext.Executed = true;
            }
        }
    }
}
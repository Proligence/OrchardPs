using System;
using System.Linq;
using Orchard.Environment.Extensions;
using Orchard.Logging;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;
using Proligence.PowerShell.Provider;

namespace Proligence.PowerShell.Core.Recipes.RecipeHandlers {
    [OrchardFeature("Proligence.PowerShell.Recipes")]
    public class PowerShellRecipeHandler : IRecipeHandler {
        private readonly IPsSessionManager _sessionManager;

        public PowerShellRecipeHandler(IPsSessionManager sessionManager) {
            _sessionManager = sessionManager;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void ExecuteRecipeStep(RecipeContext recipeContext) {
            if (string.Equals(recipeContext.RecipeStep.Name, "PowerShellCommand", StringComparison.OrdinalIgnoreCase)) {
                var commands = recipeContext.RecipeStep.Step.Value
                    .Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(cmd => cmd.Trim())
                    .Where(cmd => !string.IsNullOrWhiteSpace(cmd));

                try {
                    _sessionManager.Execute(commands);
                }
                catch (PowerShellException ex) {
                    Logger.Error(ex.Message);
                    throw;
                }

                recipeContext.Executed = true;
            }
        }
    }
}
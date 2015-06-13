namespace Proligence.PowerShell.Provider.Models
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;
    using Orchard.ContentManagement;

    public class PowerShellSettingsPart : ContentPart, IPowerShellSettings
    {
        [Range(PowerShellSettings.MinimumConsoleWidth, PowerShellSettings.MaximumConsoleWidth)]
        public int ConsoleWidth
        {
            get
            {
                return this.PowerShellSettings.ConsoleWidth;
            }

            set
            {
                var settings = this.PowerShellSettings;
                settings.ConsoleWidth = value;
                this.PowerShellSettings = settings;
            }
        }

        [Range(PowerShellSettings.MinimumConsoleHeight, PowerShellSettings.MaximumConsoleHeight)]
        public int ConsoleHeight
        {
            get
            {
                return this.PowerShellSettings.ConsoleHeight;
            }

            set
            {
                var settings = this.PowerShellSettings;
                settings.ConsoleHeight = value;
                this.PowerShellSettings = settings;
            }
        }

        public PowerShellSettings PowerShellSettings
        {
            get
            {
                var json = this.Retrieve<string>("PowerShellSettings");
                if (json != null)
                {
                    return JsonConvert.DeserializeObject<PowerShellSettings>(json);
                }

                return new PowerShellSettings();
            }

            set
            {
                this.Store("PowerShellSettings", value ?? new PowerShellSettings());
            }
        }
    }
}
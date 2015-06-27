namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System;
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class EditContentPartDefinitionTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public EditContentPartDefinitionTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldUpdateDescription()
        {
            string newDescription = Guid.NewGuid().ToString("N");
            this.powerShell.Session.ProcessInput("Edit-ContentPartDefinition CommonPart -Description '" + newDescription + "'");

            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Equal(newDescription, this.GetDescription("CommonPart"));
        }

        [Fact, Integration]
        public void ShouldUpdateAttachableFlag()
        {
            // Repeat test two times - first set/unset flag, then unset/se flag
            for (int i = 0; i < 2; i++)
            {
                if (this.IsAttachable("CommentPart"))
                {
                    this.powerShell.Session.ProcessInput("Edit-ContentPartDefinition CommentPart -Attachable $false");
                    this.powerShell.ConsoleConnection.AssertNoErrors();
                    Assert.False(this.IsAttachable("CommentPart"));
                }
                else
                {
                    this.powerShell.Session.ProcessInput("Edit-ContentPartDefinition CommentPart -Attachable $true");
                    this.powerShell.ConsoleConnection.AssertNoErrors();
                    Assert.True(this.IsAttachable("CommentPart"));
                }
            }
        }

        [Fact, Integration]
        public void ShouldUpdateCustomSetting()
        {
            var settingName = Guid.NewGuid().ToString("N");
            this.powerShell.Session.ProcessInput("Edit-ContentPartDefinition CommonPart -" + settingName + " value");
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Equal("value", this.GetSettingValue("CommonPart", settingName));

            this.powerShell.Session.ProcessInput("Edit-ContentPartDefinition CommonPart -" + settingName + " 'new value'");
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Equal("new value", this.GetSettingValue("CommonPart", settingName));
        }

        [Fact, Integration]
        public void ShouldDeleteCustomSetting()
        {
            var settingName = Guid.NewGuid().ToString("N");
            this.powerShell.Session.ProcessInput("Edit-ContentPartDefinition CommonPart -" + settingName + " value");
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Equal("value", this.GetSettingValue("CommonPart", settingName));

            this.powerShell.Session.ProcessInput("Edit-ContentPartDefinition CommonPart -" + settingName + " $null");
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Null(this.GetSettingValue("CommonPart", settingName));
        }

        [Fact, Integration]
        public void ShouldUpdateContentPartDefinitionObject()
        {
            string newDescription = Guid.NewGuid().ToString("N");
            this.powerShell.Session.ProcessInput(
                "Get-ContentPartDefinition CommonPart | Edit-ContentPartDefinition -Description '" + newDescription + "'");
            
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Equal(newDescription, this.GetDescription("CommonPart"));
        }

        private string GetDescription(string contentPart)
        {
            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("Get-ContentPartDefinition " + contentPart);
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            this.powerShell.ConsoleConnection.Reset();

            return table[0, "Description"];
        }

        private bool IsAttachable(string contentPart)
        {
            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("Get-ContentPartDefinition " + contentPart);
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            this.powerShell.ConsoleConnection.Reset();

            return table[0, "Attachable"].Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        private string GetSettingValue(string contentType, string settingName)
        {
            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("(Get-ContentPartDefinition " + contentType + ").Settings");
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            this.powerShell.ConsoleConnection.Reset();

            var row = table.Rows.FirstOrDefault(r => r[0] == "ContentPartSettings." + settingName);
            if (row != null)
            {
                return row[1];
            }

            return null;
        }
    }
}
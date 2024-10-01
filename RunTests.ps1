param ($Configuration = 'Debug', [switch]$Integration)

function RunXUnit($AssemblyPath) {
    $XUnit = Join-Path $PSScriptRoot 'lib\xunit\xunit.console.exe'
    $XUnitParams = ''
    
    if (-not $Integration) {
        $XUnitParams += "-notrait 'category=integration'"
    }
    
    $AssemblyFullPath = Join-Path $PSScriptRoot $AssemblyPath
    $CommandText = $XUnit + " " + $AssemblyFullPath + " " + $XUnitParams
    Write-Host $CommandText
    
    Invoke-Expression $CommandText
    if (-not $?) {
        Write-Error "Unit tests failed in '$AssemblyPath'!"
    }

    Write-Host ''
    Write-Host ''
}

RunXUnit "src\Orchard.Tests.Modules.PowerShell.Core\bin\$Configuration\Orchard.Tests.Modules.PowerShell.Core.dll"
RunXUnit "src\Orchard.Tests.Modules.PowerShell.Provider\bin\$Configuration\Orchard.Tests.Modules.PowerShell.Provider.dll"
RunXUnit "src\Orchard.Tests.Modules.PowerShell.Web\bin\$Configuration\Orchard.Tests.Modules.PowerShell.Web.dll"
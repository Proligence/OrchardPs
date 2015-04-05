@echo off

if "%1" == "" goto usage

mklink /J %1\lib\powershell %~dp0lib\powershell
mklink /J %1\lib\xunit %~dp0lib\xunit
mklink /J %1\src\Orchard.Tests.Modules.PowerShell.Core %~dp0src\Orchard.Tests.Modules.PowerShell.Core
mklink /J %1\src\Orchard.Tests.Modules.PowerShell.Provider %~dp0src\Orchard.Tests.Modules.PowerShell.Provider
mklink /J %1\src\Orchard.Tests.Modules.PowerShell.Web %~dp0src\Orchard.Tests.Modules.PowerShell.Web
mklink /J %1\src\Orchard.Tests.PowerShell.Infrastructure %~dp0src\Orchard.Tests.PowerShell.Infrastructure
mklink /J %1\src\Orchard.Web\Modules\Proligence.PowerShell.Core %~dp0src\Orchard.Web\Modules\Proligence.PowerShell.Core
mklink /J %1\src\Orchard.Web\Modules\Proligence.PowerShell.Provider %~dp0src\Orchard.Web\Modules\Proligence.PowerShell.Provider
mklink /J %1\src\Orchard.Web\Modules\Proligence.PowerShell.Web %~dp0src\Orchard.Web\Modules\Proligence.PowerShell.Web
mklink /J %1\src\Tools\OrchardPs %~dp0src\Tools\OrchardPs 
goto done

:usage
echo Usage: MapToOrchard.cmd OrchardRootDir
goto exit

:done
echo Done.

:exit
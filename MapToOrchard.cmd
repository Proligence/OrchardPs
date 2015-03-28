@echo off

if "%1" == "" goto usage

mklink /J %1\lib\powershell %~dp0lib\powershell
mklink /J %1\src\Orchard.Tests.Modules.PowerShell %~dp0src\Orchard.Tests.Modules.PowerShell
mklink /J %1\src\Orchard.Tests.Modules.PowerShell.Provider %~dp0src\Orchard.Tests.Modules.PowerShell.Provider
mklink /J %1\src\Orchard.Web\Modules\Proligence.PowerShell %~dp0src\Orchard.Web\Modules\Proligence.PowerShell
mklink /J %1\src\Orchard.Web\Modules\Proligence.PowerShell.Provider %~dp0src\Orchard.Web\Modules\Proligence.PowerShell.Provider
mklink /J %1\src\Orchard.Web\Modules\Proligence.PowerShell.WebConsole %~dp0src\Orchard.Web\Modules\Proligence.PowerShell.WebConsole
goto done

:usage
echo Usage: MapToOrchard.cmd OrchardRootDir
goto exit

:done
echo Done.

:exit
@echo off

if "%~1" == "" goto usage

REM Strip quotes from input path
set "orchardpath=%~1"

mklink /J "%orchardpath%\lib\powershell" "%~dp0lib\powershell"
mklink /J "%orchardpath%\lib\xunit" "%~dp0lib\xunit"
mklink /J "%orchardpath%\src\Orchard.Tests.Modules.PowerShell.Core" "%~dp0src\Orchard.Tests.Modules.PowerShell.Core"
mklink /J "%orchardpath%\src\Orchard.Tests.Modules.PowerShell.Provider" "%~dp0src\Orchard.Tests.Modules.PowerShell.Provider"
mklink /J "%orchardpath%\src\Orchard.Tests.Modules.PowerShell.Web" "%~dp0src\Orchard.Tests.Modules.PowerShell.Web"
mklink /J "%orchardpath%\src\Orchard.Tests.PowerShell.Infrastructure" "%~dp0src\Orchard.Tests.PowerShell.Infrastructure"
mklink /J "%orchardpath%\src\Orchard.Web\Modules\Proligence.PowerShell.Core" "%~dp0src\Orchard.Web\Modules\Proligence.PowerShell.Core"
mklink /J "%orchardpath%\src\Orchard.Web\Modules\Proligence.PowerShell.Provider" "%~dp0src\Orchard.Web\Modules\Proligence.PowerShell.Provider"
mklink /J "%orchardpath%\src\Orchard.Web\Modules\Proligence.PowerShell.Web" "%~dp0src\Orchard.Web\Modules\Proligence.PowerShell.Web"
mklink /J "%orchardpath%\src\Tools\OrchardPs" "%~dp0src\Tools\OrchardPs"
goto done

:usage
echo Usage: MapToOrchard.cmd OrchardRootDir
goto exit

:done
echo Done.

:exit
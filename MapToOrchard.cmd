@echo off

if "%1" == "" goto usage

mklink /J %1\lib\powershell %~dp0lib\powershell
mklink /J %1\lib\powershell-vfs %~dp0lib\powershell-vfs
mklink /J %1\src\Orchard.Tests.Modules.PowerShell %~dp0src\Orchard.Tests.Modules.PowerShell
mklink /J %1\src\Orchard.Web\Modules\Proligence.PowerShell %~dp0src\Orchard.Web\Modules\Proligence.PowerShell
mklink /J %1\src\Tools\Orchard.Management.PsProvider %~dp0src\Tools\Orchard.Management.PsProvider
mklink /J %1\src\Tools\Orchard.Management.PsProvider.Tests %~dp0src\Tools\Orchard.Management.PsProvider.Tests
mklink /J %1\src\Tools\OrchardPs %~dp0src\Tools\OrchardPs 
mklink /J %1\src\Tools\Proligence.PowerShell.Vfs %~dp0src\PowerShellVfs\src\Proligence.PowerShell.Vfs
mklink /J %1\src\Tools\Proligence.PowerShell.Vfs.Tests %~dp0src\PowerShellVfs\src\Proligence.PowerShell.Vfs.Tests
goto done

:usage
echo Usage: MapToOrchard.cmd OrchardRootDir
goto exit

:done
echo Done.

:exit
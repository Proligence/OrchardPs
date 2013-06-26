$cert = Get-ChildItem Cert:\CurrentUser\My\95EF662130DEA9E1A3F07BE618659C8255920891
Set-AuthenticodeSignature .\Orchard.Web\Modules\Proligence.PowerShell\Commands\Commands.format.ps1xml $cert
Set-AuthenticodeSignature .\Orchard.Web\Modules\Proligence.PowerShell\Common\Common.format.ps1xml $cert
Set-AuthenticodeSignature .\Orchard.Web\Modules\Proligence.PowerShell\Modules\Modules.format.ps1xml $cert
Set-AuthenticodeSignature .\Orchard.Web\Modules\Proligence.PowerShell\Sites\Sites.format.ps1xml $cert
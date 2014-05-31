# Get certificate from store
$cert = Get-ChildItem Cert:\CurrentUser\My\95EF662130DEA9E1A3F07BE618659C8255920891

# Sign files
Get-ChildItem -Recurse -Include *.ps1xml .\Orchard.Web\Modules\Proligence.PowerShell | `
ForEach-Object { Set-AuthenticodeSignature $_.FullName $cert }
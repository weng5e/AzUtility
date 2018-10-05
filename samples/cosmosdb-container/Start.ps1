#----------------------------------------------------------
# Copyright (C) Microsoft Corporation. All rights reserved.
#----------------------------------------------------------

using namespace System.IO

[CmdletBinding(PositionalBinding = $false)]
param(
    [ValidateSet('BoundedStaleness', 'Eventual', 'Session', 'Strong')]
    [Parameter()]
    [string]
    $Consistency = 'Session',

    [ValidateRange(1, 250)]
    [Parameter()]
    [UInt32]
    $DefaultPartitionCount = 25,

    [ValidateRange(1, 250)]
    [Parameter()]
    [UInt32]
    $PartitionCount = 25,

    [Parameter()]
    [switch]
    $SimulateRateLimiting,

    [Parameter()]
    [UInt32]
    $Timeout = 120,

    [Parameter()]
    [switch]
    $Trace
)

[Console]::BufferWidth = 32766

New-Variable Key -Scope Global -Option Constant -Value 'C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=='
New-Variable HostDirectory -Scope Global -Option Constant -Value ((Get-Item $PSScriptRoot).CreateSubdirectory('bind-mount'))
New-Variable DiagnosticsDirectory -Scope Global -Option Constant -Value ($HostDirectory.CreateSubdirectory('Diagnostics'))

Start-Transcript -IncludeInvocationHeader "$DiagnosticsDirectory\Transcript.log"
$ErrorActionPreference = "Stop"; $DebugPreference = "Continue"

function Copy-Diagnostics {
    [CmdletBinding()]
    param(
        [Parameter()]
        [DirectoryInfo]
        $DiagnosticsDirectory = $DiagnosticsDirectory
    )

    Stop-CosmosDbEmulator; Get-Process CosmosDB.Emulator, DocumentDB.* | Stop-Process

    Get-Item -ErrorAction SilentlyContinue "$env:ProgramFiles\Azure Cosmos DB Emulator\*.etl" | ForEach-Object {
        Copy-File $_ $DiagnosticsDirectory
    }

    Get-Item -ErrorAction SilentlyContinue "$env:LOCALAPPDATA\CrashDumps\CosmosDB.*.dmp", "$env:LOCALAPPDATA\CrashDumps\DocumentDB.*.dmp" | ForEach-Object {
        Copy-File $_ $DiagnosticsDirectory
    }
}

function DoStart {
    [CmdletBinding()]
    param()

    Stop-CosmosDbEmulator; Start-CosmosDbEmulator -AllowNetworkAccess -NoFirewall -NoUI -Key $Key `
        -DefaultPartitionCount $DefaultPartitionCount -PartitionCount $PartitionCount `
        -Consistency $Consistency -SimulateRateLimiting:($SimulateRateLimiting) `
        -Timeout $Timeout -Trace:($Trace)

    # Export two forms of the Emulator's certificate (CERT and PFX) and provide a script for importing the PFX form to the
    # user's trusted certificate store

    $password = ConvertTo-SecureString -String $Key -Force -AsPlainText
    $cert = Get-ChildItem cert:\LocalMachine\My | Where-Object { $_.FriendlyName -eq "DocumentDbEmulatorCertificate" }
    Export-PfxCertificate -Cert $cert -FilePath "$HostDirectory\CosmosDbEmulatorCert.pfx" -Password $password | Out-Null

    $cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
    $cert.Import("$HostDirectory\CosmosDbEmulatorCert.pfx", $Key, "DefaultKeySet")
    $cert | Export-Certificate -FilePath "$HostDirectory\CosmosDbEmulatorCert.cer" -Type CERT

    if (-not (Test-Path "$HostDirectory\importcert.ps1")) {
        [void](New-Item -ItemType File "$HostDirectory\importcert.ps1")
    }

    Set-Content "$HostDirectory\importcert.ps1" -Value @"
# Generated by: $($MyInvocation.PSCommandPath)
# Date: $([DateTimeOffset]::Now.ToString('s'))
[CmdletBinding()]
param()
Write-Information -InformationAction Continue ""
Write-Information -InformationAction Continue "Importing self-signed certificate generated by the Azure Cosmos DB Emulator runnning in container $(hostname) to the certificate store on `$(hostname)"
"My", "Root" | ForEach-Object { Import-Certificate -FilePath "`$PSScriptRoot\CosmosDBEmulatorCert.cer" -CertStoreLocation Cert:\localMachine\`$_ }
"@

    # Pipe an emulator info object to the output stream

    $Emulator = Get-Item "$env:ProgramFiles\Azure Cosmos DB Emulator\CosmosDB.Emulator.exe"
    $IPAddress = Get-NetIPAddress -AddressFamily IPV4 -AddressState Preferred -PrefixOrigin Manual | Select-Object IPAddress

    New-Object PSObject @{
        Emulator  = $Emulator.BaseName
        Version   = $Emulator.VersionInfo.ProductVersion
        Endpoint  = @($(hostname), $IPAddress.IPAddress) | ForEach-Object { "https://${_}:8081/" }
        IPAddress = $IPAddress.IPAddress
        Key       = $Key
    }
}

try {
    if ($Trace) {
        Register-EngineEvent PowerShell.Exiting -SupportEvent -Action (Get-Item function:\Copy-Diagnostics).ScriptBlock
        Set-PSDebug -Strict -Trace 1
    }

    $result = DoStart
}
catch {
    [Environment]::Exit(1)
}

$result

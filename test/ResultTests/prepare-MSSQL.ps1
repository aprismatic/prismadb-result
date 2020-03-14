param(
    [string]$ContainerName = 'UnitTestMSSQLServer',
    [string]$Port = 1433
)

"Starting up SQL Server container on port $Port... "

& "$PSScriptRoot\kill-MSSQL.ps1" $ContainerName

& docker pull mcr.microsoft.com/mssql/server:latest

& docker $dockerHost run -d -p ${Port}:1433 -e ACCEPT_EULA=Y -e SA_PASSWORD=Password12! --name $ContainerName mcr.microsoft.com/mssql/server:latest

if (!$?)
{
    ""
    Write-Host -NoNewline -Foreground "red" "ERROR: "
    "SQL Server failed to start. Exiting..."
    Exit 1
}


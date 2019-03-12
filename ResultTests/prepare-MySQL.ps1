param(
    [string]$ContainerName = 'UnitTestMySQLServer',
    [string]$Port = 3306
)

"Starting up MySQL 5.7 server container on port $Port... "

& "$PSScriptRoot\kill-MySQL.ps1" $ContainerName

& docker $dockerHost run -d -p ${Port}:3306 -e MYSQL_ROOT_PASSWORD=Password12! --name $ContainerName mysql

if (!$?)
{
    ""
    Write-Host -NoNewline -Foreground "red" "ERROR: "
    "MySQL server failed to start. Exiting..."
    Exit 1
}


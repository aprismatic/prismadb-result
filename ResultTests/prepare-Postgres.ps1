param(
    [string]$ContainerName = 'UnitTestPostgresServer',
    [string]$Port = 5432
)

"Starting up PostgreSQL server container on port $Port... "

& "$PSScriptRoot\kill-Postgres.ps1" $ContainerName

& docker $dockerHost run -d -p ${Port}:5432 -e POSTGRES_PASSWORD=Password12! --name $ContainerName postgres:latest

if (!$?)
{
    ""
    Write-Host -NoNewline -Foreground "red" "ERROR: "
    "PostgreSQL server failed to start. Exiting..."
    Exit 1
}


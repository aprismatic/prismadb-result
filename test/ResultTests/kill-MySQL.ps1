param(
    [string]$ContainerName = 'UnitTestMySQLServer'
)

"Stopping & removing containers named '$ContainerName'..."
& docker $dockerHost rm --force $ContainerName 2>&1 | Out-Null

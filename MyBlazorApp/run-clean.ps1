param(
    [int]$Port = 5100,
    [string]$ProjectPath = "."
)

$connections = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue
if ($connections) {
    $processIds = $connections | Select-Object -ExpandProperty OwningProcess -Unique
    foreach ($processId in $processIds) {
        Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
        Write-Host "Stopped process $processId on port $Port"
    }
}

Get-Process MyBlazorApp -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

Push-Location $ProjectPath
try {
    dotnet run
}
finally {
    Pop-Location
}

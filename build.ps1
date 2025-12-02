# Build script for CrateGuardCS plugin
param([string]$RustServerPath)

$projectName = "CrateGuardCS"
$libDir = "lib"
$outputDir = "oxide/plugins"

Write-Host "Building $projectName plugin..." -ForegroundColor Cyan

# Create lib directory
if (-not (Test-Path $libDir)) {
    New-Item -ItemType Directory -Path $libDir | Out-Null
}

# Copy assemblies from Rust server if path provided
if ($RustServerPath) {
    $rustManagedPath = Join-Path $RustServerPath "RustDedicated_Data\Managed"
    if (Test-Path $rustManagedPath) {
        Write-Host "Copying Rust assemblies..." -ForegroundColor Yellow
        Copy-Item "$rustManagedPath\UnityEngine.dll" $libDir -Force -ErrorAction SilentlyContinue
        Copy-Item "$rustManagedPath\Assembly-CSharp.dll" $libDir -Force -ErrorAction SilentlyContinue
    }
}

# Check for required assemblies
$required = @("Oxide.Core.dll", "Oxide.Rust.dll", "UnityEngine.dll", "Assembly-CSharp.dll")
$missing = @()

foreach ($dll in $required) {
    if (-not (Test-Path "$libDir\$dll")) {
        $missing += $dll
    }
}

if ($missing.Count -gt 0) {
    Write-Host "`nError: Missing assemblies in $libDir`:" -ForegroundColor Red
    $missing | ForEach-Object { Write-Host "  - $_" }
    Write-Host "`nPlace them in the '$libDir' directory and try again." -ForegroundColor Yellow
    exit 1
}

# Build
Write-Host "Compiling..." -ForegroundColor Cyan
dotnet build "$projectName.csproj" -c Release

if ($LASTEXITCODE -eq 0) {
    $dllPath = "$outputDir\$projectName.dll"
    if (Test-Path $dllPath) {
        $size = [math]::Round((Get-Item $dllPath).Length / 1KB, 2)
        Write-Host "`nBuild successful: $dllPath ($size KB)" -ForegroundColor Green
    }
}
else {
    Write-Host "`nBuild failed" -ForegroundColor Red
    exit 1
}

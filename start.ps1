$ErrorActionPreference = "Stop"
$src = Join-Path $PSScriptRoot "src\RestaurantOrders"
$dest = Join-Path $env:TEMP "RestaurantOrdersRun"

if (Test-Path $dest) {
    Remove-Item $dest -Recurse -Force
}

New-Item $dest -ItemType Directory | Out-Null
$utf8bom = New-Object System.Text.UTF8Encoding $true
$copied = New-Object System.Collections.Generic.List[string]

Get-ChildItem $src -Recurse -Filter *.cs |
    Where-Object { $_.FullName -notmatch '\\obj\\|\\bin\\' } |
    ForEach-Object {
        $rel = $_.FullName.Substring($src.Length).TrimStart('\')
        $target = Join-Path $dest $rel
        $dir = Split-Path $target
        if (-not (Test-Path $dir)) {
            New-Item $dir -ItemType Directory -Force | Out-Null
        }

        $text = [System.IO.File]::ReadAllText($_.FullName, (New-Object System.Text.UTF8Encoding $false))
        $text = $text -replace '\b(OrderLine|MenuItem|Customer|string|Order)\?', '$1'
        [System.IO.File]::WriteAllText($target, $text, $utf8bom)
        $copied.Add($target)
    }

try {
    $Host.UI.RawUI.WindowTitle = "Restaurant Orders"
} catch {
}

# Cikti Turkce kalsin; girdi ayarina dokunma, yoksa secim yazilamayabilir
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
Set-Location $src
Add-Type -Path $copied.ToArray()
[RestaurantOrders.Program]::Main()

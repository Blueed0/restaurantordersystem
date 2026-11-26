@echo off
cd /d "%~dp0"
start "Restaurant Orders" powershell.exe -NoProfile -ExecutionPolicy Bypass -NoExit -File "%~dp0start.ps1"

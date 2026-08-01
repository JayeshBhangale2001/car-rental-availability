# New Machine Setup Guide (Windows)

This guide is designed to make first-time setup predictable and low-friction.

It covers:
- Fresh machine prerequisites
- Backend and frontend startup
- Health checks
- Known Windows issues and exact fixes

## 1) Install Required Tools

Open PowerShell as Administrator and run:

```powershell
winget install --id Microsoft.DotNet.SDK.8 -e --accept-package-agreements --accept-source-agreements
winget install --id OpenJS.NodeJS.LTS -e --accept-package-agreements --accept-source-agreements
```

Then close all terminals and open a new PowerShell window.

Verify installations:

```powershell
dotnet --list-sdks
dotnet --list-runtimes
node -v
npm -v
```

Expected:
- .NET 8 SDK appears in `dotnet --list-sdks`
- `Microsoft.AspNetCore.App 8.x` appears in `dotnet --list-runtimes`
- Node and npm return versions

## 2) Clone and Open Project

```powershell
git clone <your-repo-url>
cd "car-rental-availability"
```

## 3) Start Backend API (Terminal 1)

```powershell
cd "D:\New folder\car-rental-availability\src"
dotnet restore CarRental.sln
dotnet run --project CarRental.Api --urls "http://localhost:5000"
```

Backend success signal:
- `Now listening on: http://localhost:5000`

## 4) Start Frontend (Terminal 2)

```powershell
cd "D:\New folder\car-rental-availability\ui"
npm install
npm start
```

Frontend success signal:
- `Local: http://localhost:4200/`

## 5) Verify Everything Is Running (Terminal 3)

```powershell
Invoke-WebRequest http://localhost:5000/swagger/index.html -UseBasicParsing
Invoke-WebRequest http://localhost:4200 -UseBasicParsing
```

If both return HTTP 200, setup is complete.

## 6) Important Project Port Mapping

The frontend proxy is configured in `ui/proxy.conf.json`:
- `/cars` -> `http://localhost:5000`

If backend port changes, update proxy target accordingly.

## 7) Troubleshooting (Windows)

### A) `node` is not recognized

Use this in current terminal session:

```powershell
$env:Path = "C:\Program Files\nodejs;$env:Path"
node -v
npm -v
```

If still missing, reinstall Node LTS and open a new terminal.

### B) `npm.ps1 cannot be loaded because running scripts is disabled`

Option 1 (recommended, no policy change):

```powershell
& "C:\Program Files\nodejs\npm.cmd" -v
& "C:\Program Files\nodejs\npm.cmd" install
& "C:\Program Files\nodejs\npm.cmd" start
```

Option 2 (temporary policy for this terminal only):

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
npm -v
```

### C) `.NET 8 framework missing` when starting backend

If you see app-launch failure for `Microsoft.AspNetCore.App 8.0.0`, install .NET 8 SDK/runtime:

```powershell
winget install --id Microsoft.DotNet.SDK.8 -e --accept-package-agreements --accept-source-agreements
```

Then verify:

```powershell
dotnet --list-runtimes
```

Ensure `Microsoft.AspNetCore.App 8.x` is listed.

### D) `localhost refused to connect`

Check listeners:

```powershell
netstat -ano | findstr :5000
netstat -ano | findstr :4200
```

If no process is listening:
- Restart backend command in Terminal 1
- Restart frontend command in Terminal 2

If port is occupied by another process:

```powershell
Get-Process -Id <PID>
```

Stop conflicting process or run with a different port and update proxy.

## 8) Recommended Daily Start Sequence

1. Terminal 1:
   - `cd src`
   - `dotnet run --project CarRental.Api --urls "http://localhost:5000"`
2. Terminal 2:
   - `cd ui`
   - `npm start`
3. Open `http://localhost:4200`

## 9) Copilot-Friendly Prompt You Can Reuse

Paste this in Copilot chat on a new machine:

"Set up this repo end-to-end on Windows. Verify Node, npm, and .NET 8. Install missing tools. Start backend on localhost:5000 and frontend on localhost:4200. If PowerShell blocks npm.ps1, use npm.cmd. If node is not recognized, fix PATH in-session. Then run health checks for both endpoints and report results."

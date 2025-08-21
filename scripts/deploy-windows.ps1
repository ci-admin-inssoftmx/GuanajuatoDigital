# Deploy Script for GuanajuatoDigital - Windows Server
param(
    [Parameter(Mandatory=$true)]
    [string]$Environment,
    
    [Parameter(Mandatory=$true)]
    [string]$PublishPath,
    
    [string]$SiteName = "GuanajuatoDigital",
    [string]$Port = "9080",
    [string]$AppPoolName = "GuanajuatoDigitalAppPool"
)

# Configuration por ambiente
$config = @{
    "DEV" = @{
        Port = "9080"
        Path = "C:\inetpub\wwwroot\GuanajuatoDigital-DEV"
        Url = "http://dev-guanajuato.local:9080"
    }
    "PRE" = @{
        Port = "9090"
        Path = "C:\inetpub\wwwroot\GuanajuatoDigital-PRE"
        Url = "http://pre-guanajuato.local:9090"
    }
    "PRO" = @{
        Port = "9100"
        Path = "C:\inetpub\wwwroot\GuanajuatoDigital-PRO"
        Url = "http://pro-guanajuato.local:9100"
    }
}

$envConfig = $config[$Environment]
if (-not $envConfig) {
    Write-Error "Environment '$Environment' not supported. Use: DEV, PRE, or PRO"
    exit 1
}

Write-Host "🚀 Starting deployment to $Environment environment..." -ForegroundColor Green
Write-Host "📂 Deploy path: $($envConfig.Path)" -ForegroundColor Yellow
Write-Host "🌐 URL: $($envConfig.Url)" -ForegroundColor Yellow

try {
    # Import WebAdministration module
    Import-Module WebAdministration -ErrorAction Stop
    
    # Stop Application Pool if exists
    Write-Host "⏹️ Stopping Application Pool: $AppPoolName-$Environment"
    if (Get-IISAppPool -Name "$AppPoolName-$Environment" -ErrorAction SilentlyContinue) {
        Stop-WebAppPool -Name "$AppPoolName-$Environment"
        Start-Sleep -Seconds 5
    }
    
    # Stop Website if exists
    Write-Host "⏹️ Stopping Website: $SiteName-$Environment"
    if (Get-Website -Name "$SiteName-$Environment" -ErrorAction SilentlyContinue) {
        Stop-Website -Name "$SiteName-$Environment"
        Start-Sleep -Seconds 3
    }
    
    # Create backup of existing deployment
    if (Test-Path $envConfig.Path) {
        $backupPath = "$($envConfig.Path)_backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
        Write-Host "💾 Creating backup: $backupPath"
        Copy-Item -Path $envConfig.Path -Destination $backupPath -Recurse -Force
    }
    
    # Ensure deployment directory exists
    Write-Host "📁 Preparing deployment directory..."
    if (-not (Test-Path $envConfig.Path)) {
        New-Item -ItemType Directory -Path $envConfig.Path -Force | Out-Null
    } else {
        # Clear existing files (keep backup)
        Get-ChildItem -Path $envConfig.Path | Remove-Item -Recurse -Force
    }
    
    # Copy new files
    Write-Host "📋 Copying application files..."
    Copy-Item -Path "$PublishPath\*" -Destination $envConfig.Path -Recurse -Force
    
    # Create logs directory
    $logsPath = Join-Path $envConfig.Path "logs"
    if (-not (Test-Path $logsPath)) {
        New-Item -ItemType Directory -Path $logsPath -Force | Out-Null
    }
    
    # Set proper permissions
    Write-Host "🔐 Setting permissions..."
    $acl = Get-Acl $envConfig.Path
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($accessRule)
    Set-Acl -Path $envConfig.Path -AclObject $acl
    
    # Create Application Pool
    Write-Host "🏊 Creating Application Pool: $AppPoolName-$Environment"
    if (Get-IISAppPool -Name "$AppPoolName-$Environment" -ErrorAction SilentlyContinue) {
        Remove-WebAppPool -Name "$AppPoolName-$Environment"
    }
    
    New-WebAppPool -Name "$AppPoolName-$Environment" -Force
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName-$Environment" -Name "managedRuntimeVersion" -Value ""
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName-$Environment" -Name "enable32BitAppOnWin64" -Value $false
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName-$Environment" -Name "processModel.identityType" -Value "ApplicationPoolIdentity"
    
    # Create Website
    Write-Host "🌐 Creating Website: $SiteName-$Environment"
    if (Get-Website -Name "$SiteName-$Environment" -ErrorAction SilentlyContinue) {
        Remove-Website -Name "$SiteName-$Environment"
    }
    
    New-Website -Name "$SiteName-$Environment" -Port $envConfig.Port -PhysicalPath $envConfig.Path -ApplicationPool "$AppPoolName-$Environment"
    
    # Start Application Pool
    Write-Host "▶️ Starting Application Pool..."
    Start-WebAppPool -Name "$AppPoolName-$Environment"
    Start-Sleep -Seconds 3
    
    # Start Website
    Write-Host "▶️ Starting Website..."
    Start-Website -Name "$SiteName-$Environment"
    Start-Sleep -Seconds 5
    
    Write-Host "✅ Deployment completed successfully!" -ForegroundColor Green
    Write-Host "🌐 Application is available at: $($envConfig.Url)" -ForegroundColor Green
    Write-Host "📊 Application Pool: $AppPoolName-$Environment" -ForegroundColor Yellow
    Write-Host "📂 Physical Path: $($envConfig.Path)" -ForegroundColor Yellow
    
    # Return deployment info
    return @{
        Status = "Success"
        Url = $envConfig.Url
        Path = $envConfig.Path
        AppPool = "$AppPoolName-$Environment"
        Environment = $Environment
    }
    
} catch {
    Write-Error "❌ Deployment failed: $($_.Exception.Message)"
    Write-Error $_.ScriptStackTrace
    
    # Attempt rollback if backup exists
    $backupPath = Get-ChildItem -Path "$(Split-Path $envConfig.Path)\*backup*" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($backupPath) {
        Write-Host "🔄 Attempting rollback from: $($backupPath.FullName)" -ForegroundColor Yellow
        try {
            Remove-Item -Path $envConfig.Path -Recurse -Force -ErrorAction SilentlyContinue
            Copy-Item -Path $backupPath.FullName -Destination $envConfig.Path -Recurse -Force
            Start-WebAppPool -Name "$AppPoolName-$Environment" -ErrorAction SilentlyContinue
            Start-Website -Name "$SiteName-$Environment" -ErrorAction SilentlyContinue
            Write-Host "✅ Rollback completed" -ForegroundColor Green
        } catch {
            Write-Error "❌ Rollback failed: $($_.Exception.Message)"
        }
    }
    
    exit 1
}

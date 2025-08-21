# Health Check Script for GuanajuatoDigital
param(
    [Parameter(Mandatory=$true)]
    [string]$Url,
    
    [int]$MaxRetries = 5,
    [int]$RetryInterval = 10,
    [switch]$Quick,
    [switch]$Detailed
)

function Test-ApplicationHealth {
    param(
        [string]$TestUrl,
        [bool]$IsDetailed = $false
    )
    
    $healthStatus = @{
        Url = $TestUrl
        Timestamp = Get-Date
        Status = "Unknown"
        ResponseTime = 0
        StatusCode = 0
        Headers = @{}
        Content = ""
        Errors = @()
    }
    
    try {
        $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
        
        # Test basic connectivity
        $response = Invoke-WebRequest -Uri $TestUrl -UseBasicParsing -TimeoutSec 30 -ErrorAction Stop
        
        $stopwatch.Stop()
        $healthStatus.ResponseTime = $stopwatch.ElapsedMilliseconds
        $healthStatus.StatusCode = $response.StatusCode
        $healthStatus.Headers = $response.Headers
        
        if ($IsDetailed) {
            $healthStatus.Content = $response.Content
        }
        
        # Determine health status
        if ($response.StatusCode -eq 200) {
            $healthStatus.Status = "Healthy"
        } elseif ($response.StatusCode -ge 200 -and $response.StatusCode -lt 400) {
            $healthStatus.Status = "Warning"
        } else {
            $healthStatus.Status = "Unhealthy"
        }
        
        # Additional checks for ASP.NET Core apps
        if ($response.Headers.ContainsKey("Server")) {
            $server = $response.Headers["Server"]
            if ($server -like "*Kestrel*" -or $server -like "*IIS*") {
                Write-Verbose "Detected ASP.NET Core application"
            }
        }
        
    } catch {
        $healthStatus.Status = "Unhealthy"
        $healthStatus.Errors += $_.Exception.Message
        Write-Verbose "Health check failed: $($_.Exception.Message)"
    }
    
    return $healthStatus
}

function Show-HealthReport {
    param($HealthData)
    
    Write-Host "`n🏥 Health Check Report" -ForegroundColor Cyan
    Write-Host "=" * 50 -ForegroundColor Cyan
    Write-Host "🌐 URL: $($HealthData.Url)" -ForegroundColor Yellow
    Write-Host "⏰ Timestamp: $($HealthData.Timestamp)" -ForegroundColor Yellow
    
    switch ($HealthData.Status) {
        "Healthy" { 
            Write-Host "✅ Status: $($HealthData.Status)" -ForegroundColor Green 
        }
        "Warning" { 
            Write-Host "⚠️ Status: $($HealthData.Status)" -ForegroundColor Yellow 
        }
        "Unhealthy" { 
            Write-Host "❌ Status: $($HealthData.Status)" -ForegroundColor Red 
        }
        default { 
            Write-Host "❓ Status: $($HealthData.Status)" -ForegroundColor Gray 
        }
    }
    
    Write-Host "📊 HTTP Status: $($HealthData.StatusCode)" -ForegroundColor Yellow
    Write-Host "⚡ Response Time: $($HealthData.ResponseTime)ms" -ForegroundColor Yellow
    
    if ($HealthData.Errors.Count -gt 0) {
        Write-Host "❌ Errors:" -ForegroundColor Red
        $HealthData.Errors | ForEach-Object { Write-Host "   - $_" -ForegroundColor Red }
    }
    
    if ($Detailed -and $HealthData.Content) {
        Write-Host "`n📄 Response Content (first 500 chars):" -ForegroundColor Cyan
        Write-Host $HealthData.Content.Substring(0, [Math]::Min(500, $HealthData.Content.Length)) -ForegroundColor Gray
    }
    
    Write-Host "=" * 50 -ForegroundColor Cyan
}

# Main execution
Write-Host "🏥 Starting health check for: $Url" -ForegroundColor Green

if ($Quick) {
    # Quick check - single attempt
    $health = Test-ApplicationHealth -TestUrl $Url -IsDetailed $Detailed
    Show-HealthReport -HealthData $health
    
    if ($health.Status -eq "Healthy") {
        Write-Host "✅ Quick health check PASSED" -ForegroundColor Green
        exit 0
    } else {
        Write-Host "❌ Quick health check FAILED" -ForegroundColor Red
        exit 1
    }
} else {
    # Full check with retries
    $attempt = 1
    $lastError = ""
    
    while ($attempt -le $MaxRetries) {
        Write-Host "🔄 Attempt $attempt of $MaxRetries..." -ForegroundColor Yellow
        
        $health = Test-ApplicationHealth -TestUrl $Url -IsDetailed $Detailed
        
        if ($health.Status -eq "Healthy") {
            Show-HealthReport -HealthData $health
            Write-Host "✅ Health check PASSED on attempt $attempt" -ForegroundColor Green
            exit 0
        } else {
            $lastError = if ($health.Errors.Count -gt 0) { $health.Errors[0] } else { "Unknown error" }
            Write-Host "❌ Attempt $attempt failed: $lastError" -ForegroundColor Red
            
            if ($attempt -lt $MaxRetries) {
                Write-Host "⏳ Waiting $RetryInterval seconds before retry..." -ForegroundColor Yellow
                Start-Sleep -Seconds $RetryInterval
            }
        }
        
        $attempt++
    }
    
    # All attempts failed
    Show-HealthReport -HealthData $health
    Write-Host "❌ Health check FAILED after $MaxRetries attempts" -ForegroundColor Red
    Write-Host "🔍 Last error: $lastError" -ForegroundColor Red
    exit 1
}

# Additional health checks for production
if ($Url -like "*pro*" -or $Url -like "*production*") {
    Write-Host "`n🔒 Running additional production health checks..." -ForegroundColor Cyan
    
    # Check SSL certificate if HTTPS
    if ($Url -like "https://*") {
        try {
            $uri = [System.Uri]$Url
            $tcpClient = New-Object System.Net.Sockets.TcpClient
            $tcpClient.Connect($uri.Host, 443)
            $sslStream = New-Object System.Net.Security.SslStream($tcpClient.GetStream())
            $sslStream.AuthenticateAsClient($uri.Host)
            
            $cert = $sslStream.RemoteCertificate
            $expiryDate = [DateTime]::Parse($cert.GetExpirationDateString())
            $daysUntilExpiry = ($expiryDate - (Get-Date)).Days
            
            if ($daysUntilExpiry -lt 30) {
                Write-Host "⚠️ SSL Certificate expires in $daysUntilExpiry days!" -ForegroundColor Yellow
            } else {
                Write-Host "✅ SSL Certificate valid until $expiryDate" -ForegroundColor Green
            }
            
            $sslStream.Close()
            $tcpClient.Close()
        } catch {
            Write-Host "❌ SSL Check failed: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

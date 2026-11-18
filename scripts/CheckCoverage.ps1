Push-Location C:\Dev\GitRepos\CheersDb

dotnet test --configuration Release --results-directory .\TestResults --coverage --coverage-output coverage.cobertura.xml --coverage-output-format cobertura
[xml]$testCoverage = Get-Content .\TestResults\coverage.cobertura.xml
$branchRate = [double]$testCoverage.coverage.'branch-rate'
$lineRate = [double]$testCoverage.coverage.'line-rate'

Write-Host ""
Write-Host "Branch Coverage: $($branchRate.ToString('P2'))"
Write-Host "Line Coverage: $($lineRate.ToString('P2'))"
Write-Host ""

Pop-Location
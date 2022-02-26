# Check the code is in sync
$changed = (select-string "nothing to commit" artifacts\status.txt).count -eq 0
if (-not $changed) { return $changed }
# Check if tracking issue is open/closed
$Headers = @{ Authorization = 'token {0}' -f $ENV:GITHUB_TOKEN; };
$result = Invoke-RestMethod -Uri $issue
if ($result.state -eq "closed") { 
 $json = "{ `"state`": `"open`" }"
 $result = Invoke-RestMethod -Method PATCH -Headers $Headers -Uri $issue -Body $json
}
# Add a comment
$status = [IO.File]::ReadAllText("artifacts\status.txt")
$diff = [IO.File]::ReadAllText("artifacts\diff.txt")
$body = @"
The shared code is out of sync.
<details>
  <summary>The Diff</summary>

``````
$status
$diff
``````

</details>
"@
$json = ConvertTo-Json -InputObject @{ 'body' = $body }
$issue = $issue + '/comments'
$result = Invoke-RestMethod -Method POST -Headers $Headers -Uri $issue -Body $json

# Check if there's an open PR in Scrapy.NET or Runtime to resolve this difference.
$sendpr = $true
$Headers = @{ Accept = 'application/vnd.github.v3+json' };

$prsLink = "https://api.github.com/repos/scrapy-net/scrapy.net/pulls?state=open"
$result = Invoke-RestMethod -Method GET -Headers $Headers -Uri $prsLink

foreach ($pr in $result) {
  if ($pr.body -And $pr.body.Contains("Fixes #18943")) {
    $sendpr = $false
    return $sendpr
  }
}

$prsLink = "https://api.github.com/repos/scrapy-net/runtime/pulls?state=open"
$result = Invoke-RestMethod -Method GET -Headers $Headers -Uri $prsLink

foreach ($pr in $result) {
  if ($pr.body -And $pr.body.Contains("Fixes https://github.com/scrapy-net/scrapy.net/issues/18943")) {
    $sendpr = $false
    return $sendpr
  }
}

return $sendpr
write-host "Updating GlobalAssemblyInfo.cs with Build Number $env:build_buildnumber."
(get-content "GlobalAssemblyInfo.cs") -replace '0.0.0.0', $env:build_buildnumber | set-content "GlobalAssemblyInfo.cs"

write-host "Updating GlobalAssemblyInfo.cs with Source Version $env:build_sourceversion."
(get-content "GlobalAssemblyInfo.cs") -replace '1.1.1.1', $env:build_sourceversion | set-content "GlobalAssemblyInfo.cs"

write-host "GlobalAssemblyInfo.cs updated to..."
(get-content "GlobalAssemblyInfo.cs")

write-host "Update Complete."
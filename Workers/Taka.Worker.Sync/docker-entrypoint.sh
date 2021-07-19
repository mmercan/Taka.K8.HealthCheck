#!/bin/bash

dotnet sonarscanner begin /k:$PROJECT /v:$VERSION /d:sonar.host.url=$SONAR_HOST /o:mmercan-github /d:sonar.login=$SONARKEY  /d:sonar.cs.opencover.reportsPaths="/TestResults/coverage.opencover.xml" /d:sonar.exclusions="**bootstrap.css, **bootstrap-reboot.css, **bootstrap.js, **/wwwroot/**, **Empty.Tests" /d:sonar.coverage.exclusions="**Tests*.cs, **.js," /d:sonar.cs.vstest.reportsPaths="/TestResults/*.trx"
dotnet build ./Workers/Taka.Worker.Sync/Taka.Worker.Sync.sln

# dotnet test ./Workers/Taka.Worker.Sync/Taka.Worker.Sync.sln   /p:CollectCoverage=true /p:CoverletOutput=/TestResults/ /p:MergeWith=/TestResults/coverage.json --logger=trx -r /TestResults/
# dotnet test ./Empty.Tests/Empty.Tests.sln /p:CollectCoverage=true /p:MergeWith="/TestResults/coverage.json" /p:CoverletOutputFormat="opencover" /p:CoverletOutput=/TestResults/

dotnet test ./Workers/Taka.Worker.Sync/Taka.Worker.Sync.sln  /p:CollectCoverage=true /p:Exclude="[xunit.*.*]*" /p:CoverletOutput=/TestResults/ /p:MergeWith=/TestResults/coverage.json --logger=trx -r /TestResults/
dotnet test ./Empty.Tests/Empty.Tests.sln /p:CollectCoverage=true /p:MergeWith="/TestResults/coverage.json" /p:CoverletOutputFormat="opencover" /p:CoverletOutput=/TestResults/ 

dotnet build-server shutdown
dotnet sonarscanner end /d:sonar.login=$SONARKEY

 
dotnet build --configuration Debug | out-null
$path = ".\ScheduledFileIODemo\bin\Debug\netcoreapp2.2\ScheduledFileIODemo.dll"

write-host "Starting 4 tests with various settings"
write-host "-----------------------"
dotnet $path
write-host "-----------------------"
dotnet $path -optimized
write-host "-----------------------"
dotnet $path -filecount 1000 -testcount 20
write-host "-----------------------"
dotnet $path -filecount 1000 -testcount 20 -optimized
write-host "-----------------------"
write-host "Tests complete"
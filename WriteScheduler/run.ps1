dotnet build --configuration Debug | out-null
$path = ".\ScheduledFileIODemo\bin\Debug\netcoreapp2.2\ScheduledFileIODemo.dll"

write-host "Starting 4 tests with various settings"
write-host "======================================"
dotnet $path -filecount 5000 -testcount 20 -mockwrite
write-host "======================================"
dotnet $path -filecount 1000 -testcount 20 -mockwrite
write-host "======================================"
write-host "Tests complete"
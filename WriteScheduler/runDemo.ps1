write-host "Running test suite..."
write-host "======================================"
dotnet test ".\ScheduledFileIOTests\"
write-host "======================================"
write-host "Tests complete"

dotnet build --configuration Debug | out-null
$path = ".\ScheduledFileIODemo\bin\Debug\netcoreapp2.2\ScheduledFileIODemo.dll"

write-host "======================================"
write-host "Running 2 demo instances with various settings"
write-host "======================================"
dotnet $path -testcount 20
write-host "======================================"
dotnet $path -filecount 1000 -testcount 20
write-host "======================================"
write-host "Demos complete"
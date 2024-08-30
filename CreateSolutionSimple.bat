
dotnet new sln -o Lueben.Depot

cd Lueben.Depot

dotnet new hwnorch -o Lueben.Depot.Orchestrator
dotnet new hwnms -t EmailAdapter -o Lueben.Depot.EmailAdapter
dotnet new hwnjob -t FileProcessing -o Lueben.Depot.FileProcessing
dotnet new hwnapi -o Lueben.Depot

dotnet new hwnmsnuget
dotnet new hwnstylecop

mkdir lib
cd lib
dotnet new hwnstubdll

cd ..

dotnet restore

start "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\devenv.exe" Lueben.Depot.sln
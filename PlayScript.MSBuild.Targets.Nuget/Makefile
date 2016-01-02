CONFIG?=Debug

all: task pack content

pack:
	nuget pack

task:
	cd ../PlayScript.MSBuild.Tasks; xbuild PlayScript.MSBuild.Tasks.csproj /property:Configuration=Release
	cp ../PlayScript.MSBuild.Tasks/bin/Release/PlayScript.MSBuild.Tasks.dll build 

clean:
	-rm PlayScript.MSBuild.*.nupkg
	-cd ../PlayScript.MSBuild.Tasks; xbuild PlayScript.MSBuild.Tasks.csproj /target:Clean /property:Configuration=Release
	-cd ../PlayScript.MSBuild.Tasks; xbuild PlayScript.MSBuild.Tasks.csproj /target:Clean /property:Configuration=Debug

publish: clean all
	nuget push PlayScript.MSBuild.*.nupkg $(MyGetApiKey) -source https://www.myget.org/F/playscript/api/v2

content:
	unzip -l PlayScript.MSBuild.*.nupkg

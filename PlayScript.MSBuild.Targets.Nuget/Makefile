CONFIG?=Debug

all:
	nuget pack 

clean:
	rm PlayScript.MSBuild.*.nupkg

publish: clean all
	nuget push PlayScript.MSBuild.*.nupkg $(MyGetApiKey) -source https://www.myget.org/F/playscript/api/v2

content:
	unzip -l PlayScript.MSBuild.*.nupkg

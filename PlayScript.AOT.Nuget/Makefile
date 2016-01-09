
all: clean copy pack content

pack:
	nuget pack

clean:
	-find lib -name "*.dll" -print0 | xargs -0 -n 1 rm
	-rm PlayScript.AOT.*.nupkg

content:
	unzip -l PlayScript.AOT.*.nupkg

copy:
	cp ../../playscript/mcs/class/lib/net_4_5/pscorlib_aot.dll lib/net45
	cp ../../playscript/mcs/class/lib/net_4_5/PlayScript.Dynamic_aot.dll lib/net45

publish: all
	source mygetapikey.sh
	nuget push PlayScript.AOT.*.nupkg $(MyGetApiKey) -source https://www.myget.org/F/playscript/api/v2

using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"PlayScript",
	Namespace = "MonoDevelop",
	Version = MonoDevelop.BuildInfo.Version,
	Url = "http://github.com/playscriptredux"
)]

[assembly:AddinName ("PlayScript Language Bindings")]
[assembly:AddinCategory ("Language bindings")]
[assembly:AddinDescription ("Bringing ActionScript to Xamarin/Mono")]
[assembly:AddinAuthor ("SushiHangover/RobertN")]

//<Runtime>
//	<Import assembly="ICSharpCode.NRefactory.PlayScript.dll" />
//	<Import assembly="MonoDevelop.PlayScriptBinding.dll" />
//	<Import assembly="PlayScript.Dynamic.dll" />
//	<Import assembly="PlayScript.Dynamic_aot.dll" />
//	<Import assembly="pscorlib.dll" />
//	<Import assembly="pscorlib_aot.dll" />
//	<Import assembly="Mono.PlayScript.dll" />
//	<Import file="playc.exe" />
//	<Import file="playc" />
//</Runtime>
//
//<Extension path="/MonoDevelop/Core/SupportPackages">
//	<Package name="pscorlib" version="1.0" clrVersion="Default">
//	<Assembly file="PlayScript.Dynamic.dll" />
//	<Assembly file="pscorlib.dll" />
//	<Assembly file="Mono.PlayScript.dll" />
//	</Package>
//	<Package name="pscorlib_aot" version="1.0" clrVersion="Default">
//	<Assembly file="PlayScript.Dynamic_aot.dll" />
//	<Assembly file="pscorlib_aot.dll" />
//	</Package>
//</Extension>

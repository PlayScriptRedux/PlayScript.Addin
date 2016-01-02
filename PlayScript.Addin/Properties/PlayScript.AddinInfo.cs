using System;
using Mono.Addins;
using Mono.Addins.Description;
using PlayScript;

[assembly:Addin (
	"PlayScript",
	Namespace = "MonoDevelop",
	Version = PlayScript.Addin.Version,
	Flags = AddinFlags.None,
	Category = "Language bindings",
	Url = "https://github.com/PlayScriptRedux/PlayScript.Addin"
)]

[assembly:AddinName ("PlayScript Language Bindings")]
[assembly:AddinDescription ("PlayScript and ActionScript Support for Xamarin Studio\n\nBy SushiHangover")]
[assembly:AddinAuthor ("SushiHangover/RobertN")]

[assembly:Mono.Addins.ImportAddinAssembly ("ICSharpCode.Decompiler.dll")]
[assembly:Mono.Addins.ImportAddinAssembly ("ICSharpCode.NRefactory.PlayScript.IKVM.dll")]
[assembly:Mono.Addins.ImportAddinAssembly ("ICSharpCode.NRefactory.PlayScript.Refactoring.dll")]
[assembly:Mono.Addins.ImportAddinAssembly ("ICSharpCode.NRefactory.PlayScript.Xml.dll")]
[assembly:Mono.Addins.ImportAddinAssembly ("ICSharpCode.NRefactory.PlayScript.dll")]
[assembly:Mono.Addins.ImportAddinAssembly ("IKVM.Reflection.dll")]
[assembly:Mono.Addins.ImportAddinAssembly ("Mono.Cecil.Mdb.dll")]
[assembly:Mono.Addins.ImportAddinAssembly ("Mono.Cecil.Pdb.dll")]
[assembly:Mono.Addins.ImportAddinAssembly ("Mono.Cecil.dll")]

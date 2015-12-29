using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"PlayScript",
	Namespace = "MonoDevelop",
	Version = MonoDevelop.BuildInfo.Version,
	Flags = AddinFlags.None,
	Category = "Language bindings",
	Url = "https://github.com/PlayScriptRedux/PlayScript.Addin"
)]

[assembly:AddinName ("PlayScript Language Bindings")]
[assembly:AddinDescription ("PlayScript and ActionScript Support for Xamarin Studio\n\nBy SushiHangover")]
[assembly:AddinAuthor ("SushiHangover/RobertN")]



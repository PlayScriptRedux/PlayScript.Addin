using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Collections.Generic;

using MonoDevelop.Projects;
using MonoDevelop.Core;
using Mono.TextEditor.Highlighting;

namespace PlayScript.Addin
{
	public class PlayScriptLanguageBinding : IDotNetLanguageBinding
	{
		public PlayScriptLanguageBinding() {
			SyntaxModeService.LoadStylesAndModes (Assembly.GetExecutingAssembly ());
		}
//		CSharpCodeProvider provider;

		// Keep the platforms combo of CodeGenerationPanelWidget in sync with this list
		public static IList<string> SupportedPlatforms = new string[] { "anycpu", "x86", "x64", "itanium" };

		public string Language {
			get {
				return "PlayScript";
			}
		}

		public string ProjectStockIcon {
			get { 
// TODO: Add custom icon
//				return "md-playscript-project";
				return "md-project";
			}
		}

		public FilePath GetFileName (FilePath baseName)
		{
			return baseName + ".play";
		}
			
		public bool IsSourceCodeFile (FilePath fileName)
		{
			return StringComparer.OrdinalIgnoreCase.Equals (Path.GetExtension (fileName), ".play");
		}

		public string SingleLineCommentTag { get { return "//"; } }
		public string BlockCommentStartTag { get { return "/*"; } }
		public string BlockCommentEndTag { get { return "*/"; } }


		public BuildResult Compile (ProjectItemCollection projectItems, DotNetProjectConfiguration configuration, ConfigurationSelector configSelector, IProgressMonitor monitor)
		{
//			return CSharpBindingCompilerManager.Compile (projectItems, configuration, configSelector, monitor);
			return null;
		}

		public ConfigurationParameters CreateCompilationParameters (XmlElement projectOptions)
		{
//			PlayScriptCompilerParameters pars = new PlayScriptCompilerParameters ();
//			if (projectOptions != null) {
//				string platform = projectOptions.GetAttribute ("Platform");
//				if (SupportedPlatforms.Contains (platform))
//					pars.PlatformTarget = platform;
//				string debugAtt = projectOptions.GetAttribute ("DefineDebug");
//				if (string.Compare ("True", debugAtt, StringComparison.OrdinalIgnoreCase) == 0)
//					pars.AddDefineSymbol ("DEBUG");
//				string releaseAtt = projectOptions.GetAttribute ("Release");
//				if (string.Compare ("True", releaseAtt, StringComparison.OrdinalIgnoreCase) == 0)
//					pars.Optimize = true;
//			}
//			return pars;
			return null;
		}

		public ProjectParameters CreateProjectParameters (XmlElement projectOptions)
		{
			return new PlayScriptProjectParameters ();
		}


		public CodeDomProvider GetCodeDomProvider ()
		{
//			if (provider == null)
//				provider = new CSharpEnhancedCodeProvider ();
//			return provider;
			return null;
		}

		public ClrVersion[] GetSupportedClrVersions ()
		{
			return new ClrVersion[] { 
				ClrVersion.Net_1_1, 
				ClrVersion.Net_2_0, 
				ClrVersion.Clr_2_1,
				ClrVersion.Net_4_0,
				ClrVersion.Net_4_5,
			};
		}
	}
}


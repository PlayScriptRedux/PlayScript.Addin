using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Xml;
using System.CodeDom.Compiler;
using System.Threading;
using Microsoft.CSharp;

using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Core.Instrumentation;

//using MonoDevelop.PlayScript.Parser;
using ICSharpCode.NRefactory.PlayScript;
using MonoDevelop.PlayScript.Formatting;
using MonoDevelop.PlayScript.Project;

namespace MonoDevelop.PlayScript
{
	class PlayScriptLanguageBinding : IDotNetLanguageBinding
	{
		CSharpCodeProvider provider;
		
		// Keep the platforms combo of CodeGenerationPanelWidget in sync with this list
		public static IList<string> SupportedPlatforms = new string[] { "anycpu", "x86", "x64", "itanium" };
	
		public string Language {
			get {
				return "PlayScript";
			}
		}
		
		public string ProjectStockIcon {
			get { 
				return "md-playscript-project";
			}
		}
		
		public bool IsSourceCodeFile (FilePath fileName)
		{
			return StringComparer.OrdinalIgnoreCase.Equals (Path.GetExtension (fileName), ".play");
		}
		
		public BuildResult Compile (ProjectItemCollection projectItems, DotNetProjectConfiguration configuration, ConfigurationSelector configSelector, IProgressMonitor monitor)
		{
			return PlayScriptBindingCompilerManager.Compile (projectItems, configuration, configSelector, monitor);
		}
		
		public ConfigurationParameters CreateCompilationParameters (XmlElement projectOptions)
		{
			PlayScriptCompilerParameters pars = new PlayScriptCompilerParameters ();
			if (projectOptions != null) {
				string platform = projectOptions.GetAttribute ("Platform");
				if (SupportedPlatforms.Contains (platform))
					pars.PlatformTarget = platform;
				string debugAtt = projectOptions.GetAttribute ("DefineDebug");
				if (string.Compare ("True", debugAtt, StringComparison.OrdinalIgnoreCase) == 0) {
					pars.DefineSymbols = "DEBUG";
					pars.DebugType = "full";
				}
				string releaseAtt = projectOptions.GetAttribute ("Release");
				if (string.Compare ("True", releaseAtt, StringComparison.OrdinalIgnoreCase) == 0)
					pars.Optimize = true;
			}
			return pars;
		}
	
		public ProjectParameters CreateProjectParameters (XmlElement projectOptions)
		{
			return new PlayScriptProjectParameters ();
		}
		
		public string SingleLineCommentTag { get { return "//"; } }
		public string BlockCommentStartTag { get { return "/*"; } }
		public string BlockCommentEndTag { get { return "*/"; } }
		
		public CodeDomProvider GetCodeDomProvider ()
		{
			if (provider == null)
				provider = new CSharpEnhancedCodeProvider ();
			return provider;
		}
		
		public FilePath GetFileName (FilePath baseName)
		{
			return baseName + ".play";
		}

		public ClrVersion[] GetSupportedClrVersions ()
		{
			return new ClrVersion[] { 
				ClrVersion.Net_1_1, 
				ClrVersion.Net_2_0, 
				ClrVersion.Clr_2_1,
				ClrVersion.Net_4_0,
				ClrVersion.Net_4_5
			};
		}
	}
	
//	internal static class Counters
//	{
//		public static Counter ResolveTime = InstrumentationService.CreateCounter ("Resolve Time", "Timing");
//	}
}

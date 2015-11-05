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
	public class ActionScriptLanguageBinding : IDotNetLanguageBinding
	{
		public ActionScriptLanguageBinding() {
			SyntaxModeService.LoadStylesAndModes (Assembly.GetExecutingAssembly ());
		}

		public static IList<string> SupportedPlatforms = new string[] { "anycpu", "x86", "x64", "itanium" };

		public string Language {
			get {
				return "ActionScript";
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
			return baseName + ".as";
		}
			
		public bool IsSourceCodeFile (FilePath fileName)
		{
			return StringComparer.OrdinalIgnoreCase.Equals (Path.GetExtension (fileName), ".as");
		}

		public string SingleLineCommentTag { get { return "//"; } }
		public string BlockCommentStartTag { get { return "/*"; } }
		public string BlockCommentEndTag { get { return "*/"; } }


		public BuildResult Compile (ProjectItemCollection projectItems, DotNetProjectConfiguration configuration, ConfigurationSelector configSelector, IProgressMonitor monitor)
		{
			return null;
		}

		public ConfigurationParameters CreateCompilationParameters (XmlElement projectOptions)
		{
			return null;
		}

		public ProjectParameters CreateProjectParameters (XmlElement projectOptions)
		{
			return new PlayScriptProjectParameters ();
		}


		public CodeDomProvider GetCodeDomProvider ()
		{
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


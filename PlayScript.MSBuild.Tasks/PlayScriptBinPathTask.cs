using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace PlayScript.MSBuild.Tasks
{
	public class PlayScriptBinPathTask : Task
	{
		[DllImport ("libc")]
		private static extern int system (string exec);

		String _PlayScriptCompiler = "NoSet";
		String _PlayScriptSDKPath = "NoSet";

		public PlayScriptBinPathTask ()
		{
		}

		public override bool Execute() {
			Console.WriteLine ("PlayScriptBinPathTask Execute");
			string psc_name = "psc";
			if (Environment.OSVersion.Platform != PlatformID.Unix) {
				psc_name += ".exe";
			}
			_PlayScriptCompiler = psc_name;
			Console.WriteLine ("PlayScriptBinPathTask {0}", _PlayScriptCompiler);

			// Does psc exist on the path?
			string psc = GetFullPath(psc_name);
			if (psc != null) {
				_PlayScriptSDKPath = psc;
				return true;
			}
			// Does psc exist within the XS Addin dir?
			var home = Environment.GetEnvironmentVariable("HOME");
			// TODO: this needs to be done via CONST and x-plat
			var psc_addin = Path.Combine (home, "Library/Application Support/XamarinStudio-5.0/LocalInstall/Addins");
			psc_addin = Path.Combine(psc_addin, "MonoDevelop.PlayScript.5.10.2");
			psc_addin = Path.Combine (psc_addin, "MonoDevelop.PlayScript.SupportPackages");
			var psc_full_path = Path.Combine (psc_addin, psc_name);
			if (File.Exists(psc_full_path)) {
				// TODO: Fixme...Hack file perms on script as Addin installer does not preserve them...
//				system (String.Format ("chmod u+x \"{0}\"", psc));
				_PlayScriptSDKPath = psc_addin;
				Console.WriteLine ("PlayScriptBinPathTask {0}", PlayScriptSDKPath);
				return true;
			}
			_PlayScriptSDKPath = "";
			return false;
		}

		[Output]
		public String PlayScriptCompiler
		{
			get {
				return _PlayScriptCompiler;
			}
		}

		[Output]
		public String PlayScriptSDKPath
		{
			get {
				return _PlayScriptSDKPath;
			}
		}

		public static bool ExistsOnPath(string fileName)
		{
			if (GetFullPath(fileName) != null)
				return true;
			return false;
		}

		public static string GetPath(string fileName)
		{
			return Path.GetDirectoryName(GetFullPath(fileName));

		}
		public static string GetFullPath(string fileName)
		{
			if (File.Exists(fileName))
				return Path.GetFullPath(fileName);

			var values = Environment.GetEnvironmentVariable("PATH");
			foreach (var path in values.Split(Path.PathSeparator))
			{
				var fullPath = Path.Combine(path, fileName);
				if (File.Exists(fullPath))
					return fullPath;
			}
			return null;
		}
	}
}


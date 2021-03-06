﻿using System;
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

		bool _error = false;
		String _PlayScriptCompiler = "NoSet";
		String _PlayScriptSDKPath = "NoSet";

		public PlayScriptBinPathTask ()
		{
		}

		public override bool Execute ()
		{
			_error = false;
			string psc_name = "psc";
			if (Environment.OSVersion.Platform != PlatformID.Unix) {
				psc_name += ".exe";
			}
			_PlayScriptCompiler = psc_name;

			// Does psc exist on the path?
			string psc = GetFullPath (psc_name);
			if (psc != null) {
				_PlayScriptSDKPath = psc;
			} else {
				// Does psc exist within the XS Addin dir?
				var home = Environment.GetEnvironmentVariable ("HOME");
				// TODO: this needs to be done via CONST and x-plat
				var psc_addin = Path.Combine (home, "Library/Application Support/XamarinStudio-5.0/LocalInstall/Addins");

				var AddinFullName = Directory.GetDirectories (psc_addin, "MonoDevelop.PlayScript.5.*");
				if (AddinFullName.Length == 1) {
					psc_addin = Path.Combine (psc_addin, AddinFullName [0]);
					psc_addin = Path.Combine (psc_addin, "MonoDevelop.PlayScript.SupportPackages");
					var psc_full_path = Path.Combine (psc_addin, psc_name);
					if (File.Exists (psc_full_path)) {
						// TODO: Fixme...Hack file perms on script as Addin installer does not preserve them...
//				system (String.Format ("chmod u+x \"{0}\"", psc));
						_PlayScriptSDKPath = psc_addin;
					}
				} else {
					//Log is null under NUnit tests? under xbuild?
					//Log.LogError ("PlayScriptBinPathTask locator failure: {0}", psc_addin);
					_error = true;
					_PlayScriptSDKPath = "";
				}
			}
			//return !Log.HasLoggedErrors;
			return !_error;
		}

		[Output]
		public String PlayScriptCompiler {
			get {
				return _PlayScriptCompiler;
			}
		}

		[Output]
		public String PlayScriptSDKPath {
			get {
				return _PlayScriptSDKPath;
			}
		}

		public static bool ExistsOnPath (string fileName)
		{
			if (GetFullPath (fileName) != null)
				return true;
			return false;
		}

		public static string GetPath (string fileName)
		{
			return Path.GetDirectoryName (GetFullPath (fileName));

		}

		public static string GetFullPath (string fileName)
		{
			if (File.Exists (fileName))
				return Path.GetFullPath (fileName);

			var values = Environment.GetEnvironmentVariable ("PATH");
			foreach (var path in values.Split(Path.PathSeparator)) {
				var fullPath = Path.Combine (path, fileName);
				if (File.Exists (fullPath))
					return fullPath;
			}
			return null;
		}
	}
}


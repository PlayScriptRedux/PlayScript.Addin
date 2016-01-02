using NUnit.Framework;
using System;
using System.IO;
using System.Diagnostics;

namespace PlayScript.MSBuild.Tasks.Test
{
	[TestFixture ()]
	public class Test
	{
		String _file;
		public Test() {
			if (Environment.OSVersion.Platform == PlatformID.Unix) {
				_file = "ls";
			} else {
				_file = "powershell.exe";
			}
		}
			
		[Test ()]
		public void ExistsOnPath ()
		{
			var found = PlayScriptBinPathTask.ExistsOnPath (_file);
			Assert.IsTrue (found);
		}

		[Test ()]
		public void GetPath ()
		{
			var path = PlayScriptBinPathTask.GetPath (_file);
			Assert.IsTrue (Directory.Exists(path));
		}

		[Test ()]
		public void GetFullPath ()
		{
			var path = PlayScriptBinPathTask.GetFullPath (_file);
			Assert.IsTrue (File.Exists(path));
		}

		[Test ()]
		public void CompilerExists ()
		{
			var playTask = new PlayScriptBinPathTask ();
			Assert.IsNotNull (playTask);
			var found = playTask.Execute ();
			Assert.IsTrue (found);
		}
	}
}


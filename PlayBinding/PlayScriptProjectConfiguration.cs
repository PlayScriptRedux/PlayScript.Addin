﻿using System;
using MonoDevelop.Projects;
using System.Collections.Generic;

namespace CSharpBinding
{
	public class AddinProjectConfiguration : DotNetProjectConfiguration
	{
		public AddinProjectConfiguration ()
		{
		}

		public AddinProjectConfiguration (string name) : base (name)
		{
		}

		public override IEnumerable<string> GetDefineSymbols ()
		{
			foreach (var d in base.GetDefineSymbols ()) {
				yield return d;
			}

			var proj = (AddinProject)ParentItem;

			//TODO: keep in sync with targets. eventually resolve from MSBuild
			var cv = proj.AddinRegistry.GetAddin ("MonoDevelop.Core").Description.CompatVersion;

			yield return "MD_" + cv.Replace ('.', '_');
		}
	}
}


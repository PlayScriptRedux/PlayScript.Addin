using System;
using MonoDevelop.Projects;
using MonoDevelop.Core.Serialization;

namespace CSharpBinding
{
	[DataItem("AddinReference")]
	class AddinReference : ProjectItem
	{
		[ItemProperty ("Include")]
		public string Id { get; set; }

		[ItemProperty]
		public string Version { get; set; }

		public AddinProject OwnerProject { get; internal set; }
	}
}


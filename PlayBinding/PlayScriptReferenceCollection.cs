using System;
using MonoDevelop.Projects;

namespace CSharpBinding
{
	class AddinReferenceCollection : ProjectItemCollection<AddinReference>
	{
		public AddinProject Project { get; private set; }

		internal AddinReferenceCollection (AddinProject project)
		{
			this.Project = project;
		}
	}
}


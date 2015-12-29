// 
// CSharpReferenceFinder.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2011 Xamarin Inc. (http://xamarin.com)
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;

using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.PlayScript.Resolver;
using MonoDevelop.Ide.FindInFiles;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.PlayScript;
using ICSharpCode.NRefactory.PlayScript.Resolver;
using System.IO;
using MonoDevelop.Ide.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using Mono.TextEditor;
using ICSharpCode.NRefactory.PlayScript.TypeSystem;
using System.Threading;

namespace MonoDevelop.PlayScript.Refactoring
{
	using MonoDevelop.Projects;
	class CSharpReferenceFinder : ReferenceFinder
	{
		ICSharpCode.NRefactory.PlayScript.Resolver.FindReferences refFinder = new ICSharpCode.NRefactory.PlayScript.Resolver.FindReferences ();
		List<object> searchedMembers;
		List<FilePath> files = new List<FilePath> ();
		List<Tuple<FilePath, MonoDevelop.Ide.Gui.Document>> openDocuments = new List<Tuple<FilePath, MonoDevelop.Ide.Gui.Document>> ();
		
		string memberName;
		string keywordName;
		
		public CSharpReferenceFinder ()
		{
			IncludeDocumentation = true;
		}
		
		public void SetSearchedMembers (IEnumerable<object> members)
		{
			searchedMembers = new List<object> (members);
			var firstMember = searchedMembers.FirstOrDefault ();
			if (firstMember is INamedElement) {
				var namedElement = (INamedElement)firstMember;
				var name = namedElement.Name;
				if (namedElement is IMethod && (((IMethod)namedElement).IsConstructor | ((IMethod)namedElement).IsDestructor))
					name = ((IMethod)namedElement).DeclaringType.Name;
				memberName = name;

				keywordName = CSharpAmbience.NetToCSharpTypeName (namedElement.FullName);
				if (keywordName == namedElement.FullName)
					keywordName = null;
			}
			if (firstMember is string)
				memberName = firstMember.ToString ();
			if (firstMember is IVariable)
				memberName = ((IVariable)firstMember).Name;
			if (firstMember is ITypeParameter)
				memberName = ((ITypeParameter)firstMember).Name;
			if (firstMember is INamespace)
				memberName = ((INamespace)firstMember).Name;
		}
		
		void SetPossibleFiles (IEnumerable<FilePath> files)
		{
			foreach (var file in files) {
				var openDocument = IdeApp.Workbench.GetDocument (file);
				if (openDocument == null) {
					this.files.Add (file);
				} else {
					this.openDocuments.Add (Tuple.Create (file, openDocument));
				}
			}
		}
		
		MemberReference GetReference (Project project, ResolveResult result, AstNode node, SyntaxTree syntaxTree, string fileName, Mono.TextEditor.TextEditorData editor)
		{
			AstNode originalNode = node;
			if (result == null)
				return null;

			object valid = null;
			if (result is MethodGroupResolveResult) {
				valid = ((MethodGroupResolveResult)result).Methods.FirstOrDefault (
					m => searchedMembers.Any (member => member is IMethod && ((IMethod)member).Region == m.Region));
			} else if (result is MemberResolveResult) {
				var foundMember = ((MemberResolveResult)result).Member;
				valid = searchedMembers.FirstOrDefault (
					member => member is IMember && ((IMember)member).Region == foundMember.Region);
			} else if (result is NamespaceResolveResult) {
				var ns = ((NamespaceResolveResult)result).Namespace;
				valid = searchedMembers.FirstOrDefault (n => n is INamespace && ns.FullName.StartsWith (((INamespace)n).FullName, StringComparison.Ordinal));
				if (!(node is NamespaceDeclaration))
					goto skip;
			} else if (result is LocalResolveResult) {
				var ns = ((LocalResolveResult)result).Variable;
				valid = searchedMembers.FirstOrDefault (n => n is IVariable && ((IVariable)n).Region == ns.Region);
			} else if (result is TypeResolveResult) {
				valid = searchedMembers.FirstOrDefault (n => n is IType);
			}
			if (node is ConstructorInitializer)
				return null;
			if (node is ObjectCreateExpression)
				node = ((ObjectCreateExpression)node).Type;
			if (node is IndexerDeclaration)
				node = ((IndexerDeclaration)node).ThisToken;

			if (node is InvocationExpression)
				node = ((InvocationExpression)node).Target;
			
			if (node is MemberReferenceExpression)
				node = ((MemberReferenceExpression)node).MemberNameToken;
			
			if (node is SimpleType)
				node = ((SimpleType)node).IdentifierToken;

			if (node is MemberType)
				node = ((MemberType)node).MemberNameToken;
			
			if (node is NamespaceDeclaration) {
				var nsd = ((NamespaceDeclaration)node);
				node = nsd.NamespaceName;
				if (node == null)
					return null;
			}

			if (node is TypeDeclaration && (searchedMembers.First () is IType)) 
				node = ((TypeDeclaration)node).NameToken;
			if (node is DelegateDeclaration) 
				node = ((DelegateDeclaration)node).NameToken;

			if (node is EntityDeclaration && (searchedMembers.First () is IMember)) 
				node = ((EntityDeclaration)node).NameToken;
			
			if (node is ParameterDeclaration && (searchedMembers.First () is IParameter)) 
				node = ((ParameterDeclaration)node).NameToken;
			if (node is ConstructorDeclaration)
				node = ((ConstructorDeclaration)node).NameToken;
			if (node is DestructorDeclaration)
				node = ((DestructorDeclaration)node).NameToken;
			if (node is NamedArgumentExpression)
				node = ((NamedArgumentExpression)node).NameToken;
			if (node is NamedExpression)
				node = ((NamedExpression)node).NameToken;
			if (node is VariableInitializer)
				node = ((VariableInitializer)node).NameToken;

			if (node is IdentifierExpression) {
				node = ((IdentifierExpression)node).IdentifierToken;
			}

		skip:

			var region = new DomRegion (fileName, node.StartLocation, node.EndLocation);

			var length = node is PrimitiveType ? keywordName.Length : node.EndLocation.Column - node.StartLocation.Column;
			if (valid == null)
				valid = searchedMembers.FirstOrDefault ();
			var reference = new CSharpMemberReference (project, originalNode, syntaxTree, valid, region, editor.LocationToOffset (region.Begin), length);

			reference.ReferenceUsageType = GetUsage (originalNode);
			return reference;
		}

		// same logic than the extract method analyzation, unfortunately it's not reusable in this context 
		// we need to do it bottom up here.
		ReferenceUsageType GetUsage (AstNode node)
		{
			if (node.Parent is UnaryOperatorExpression) {
				var unaryOperatorExpression = (UnaryOperatorExpression)node.Parent;
				if (unaryOperatorExpression.Operator == UnaryOperatorType.Increment || 
				    unaryOperatorExpression.Operator == UnaryOperatorType.Decrement ||
					unaryOperatorExpression.Operator == UnaryOperatorType.PostIncrement || 
				    unaryOperatorExpression.Operator == UnaryOperatorType.PostDecrement) {
					return ReferenceUsageType.ReadWrite;
				}
			} else if (node.Parent is DirectionExpression) {
				var de = (DirectionExpression)node.Parent;
				if (de.FieldDirection == FieldDirection.Ref)
					return ReferenceUsageType.ReadWrite;
				if (de.FieldDirection == FieldDirection.Out)
					return ReferenceUsageType.Write;
			} else if (node.Parent is AssignmentExpression) {
				var ae = (AssignmentExpression)node.Parent;
				if (ae.Left == node)
					return ReferenceUsageType.Write;
			} else if (node is VariableInitializer) {
				return ReferenceUsageType.Write;
			} else if (node is ParameterDeclaration) {
				return ReferenceUsageType.Write;
			} else if (node.Parent is ForeachStatement) {
				if (node.Role == Roles.Identifier)
					return ReferenceUsageType.Write;
			}
			return ReferenceUsageType.Read;
		}

		public class CSharpMemberReference : MemberReference
		{
			public SyntaxTree SyntaxTree {
				get;
				private set;
			}

			public AstNode AstNode {
				get;
				private set;
			}

			public Project Project {
				get;
				private set;
			}

			public CSharpMemberReference (Project project, AstNode astNode, SyntaxTree syntaxTree,  object entity, DomRegion region, int offset, int length) : base (entity, region, offset, length)
			{
				this.Project = project;
				this.AstNode = astNode;
				this.SyntaxTree = syntaxTree;
			}
		}

		bool IsNodeValid (object searchedMember, AstNode node)
		{
			if (searchedMember is IField && node is FieldDeclaration)
				return false;
			return true;
		}
		
		public IEnumerable<MemberReference> FindInDocument (MonoDevelop.Ide.Gui.Document doc, CancellationToken token = default (CancellationToken))
		{
			if (string.IsNullOrEmpty (memberName))
				return Enumerable.Empty<MemberReference> ();
			var editor = doc.Editor;
			var parsedDocument = doc.ParsedDocument;
			if (parsedDocument == null)
				return Enumerable.Empty<MemberReference> ();
			var unit = parsedDocument.GetAst<SyntaxTree> ();
			var file = parsedDocument.ParsedFile as CSharpUnresolvedFile;
			var result = new List<MemberReference> ();
			
			foreach (var obj in searchedMembers) {
				if (obj is IVariable && !(obj is IParameter) && !(obj is IField)) {
					refFinder.FindLocalReferences ((IVariable)obj, file, unit, doc.Compilation, (astNode, r) => { 
						if (IsNodeValid (obj, astNode))
							result.Add (GetReference (doc.Project, r, astNode, unit, editor.FileName, editor));
					}, token);
				} else if (obj is ISymbol) {
					var sym = (ISymbol)obj;

					// May happen for anonymous types since empty constructors are always generated.
					// But there is no declaring type definition for them - we filter out this case.
					if (sym.SymbolKind == SymbolKind.Constructor && ((IEntity)sym).DeclaringTypeDefinition == null)
						continue;
					refFinder.FindReferencesInFile (refFinder.GetSearchScopes (sym), file, unit, doc.Compilation, (astNode, r) => {
						if (IsNodeValid (obj, astNode))
							result.Add (GetReference (doc.Project, r, astNode, unit, editor.FileName, editor)); 
					}, token);
				}
			}
			return result;
		}
		
		public override IEnumerable<MemberReference> FindReferences (Project project, IProjectContent content, IEnumerable<FilePath> possibleFiles, IProgressMonitor monitor, IEnumerable<object> members)
		{
			if (content == null)
				throw new ArgumentNullException ("content", "Project content not set.");
			SetPossibleFiles (possibleFiles);
			SetSearchedMembers (members);
			var scopes = searchedMembers.Select (e => e is INamespace ? refFinder.GetSearchScopes ((INamespace)e) : refFinder.GetSearchScopes ((ISymbol)e));
			var compilation = project != null ? TypeSystemService.GetCompilation (project) : content.CreateCompilation ();
			List<MemberReference> refs = new List<MemberReference> ();
			foreach (var opendoc in openDocuments) {
				foreach (var newRef in FindInDocument (opendoc.Item2)) {
					if (newRef == null || refs.Any (r => r.FileName == newRef.FileName && r.Region == newRef.Region))
						continue;
					refs.Add (newRef);
				}
			}
			foreach (var file in files) {
				if (monitor != null)
					monitor.Step (1);
				string text;
				try {
					if  (!File.Exists (file))
						continue;
					text = Mono.TextEditor.Utils.TextFileUtility.ReadAllText (file);
				} catch (Exception e) {
					LoggingService.LogError ("Exception while file reading.", e);
					continue;
				}
				if (memberName != null && text.IndexOf (memberName, StringComparison.Ordinal) < 0 &&
					(keywordName == null || text.IndexOf (keywordName, StringComparison.Ordinal) < 0))
					continue;
				using (var editor = TextEditorData.CreateImmutable (text)) {
					editor.Document.FileName = file;
					var unit = new PlayScriptParser ().Parse (editor);
					if (unit == null)
						continue;
					
					var storedFile = content.GetFile (file);
					var parsedFile = storedFile as CSharpUnresolvedFile;
					
					if (parsedFile == null && storedFile is ParsedDocumentDecorator) {
						parsedFile = ((ParsedDocumentDecorator)storedFile).ParsedFile as CSharpUnresolvedFile;
					}
					
					if (parsedFile == null) {
						// for fallback purposes - should never happen.
						parsedFile = unit.ToTypeSystem ();
						content = content.AddOrUpdateFiles (parsedFile);
						compilation = content.CreateCompilation ();
					}
					foreach (var scope in scopes) {
						refFinder.FindReferencesInFile (
							scope,
							parsedFile,
							unit,
							compilation,
							(astNode, result) => {
								var newRef = GetReference (project, result, astNode, unit, file, editor);
								if (newRef == null || refs.Any (r => r.FileName == newRef.FileName && r.Region == newRef.Region))
									return;
								refs.Add (newRef);
							},
							CancellationToken.None
						);
					}
				}
			}
			return refs;
		}
	}
}


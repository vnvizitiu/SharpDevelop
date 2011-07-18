﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakeSelectedProject : IPackageManagementSelectedProject
	{
		public FakePackageManagementProject FakeProject = new FakePackageManagementProject();
		
		public FakeSelectedProject()
		{
		}
		
		public FakeSelectedProject(string name)
			: this(name, false)
		{
		}
		
		public FakeSelectedProject(string name, bool selected)
		{
			this.Name = name;
			this.IsSelected = selected;
		}
		
		public string Name { get; set; }
		public bool IsSelected { get; set; }
		
		public override string ToString()
		{
			return String.Format("Name: {0}, IsSelected: {1}", Name, IsSelected);
		}
		
		public FakeInstallPackageAction FakeInstallPackageAction {
			get { return FakeProject.FakeInstallPackageAction; }
			set { FakeProject.FakeInstallPackageAction = value; }
		}
		
		public IPackageManagementProject Project {
			get { return FakeProject; }
		}
		
		public FakePackageOperation AddFakeInstallPackageOperation()
		{
			return FakeProject.AddFakeInstallOperation();
		}
		
		public FakePackageOperation AddFakeUninstallPackageOperation()
		{
			return FakeProject.AddFakeUninstallOperation();
		}
	}
}
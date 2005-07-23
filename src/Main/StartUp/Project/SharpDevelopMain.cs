﻿// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Resources;
using System.Xml;
using System.Threading;
using System.Runtime.Remoting;
using System.Security.Policy;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This Class is the Core main class, it starts the program.
	/// </summary>
	public class SharpDevelopMain
	{
		static string[] commandLineArgs = null;
		
		public static string[] CommandLineArgs {
			get {
				return commandLineArgs;
			}
		}
		
		static void ShowErrorBox(object sender, ThreadExceptionEventArgs eargs)
		{
			ShowErrorBox(eargs.Exception, null);
		}
		
		static void ShowErrorBox(Exception exception, string message)
		{
			using (ExceptionBox box = new ExceptionBox(exception, message)) {
				DialogResult result = box.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
				
				if (result == DialogResult.Abort) {
					Application.Exit();
				}
			}
		}
		
		readonly static string[] assemblyList = {
			"Microsoft.VisualBasic.dll",
			"Microsoft.JScript.dll",
			"mscorlib.dll",
			"System.Data.dll",
			"System.Design.dll",
			"System.DirectoryServices.dll",
			"System.Drawing.Design.dll",
			"System.Drawing.dll",
			"System.EnterpriseServices.dll",
			"System.Management.dll",
			"System.Messaging.dll",
			"System.Runtime.Remoting.dll",
			"System.Runtime.Serialization.Formatters.Soap.dll",
			
			"System.Security.dll",
			"System.ServiceProcess.dll",
			"System.Web.Services.dll",
			"System.Web.dll",
			"System.Windows.Forms.dll",
			"System.dll",
			"System.XML.dll"
		};
		
		/// <summary>
		/// Starts the core of SharpDevelop.
		/// </summary>
		[STAThread()]
		public static void Main(string[] args)
		{
			if (Debugger.IsAttached) {
				Run(args);
			} else {
				try {
					Run(args);
				} catch (Exception ex) {
					Console.WriteLine(ex);
					try {
						Application.Run(new ExceptionBox(ex, "Unhandled exception terminated SharpDevelop"));
					} catch {
						MessageBox.Show(ex.ToString(), "Critical error (cannot use ExceptionBox)");
					}
				}
			}
		}
		
		static void Run(string[] args)
		{
			#if DEBUG
			Control.CheckForIllegalCrossThreadCalls = true;
			#endif
			commandLineArgs = args;
			bool noLogo = false;
			
			SplashScreenForm.SetCommandLineArgs(args);
			
			foreach (string parameter in SplashScreenForm.GetParameterList()) {
				switch (parameter.ToUpper()) {
					case "NOLOGO":
						noLogo = true;
						break;
				}
			}
			
			if (!noLogo) {
				SplashScreenForm.SplashScreen.Show();
			}
			try {
				RunApplication();
			} finally {
				if (SplashScreenForm.SplashScreen != null) {
					SplashScreenForm.SplashScreen.Dispose();
				}
			}
		}
		
		static void RunApplication()
		{
			if (!Debugger.IsAttached) {
				Application.ThreadException += ShowErrorBox;
			}
			#if !DEBUG
			MessageService.CustomErrorReporter = ShowErrorBox;
			#endif
			
//	TODO:
//			bool ignoreDefaultPath = false;
//			string [] addInDirs = ICSharpCode.SharpDevelop.AddInSettingsHandler.GetAddInDirectories(out ignoreDefaultPath);
//			SetAddInDirectories(addInDirs, ignoreDefaultPath);
			
			AddInTree.Auswerter.Add("ActiveContentExtension", new ActiveContentExtensionAuswerter());
			AddInTree.Auswerter.Add("ActiveViewContentUntitled", new ActiveViewContentUntitledAuswerter());
			AddInTree.Auswerter.Add("ActiveWindowState", new ActiveWindowStateAuswerter());
			AddInTree.Auswerter.Add("CombineOpen", new CombineOpenAuswerter());
			AddInTree.Auswerter.Add("DebuggerSupports", new DebuggerSupportsAuswerter());
			AddInTree.Auswerter.Add("IsProcessRunning", new IsProcessRunningAuswerter());
			AddInTree.Auswerter.Add("OpenWindowState", new OpenWindowStateAuswerter());
			AddInTree.Auswerter.Add("WindowActive", new WindowActiveAuswerter());
			AddInTree.Auswerter.Add("WindowOpen", new WindowOpenAuswerter());
			AddInTree.Auswerter.Add("ProjectActive", new ProjectActiveAuswerter());
			AddInTree.Auswerter.Add("ProjectOpen", new ProjectOpenAuswerter());
			AddInTree.Auswerter.Add("TextContent", new ICSharpCode.SharpDevelop.DefaultEditor.Conditions.TextContentAuswerter());
			
			AddInTree.Erbauer.Add("DialogPanel", new DialogPanelErbauer());
			AddInTree.Erbauer.Add("DisplayBinding", new DisplayBindingErbauer());
			AddInTree.Erbauer.Add("Pad", new PadErbauer());
			AddInTree.Erbauer.Add("LanguageBinding", new LanguageBindingErbauer());
			AddInTree.Erbauer.Add("Parser", new ParserErbauer());
			AddInTree.Erbauer.Add("EditAction", new ICSharpCode.SharpDevelop.DefaultEditor.Codons.EditActionErbauer());
			AddInTree.Erbauer.Add("SyntaxMode", new ICSharpCode.SharpDevelop.DefaultEditor.Codons.SyntaxModeErbauer());
			AddInTree.Erbauer.Add("BrowserSchemeExtension", new ICSharpCode.SharpDevelop.BrowserDisplayBinding.SchemeExtensionErbauer());
			
			PropertyService.Load();
			
			StringParser.RegisterStringTagProvider(new ICSharpCode.SharpDevelop.Commands.SharpDevelopStringTagProvider());
			
			AddInTree.Load();
			
			// .NET base autostarts
			// taken out of the add-in tree for performance reasons (every tick in startup counts)
			new ICSharpCode.SharpDevelop.Commands.InitializeWorkbenchCommand().Run();
			
			// run workspace autostart commands
			try {
				ArrayList commands = AddInTree.GetTreeNode("/Workspace/Autostart").BuildChildItems(null);
				foreach (ICommand command in commands) {
					command.Run();
				}
			} catch (TreePathNotFoundException) {
				// Do nothing.
			} catch (XmlException e) {
				MessageBox.Show("Could not load XML :" + Environment.NewLine + e.Message);
				return;
			} catch (Exception e) {
				MessageBox.Show("Loading error, please reinstall :"  + Environment.NewLine + e.ToString());
				return;
			} finally {
				if (SplashScreenForm.SplashScreen != null) {
					SplashScreenForm.SplashScreen.Dispose();
				}
			}
			
			// finally start the workbench.
			try {
				new ICSharpCode.SharpDevelop.Commands.StartWorkbenchCommand().Run();
			} finally {
				// unloading
				ProjectService.CloseSolution();
				FileService.Unload();
				PropertyService.Save();
			}
		}
	}
}

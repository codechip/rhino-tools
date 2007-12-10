import file from "assembly://Binsor.Presentation.Framework/DefaultConfig.boo"
import namespaces from "assembly://Binsor.Presentation.Framework/DefaultImport.boo"

assemblies = ["Blog.Shell"]
assemblies.Extend(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Blog.*.dll"))
	
InitializeContainer( assemblies )
PrintRegisteredComponents()

extend 'ShellModuleLoader':
	menuItemDatas = (
			MenuItemData(Header: "E_xit", Parent: "File", CommandName: "ExitCommand"),
			MenuItemData(Header: "H_elp", Parent: "MainMenu", Name: "Help"),
			MenuItemData(Header: "A_bout", Parent: "Help", Command: ApplicationCommands.Help),
	)
	
component 'Header', SingleViewPanelDecoratingLayout
component 'MainContent', SingleViewPanelDecoratingLayout
component 'Footer', SingleViewPanelDecoratingLayout

extend 'DefaultLayoutSelector':
	acceptableViewNames = {
		'MainContent' : ("ContentView",)
	}
import file from "assembly://IoC.UI/DefaultConfig.boo"
import namespaces from "assembly://IoC.UI/DefaultImport.boo"

InitializeContainer("Blog.Viewer.Shell")
PrintRegisteredComponents()

extend 'Blog.Viewer.Shell.ShellModuleLoader':
	menuItemDatas = (
			MenuItemData(Text: "E&xit", Parent: "File", Shortcut: "Alt, F4", Command: "File_Exit"),
			MenuItemData(Text: "H&elp", Parent: "MainMenu", Name: "Help"),
			MenuItemData(Text: "A&bout", Parent: "Help", Shortcut: "F1", Command: "About"),
	)
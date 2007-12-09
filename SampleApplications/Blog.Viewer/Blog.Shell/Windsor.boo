import file from "assembly://IoC.UI/DefaultConfig.boo"
import namespaces from "assembly://IoC.UI/DefaultImport.boo"

InitializeContainer("Blog.Shell")
PrintRegisteredComponents()

extend 'ShellModuleLoader':
	menuItemDatas = (
			MenuItemData(Header: "E_xit", Parent: "File", CommandName: "ExitCommand"),
			MenuItemData(Header: "H_elp", Parent: "MainMenu", Name: "Help"),
			MenuItemData(Header: "A_bout", Parent: "Help", Command: ApplicationCommands.Help),
	)
import file from "assembly://IoC.UI/DefaultConfig.boo"
import namespaces from "assembly://IoC.UI/DefaultImport.boo"

InitializeContainer("IoC.UI.Tests")
PrintRegisteredComponents()

extend 'DemoModuleLoader':
	items = (
			MenuItemData(Header: "E&xit", Parent: "File", CommandName: "File_Exit"),
			MenuItemData(Header: "A&bout", Parent: "Help", CommandName: "Help_About" ),
			MenuItemData(Header: "H&elp", Name: "Help"),
			MenuItemData(Header: "T&ools", Name: "Tools"),
	)
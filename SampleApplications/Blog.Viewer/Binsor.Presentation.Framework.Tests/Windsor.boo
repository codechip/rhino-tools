import file from "assembly://Binsor.Presentation.Framework/DefaultConfig.boo"
import namespaces from "assembly://Binsor.Presentation.Framework/DefaultImport.boo"

InitializeContainer("Binsor.Presentation.Framework.Tests")
PrintRegisteredComponents()

extend 'DemoModuleLoader':
	items = (
			MenuItemData(Header: "E&xit", Parent: "File", CommandName: "File_Exit"),
			MenuItemData(Header: "A&bout", Parent: "Help", CommandName: "Help_About" ),
			MenuItemData(Header: "H&elp", Name: "Help"),
			MenuItemData(Header: "T&ools", Name: "Tools"),
	)
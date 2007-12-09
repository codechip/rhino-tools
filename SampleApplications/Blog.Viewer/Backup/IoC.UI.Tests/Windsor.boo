import file from "assembly://IoC.UI/DefaultConfig.boo"
import namespaces from "assembly://IoC.UI/DefaultImport.boo"

InitializeContainer("IoC.UI.Tests")
PrintRegisteredComponents()

extend 'IoC.UI.Tests.Demo.DemoModuleLoader':
	items = (
			MenuItemData(Text: "E&xit", Parent: "File", Shortcut: "Alt, F4", Command: "File_Exit"),
			MenuItemData(Text: "A&bout", Parent: "Help", Shortcut: "F1", Command: "Help_About"),
			MenuItemData(Text: "H&elp", Name: "Help"),
			MenuItemData(Text: "T&ools", Name: "Tools", Shortcut: ""),
	)
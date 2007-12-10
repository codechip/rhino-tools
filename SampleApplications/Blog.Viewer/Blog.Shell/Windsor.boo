import file from "assembly://Binsor.Presentation.Framework/DefaultConfig.boo"
import namespaces from "assembly://Binsor.Presentation.Framework/DefaultImport.boo"

InitializeContainer("Blog.Shell", "Blog.Header", "Blog.MainContent", "Blog.Footer")
PrintRegisteredComponents()

extend 'ShellModuleLoader':
	menuItemDatas = (
			MenuItemData(Header: "E_xit", Parent: "File", CommandName: "ExitCommand"),
			MenuItemData(Header: "H_elp", Parent: "MainMenu", Name: "Help"),
			MenuItemData(Header: "A_bout", Parent: "Help", Command: ApplicationCommands.Help),
	)
	
component 'Header', SingleViewPanelDecoratingLayout:
	acceptableViewNames = ( "Header", )
	
component 'MainContent', SingleViewPanelDecoratingLayout:
	acceptableViewNames = ( "MainContent", )
	
component 'Footer', SingleViewPanelDecoratingLayout:
	acceptableViewNames = ( "Footer", )
using System;
using System.Windows.Forms;

namespace Netron.Lithium.Shapes
{
	
	public class ListViewGraphControl : GraphControl
	{
		public ListView Properties
		{
			get { return (ListView)Control; }
			set { Control = value; }
		}

		public ListViewGraphControl()
		{
			Control = new ListView();
		}
	}

	public class TextBoxGraphControl : GraphControl
	{
		public TextBox Properties
		{
			get { return (TextBox) Control; }
			set { Control = value; }
		}

		public TextBoxGraphControl()
		{
			Control = new TextBox();
		}
	}

	public class DateTimePickerGraphControl : GraphControl
	{
		public DateTimePicker Properties
		{
			get { return (DateTimePicker) Control; }
			set { Control = value; }
		}

		public DateTimePickerGraphControl()
		{
			Control = new DateTimePicker();
		}
	}


public class GroupBoxGraphControl : GraphControl
{
	public GroupBox Properties
	{
		get { return (GroupBox) Control; }
		set { Control = value; }
	}

	public GroupBoxGraphControl()
	{
		Control = new GroupBox();
	}
}

	public class LabelGraphControl : GraphControl
	{
		public Label Properties
		{
			get { return (Label) Control; }
			set { Control = value; }
		}

		public LabelGraphControl()
		{
			Control = new Label();
		}
	}

	public class MonthCalendarGraphControl : GraphControl
	{
		public MonthCalendar Properties
		{
			get { return (MonthCalendar) Control; }
			set { Control = value; }
		}

		public MonthCalendarGraphControl()
		{
			Control = new MonthCalendar();
		}
	}

	public class PictureBoxGraphControl : GraphControl
	{
		public PictureBox Properties
		{
			get { return (PictureBox) Control; }
			set { Control = value; }
		}

		public PictureBoxGraphControl()
		{
			Control = new PictureBox();
		}
	}


	public class ProgressBarGraphControl : GraphControl
	{
		public ProgressBar Properties
		{
			get { return (ProgressBar) Control; }
			set { Control = value; }
		}

		public ProgressBarGraphControl()
		{
			Control = new ProgressBar();
		}
	}

	public class TabControlGraphControl : GraphControl
	{
		public TabControl Properties
		{
			get { return (TabControl) Control; }
			set { Control = value; }
		}

		public TabControlGraphControl()
		{
			Control = new TabControl();
		}
	}


	public class TreeViewGraphControl : GraphControl
	{
		public TreeView Properties
		{
			get { return (TreeView) Control; }
			set { Control = value; }
		}

		public TreeViewGraphControl()
		{
			Control = new TreeView();
		}
	}
}
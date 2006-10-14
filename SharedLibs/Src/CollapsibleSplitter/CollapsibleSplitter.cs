/*

	Windows Forms Collapsible Splitter Control for .Net
	(c)Copyright 2002 Furty (furty74@yahoo.com). All rights reserved.
	Free for any use, so long as copyright is acknowledged.
	
	This is my second attempt at giving back to the codeproject.com community
	that has given me so much in my few months as a C# programmer, I hope you find it useful.
	If you improve this control, please email me the updated source, and if you have any 
	comments or suggestions, please post your thoughts in the feedback section on the 
	codeproject.com page for this control.
	
	Version 1.1 Changes:
	OnPaint is now overridden instead of being a handled event, and the entire splitter is now painted rather than just the collpaser control
	The splitter rectangle is now correctly defined
	The Collapsed property was renamed to IsCollapsed, and the code changed so that no value needs to be set
	New visual styles added: Win9x, XP, DoubleDots and Lines
	
	Version 1.11 Changes:
	The OnMouseMove event handler was updated to address a flickering issue discovered by John O'Byrne
	
	Version 1.2 Changes:
	Added support for horizontal splitters
	
*/
namespace Furty.Windows.Forms
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Runtime.Remoting;
	using System.Windows.Forms;
    
    
	public class CollapsibleSplitter : System.Windows.Forms.Splitter
	{
		#region Declarations

		// declare and define some base variables
		private bool hot;
		private Color hotColor = CalculateColor(SystemColors.Highlight, SystemColors.Window, 70);
		private Control controlToHide;
		private Rectangle rr;
		private System.Windows.Forms.Form parentForm;
		private bool expandParentForm;
		private VisualStyles visualStyle = VisualStyles.Mozilla;

		public enum VisualStyles
		{
			Mozilla = 0,
			XP,
			Win9x,
			DoubleDots,
			Lines
		}

		#endregion

		#region Properties

		[Bindable(true), Category("Collapsing Options"), DefaultValue("False"),
		Description("The initial state of the Splitter. Set to True if the control to hide is not visible by default")]
		public bool IsCollapsed
		{
			get
			{ 
				if(controlToHide != null)
					return !controlToHide.Visible; 
				else
					return true;
			}
		}

		[Bindable(true), Category("Collapsing Options"), DefaultValue(""),
		Description("The form control that the splitter will collapse (Visible = false)")]
		public Control ControlToHide
		{
			get{ return controlToHide; }
			set{ controlToHide = value; }
		}

		[Bindable(true), Category("Collapsing Options"), DefaultValue("False"),
		Description("The parent form hosting the control. Only needed if the ExpandParentForm property is set to True")]
		public System.Windows.Forms.Form ParentForm
		{
			get{ return parentForm; }
			set{ parentForm = value; }
		}

		[Bindable(true), Category("Collapsing Options"), DefaultValue("False"),
		Description("When true the entire parent form will be expanded and collapsed, otherwise just the contol to expand will be changed")]
		public bool ExpandParentForm
		{
			get{ return expandParentForm; }
			set{ expandParentForm = value; }
		}

		[Bindable(true), Category("Collapsing Options"), DefaultValue(""),
		Description("The visual style that will be painted on the control")]
		public VisualStyles VisualStyle
		{
			get{ return visualStyle; }
			set{ visualStyle = value; }
		}

		#endregion

		#region New Events

		public void ToggleState()
		{
			ToggleSplitter();
		}

		#endregion
        
		#region Constructor

		public CollapsibleSplitter()
		{
			this.Click += new System.EventHandler(OnClick);
			this.Resize += new System.EventHandler(OnResize);
			this.MouseLeave += new System.EventHandler(OnMouseLeave);
			this.MouseMove += new MouseEventHandler(OnMouseMove);
		}

		#endregion
  
		#region Event Handlers

		protected override void OnMouseDown(MouseEventArgs e)
		{
			// if the hider control isn't hot, let the base resize action occur
			if(!hot && controlToHide.Visible)
				base.OnMouseDown(e);
		}

		private void OnResize(object sender, System.EventArgs e)
		{
			this.Refresh();
		}

		// this method was updated in version 1.11 to fix a flickering problem
		// discovered by John O'Byrne
		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// check to see if the mouse cursor position is within the bounds of our control
			if(e.X >= rr.X && e.X <= rr.X + rr.Width && e.Y >= rr.Y && e.Y <= rr.Y + rr.Height)
			{
				if(!hot)
				{
					hot = true;
					this.Cursor = Cursors.Hand;
					this.Refresh();
				}
			}
			else
			{
				if(hot)
				{
					hot = false;
					this.Refresh();
				}
				if(!controlToHide.Visible)
					this.Cursor = Cursors.Default;
				else // Changed in v1.2 to support Horizontal Splitters
				{
					if(this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
						this.Cursor = Cursors.VSplit;
					else
						this.Cursor = Cursors.HSplit;
				}
			}
		}

		private void OnMouseLeave(object sender, System.EventArgs e)
		{
			// ensure that the hot state is removed
			hot = false;
			this.Refresh();
		}

		private void OnClick(object sender, System.EventArgs e)
		{
			if(controlToHide != null && hot)
				ToggleSplitter();
		}

		private void ToggleSplitter()
		{
			controlToHide.Visible = !controlToHide.Visible;

			// if ExpandParent has been selected, expand or contract the parent form
			// Updated in v1.2 to support Horizontal Splitters
			if(expandParentForm && parentForm != null)
			{
				if(!controlToHide.Visible && parentForm.WindowState != FormWindowState.Maximized)
				{
					// only contract the parent form if it is not maximized
					if(this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
						parentForm.Width -= controlToHide.Width;
					else
						parentForm.Height -= controlToHide.Height;
				}
				else if(controlToHide.Visible && parentForm.WindowState != FormWindowState.Maximized)
				{
					// only expand the parent form if it is not maximized
					if(this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
						parentForm.Width += controlToHide.Width;
					else
						parentForm.Height += controlToHide.Height;
				}
			}
			this.Refresh();	
		}

		#endregion

		#region Paint the control

		// OnPaint is now an override rather than an event in version 1.1
		protected override void OnPaint(PaintEventArgs e)
		{
			// create a Graphics object
			Graphics g = e.Graphics;
			
			// find the rectangle for the splitter and paint it
			Rectangle r = this.ClientRectangle; // fixed in version 1.1
			g.FillRectangle(new SolidBrush(this.BackColor), r);

			#region Vertical Splitter
			// Check the docking style and create the control rectangle accordingly
			if(this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
			{
				// create a new rectangle in the vertical center of the splitter for our collapse control button
				rr = new Rectangle(r.X, (int)r.Y + ((r.Height - 115)/2), 8, 115);
				// force the width to 8px so that everything always draws correctly
				this.Width = 8;

				// draw the background color for our control image
				if(hot)
					g.FillRectangle(new SolidBrush(hotColor), new Rectangle(rr.X + 1, rr.Y, 6, 115));
				else
					g.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(rr.X + 1, rr.Y, 6, 115));

				// draw the top & bottom lines for our control image
				g.DrawLine(new Pen(SystemColors.ControlDark, 1), rr.X + 1, rr.Y, rr.X + rr.Width - 2, rr.Y);
				g.DrawLine(new Pen(SystemColors.ControlDark, 1), rr.X + 1, rr.Y + rr.Height, rr.X + rr.Width - 2, rr.Y + rr.Height);

				// draw the arrows for our control image
				// the ArrowPointArray is a point array that defines an arrow shaped polygon
				g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), ArrowPointArray(rr.X + 2, rr.Y + 3));
				g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), ArrowPointArray(rr.X + 2, rr.Y + rr.Height - 9));

				// draw the dots for our control image using a loop
				int x = rr.X + 3;
				int y = rr.Y + 14;

				// Visual Styles added in version 1.1
				switch(visualStyle)
				{
					case VisualStyles.Mozilla:
						for(int i=0; i < 30; i++)
						{
							// light dot
							g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y + (i*3), x+1, y + 1 + (i*3));
							// dark dot
							g.DrawLine(new Pen(SystemColors.ControlDarkDark), x+1, y + 1 + (i*3), x+2, y + 2 + (i*3));
							// overdraw the background color as we actually drew 2px diagonal lines, not just dots
							if(hot)
								g.DrawLine(new Pen(hotColor), x+2, y + 1 + (i*3), x+2, y + 2 + (i*3));
							else
								g.DrawLine(new Pen(this.BackColor), x+2, y + 1 + (i*3), x+2, y + 2 + (i*3));
						}
						break;

					case VisualStyles.DoubleDots:
						for(int i=0; i < 30; i++)
						{
							// light dot
							g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x, y + 1 + (i*3), 1, 1 );
							// dark dot
							g.DrawRectangle(new Pen(SystemColors.ControlDark), x - 1, y +(i*3), 1, 1 );
							i++;
							// light dot
							g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 2, y + 1 + (i*3), 1, 1 );
							// dark dot
							g.DrawRectangle(new Pen(SystemColors.ControlDark), x + 1, y  + (i*3), 1, 1 );
						}
						break;

					case VisualStyles.Win9x:
						g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x + 2, y);
						g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x,y + 90);
						g.DrawLine(new Pen(SystemColors.ControlDark), x + 2, y, x + 2, y + 90);
						g.DrawLine(new Pen(SystemColors.ControlDark), x, y + 90, x + 2, y + 90);
						break;

					case VisualStyles.XP:
						for(int i=0; i < 18; i++)
						{
							// light dot
							g.DrawRectangle(new Pen(SystemColors.ControlLight), x, y + (i*5), 2, 2 );
							// light light dot
							g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1, y + 1 + (i*5), 1, 1 );
							// dark dark dot
							g.DrawRectangle(new Pen(SystemColors.ControlDarkDark), x, y +(i*5), 1, 1 );
							// dark fill
							g.DrawLine(new Pen(SystemColors.ControlDark), x, y + (i*5), x, y + (i*5) + 1);
							g.DrawLine(new Pen(SystemColors.ControlDark), x, y + (i*5), x + 1, y + (i*5));
						}
						break;

					case VisualStyles.Lines:
						for(int i=0; i < 44; i++)
						{
							g.DrawLine(new Pen(SystemColors.ControlDark), x, y + (i*2), x + 2, y + (i*2));
						}

						break;
				}
			}

			#endregion

			// Horizontal Splitter support added in v1.2

			#region Horizontal Splitter

			else if (this.Dock == DockStyle.Top || this.Dock == DockStyle.Bottom)
			{
				// create a new rectangle in the horizontal center of the splitter for our collapse control button
				rr = new Rectangle((int)r.X + ((r.Width - 115)/2), r.Y, 115, 8);
				// force the height to 8px
				this.Height = 8;

				// draw the background color for our control image
				if(hot)
					g.FillRectangle(new SolidBrush(hotColor), new Rectangle(rr.X, rr.Y + 1, 115, 6));
				else
					g.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(rr.X, rr.Y + 1, 115, 6));

				// draw the left & right lines for our control image
				g.DrawLine(new Pen(SystemColors.ControlDark, 1), rr.X, rr.Y + 1, rr.X, rr.Y + rr.Height - 2);
				g.DrawLine(new Pen(SystemColors.ControlDark, 1), rr.X + rr.Width, rr.Y + 1, rr.X + rr.Width, rr.Y + rr.Height - 2);

				// draw the arrows for our control image
				// the ArrowPointArray is a point array that defines an arrow shaped polygon
				g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), ArrowPointArray(rr.X + 3, rr.Y + 2));
				g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), ArrowPointArray(rr.X +  rr.Width - 9, rr.Y + 2));

				// draw the dots for our control image using a loop
				int x = rr.X + 14;
				int y = rr.Y + 3;

				// Visual Styles added in version 1.1
				switch(visualStyle)
				{
					case VisualStyles.Mozilla:
						for(int i=0; i < 30; i++)
						{
							// light dot
							g.DrawLine(new Pen(SystemColors.ControlLightLight), x + (i*3), y, x + 1 + (i*3), y + 1);
							// dark dot
							g.DrawLine(new Pen(SystemColors.ControlDarkDark), x + 1 + (i*3), y + 1, x + 2 + (i*3), y + 2);
							// overdraw the background color as we actually drew 2px diagonal lines, not just dots
							if(hot)
								g.DrawLine(new Pen(hotColor), x + 1 + (i*3), y + 2, x + 2 + (i*3), y + 2);
							else
								g.DrawLine(new Pen(this.BackColor),  x + 1 + (i*3), y + 2, x + 2 + (i*3), y + 2);
						}
						break;

					case VisualStyles.DoubleDots:
						for(int i=0; i < 30; i++)
						{
							// light dot
							g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1 + (i*3), y, 1, 1 );
							// dark dot
							g.DrawRectangle(new Pen(SystemColors.ControlDark), x + (i*3), y - 1, 1, 1 );
							i++;
							// light dot
							g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1 + (i*3), y + 2, 1, 1 );
							// dark dot
							g.DrawRectangle(new Pen(SystemColors.ControlDark), x + (i*3), y + 1, 1, 1 );
						}
						break;

					case VisualStyles.Win9x:
						g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x, y + 2);
						g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x + 88, y);
						g.DrawLine(new Pen(SystemColors.ControlDark), x, y + 2, x + 88, y + 2);
						g.DrawLine(new Pen(SystemColors.ControlDark), x + 88, y, x + 88, y + 2);
						break;

					case VisualStyles.XP:
						for(int i=0; i < 18; i++)
						{
							// light dot
							g.DrawRectangle(new Pen(SystemColors.ControlLight), x + (i*5), y, 2, 2 );
							// light light dot
							g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1 + (i*5), y + 1, 1, 1 );
							// dark dark dot
							g.DrawRectangle(new Pen(SystemColors.ControlDarkDark), x +(i*5), y, 1, 1 );
							// dark fill
							g.DrawLine(new Pen(SystemColors.ControlDark), x + (i*5), y, x + (i*5) + 1, y);
							g.DrawLine(new Pen(SystemColors.ControlDark), x + (i*5), y, x + (i*5), y + 1);
						}
						break;

					case VisualStyles.Lines:
						for(int i=0; i < 44; i++)
						{
							g.DrawLine(new Pen(SystemColors.ControlDark), x + (i*2), y, x + (i*2), y + 2);
						}

						break;
				}
			}

			#endregion

			else
			{
				throw new Exception("The Collapsible Splitter control cannot have the Filled or None Dockstyle property");
			}
			


			// dispose the Graphics object
			g.Dispose();
		}
		#endregion	

		#region Arrow Polygon Array

		// This creates a point array to draw a arrow-like polygon
		private Point[] ArrowPointArray(int x, int y)
		{
			Point[] point = new Point[3];

			// decide which direction the arrow will point
			if (
				(this.Dock == DockStyle.Right && controlToHide.Visible) 
				|| (this.Dock == DockStyle.Left && !controlToHide.Visible)
				)
			{
				// right arrow
				point[0] = new Point(x,y);
				point[1] = new Point(x + 3, y + 3);
				point[2] = new Point(x, y + 6);
			}
			else if (
				(this.Dock == DockStyle.Right && !controlToHide.Visible) 
				|| (this.Dock == DockStyle.Left && controlToHide.Visible)
				)
			{
				// left arrow
				point[0] = new Point(x + 3 ,y);
				point[1] = new Point(x, y + 3);
				point[2] = new Point(x + 3, y + 6);
			}

			// Up/Down arrows added in v1.2

			else if (
				(this.Dock == DockStyle.Top && controlToHide.Visible) 
				|| (this.Dock == DockStyle.Bottom && !controlToHide.Visible)
				)
			{
				// up arrow
				point[0] = new Point(x + 3, y);
				point[1] = new Point(x + 6, y + 4);
				point[2] = new Point(x, y + 4);
			}
			else if (
				(this.Dock == DockStyle.Top && !controlToHide.Visible) 
				|| (this.Dock == DockStyle.Bottom && controlToHide.Visible)
				)
			{
				// down arrow
				point[0] = new Point(x,y);
				point[1] = new Point(x + 6, y);
				point[2] = new Point(x + 3, y + 3);
			}

			return point;
		}

		#endregion

		#region Color Calculator

		// this method was borrowed from the RichUI Control library by Sajith M
		private static Color CalculateColor(Color front, Color back, int alpha)
		{	
			// solid color obtained as a result of alpha-blending

			Color frontColor = Color.FromArgb(255, front);
			Color backColor = Color.FromArgb(255, back);
									
			float frontRed = frontColor.R;
			float frontGreen = frontColor.G;
			float frontBlue = frontColor.B;
			float backRed = backColor.R;
			float backGreen = backColor.G;
			float backBlue = backColor.B;
			
			float fRed = frontRed*alpha/255 + backRed*((float)(255-alpha)/255);
			byte newRed = (byte)fRed;
			float fGreen = frontGreen*alpha/255 + backGreen*((float)(255-alpha)/255);
			byte newGreen = (byte)fGreen;
			float fBlue = frontBlue*alpha/255 + backBlue*((float)(255-alpha)/255);
			byte newBlue = (byte)fBlue;

			return  Color.FromArgb(255, newRed, newGreen, newBlue);
		}
        
		#endregion

		#region Base Overrides

		// These base overrides were generated by Stephen Toub's DeriveClass utility

		public override System.Windows.Forms.Cursor Cursor
		{
			get
			{
				return base.Cursor;
			}

			set { base.Cursor = value; }
		}

		public override System.Windows.Forms.AnchorStyles Anchor
		{
			get
			{
				return base.Anchor;
			}
			set
			{
				base.Anchor = value;
			}
		}
        
		public override bool AllowDrop
		{
			get
			{
				return base.AllowDrop;
			}
			set
			{
				base.AllowDrop = value;
			}
		}
        
		public override System.Drawing.Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}
        
		public override System.Drawing.Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
			set
			{
				base.BackgroundImage = value;
			}
		}
        
		public override System.Drawing.Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
			}
		}
        
		public override System.Windows.Forms.DockStyle Dock
		{
			get
			{
				return base.Dock;
			}
			set
			{
				base.Dock = value;
			}
		}
        
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}
        
		public override System.Drawing.Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}
        
		public override System.Windows.Forms.BindingContext BindingContext
		{
			get
			{
				return base.BindingContext;
			}
			set
			{
				base.BindingContext = value;
			}
		}
        
		public override System.Windows.Forms.ContextMenu ContextMenu
		{
			get
			{
				return base.ContextMenu;
			}
			set
			{
				base.ContextMenu = value;
			}
		}
        
		public override System.Drawing.Rectangle DisplayRectangle
		{
			get
			{
				return base.DisplayRectangle;
			}
		}
        
		public override bool Focused
		{
			get
			{
				return base.Focused;
			}
		}
        
		public override System.Windows.Forms.RightToLeft RightToLeft
		{
			get
			{
				return base.RightToLeft;
			}
			set
			{
				base.RightToLeft = value;
			}
		}
        
		public override System.ComponentModel.ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				base.Site = value;
			}
		}
        
		public override void ResetText()
		{
			base.ResetText();
		}
        
		public override void Refresh()
		{
			base.Refresh();
		}
        
		public override void ResetRightToLeft()
		{
			base.ResetRightToLeft();
		}
        
		public override void ResetForeColor()
		{
			base.ResetForeColor();
		}
        
		public override void ResetFont()
		{
			base.ResetFont();
		}
        
		public override void ResetCursor()
		{
			base.ResetCursor();
		}
        
		public override void ResetBackColor()
		{
			base.ResetBackColor();
		}
        
		public override bool PreProcessMessage(ref System.Windows.Forms.Message msg)
		{
			return base.PreProcessMessage(ref msg);
		}
        
		public override System.Runtime.Remoting.ObjRef CreateObjRef(System.Type requestedType)
		{
			return base.CreateObjRef(requestedType);
		}
        
		public override object InitializeLifetimeService()
		{
			return base.InitializeLifetimeService();
		}
        
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
        
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
        
		public override string ToString()
		{
			return base.ToString();
		}

		#endregion
    }
}


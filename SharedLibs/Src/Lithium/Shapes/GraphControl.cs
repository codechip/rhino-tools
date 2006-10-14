using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Netron.Lithium
{
	/// <summary>
	/// Summary description for ObjectViewCtrl.
	/// </summary>
	public class GraphControl : ShapeBase
	{
		protected override void OnVisibleChanged()
		{
			base.OnVisibleChanged();
			if(control!=null)
				control.Visible = Visible && ShouldShouldControl;
		}

		private int captionCurveRadius = 10;
		private int captionHeight = 30;
		private GraphicsPath captionPath;

		public Control Control
		{
			get { return control; }
			set
			{
				if(control!=null)
				{
					control.Enter-=new EventHandler(control_Enter);
					control.VisibleChanged-=new EventHandler(control_VisibleChanged);
					this.Lithium.Controls.Remove(control);
				}
				control = value;
				control.Visible = false;
				if(Lithium!=null)
					this.Lithium.Controls.Add(control);
				control.Location = new Point(Left+1,Top+captionHeight);
				control.Enter+=new EventHandler(control_Enter);
				control.VisibleChanged+=new EventHandler(control_VisibleChanged);
				Fit();
			}
		}

		private Image cheveronUp, cheveronDown,cheveronMissing, signPlus,signMinus;
		private Control control;
		private GraphicsPath borderPath;
		private bool shouldShouldControl = false;

		public GraphControl() : base()
		{
			cheveronUp = Image.FromStream(typeof (GraphControl).
				Assembly.GetManifestResourceStream
				("Netron.Lithium.Images.up.png"));
			cheveronDown = Image.FromStream(typeof (GraphControl).
				Assembly.GetManifestResourceStream
				("Netron.Lithium.Images.down.png"));
			cheveronMissing = Image.FromStream(typeof (GraphControl).
				Assembly.GetManifestResourceStream
				("Netron.Lithium.Images.stop.png"));
			signPlus = Image.FromStream(typeof (GraphControl).
				Assembly.GetManifestResourceStream
				("Netron.Lithium.Images.plus.png"));
			signMinus = Image.FromStream(typeof (GraphControl).
				Assembly.GetManifestResourceStream
				("Netron.Lithium.Images.minus.png"));
		}

		public override void Paint(Graphics g)
		{
			DrawCaptionBackground(g);
			DrawBorder(g);
			using(Brush textBrush = new SolidBrush(Color.Black))
			{
				g.DrawString(Text, Font, textBrush, Left+25,+Top+10);
			}
			g.DrawImage(CurrentCheveron, CheveronLeft, CheveronTop, 16, 16);
			if(HasChildren)
				g.DrawImage(CurrentSign, ToggleLeft,ToggleTop, 16, 16);
		}

		private int ToggleTop
		{
			get { return Top+3; }
		}

		private int ToggleLeft
		{
			get { return Left+5; }
		}

		private void DrawCaptionBackground(Graphics g)
		{
			using(Brush bgBrush = new LinearGradientBrush
					  (Rectangle, Color.CornflowerBlue,
					  Color.AntiqueWhite, LinearGradientMode.Horizontal))
			{
				GraphicsPath path = CreateCaptionPath();
				g.FillPath(bgBrush, path);
			}
		}

		private GraphicsPath CreateCaptionPath()
		{
			if(captionPath==null)
			{
				int arcSize = captionCurveRadius*2;
				captionPath = new GraphicsPath();
				captionPath.AddLine( Left+captionCurveRadius, Top, 
					Right-captionCurveRadius-1, Top);
				captionPath.AddArc( Right-arcSize, Top, arcSize, arcSize, 270, 90);
				if(ShowingControl==false)
				{
					captionPath.AddLine( Right, captionCurveRadius, Right, 
						Top+captionHeight-captionCurveRadius);
					captionPath.AddArc(Right-arcSize,Top+captionHeight-arcSize,
						arcSize,arcSize,0,90);
					captionPath.AddLine(Right-captionCurveRadius, Top+captionHeight,
						Left+captionCurveRadius, Top+captionHeight);
					captionPath.AddArc(Left, Top+captionHeight-arcSize,
						arcSize,arcSize,90,90);
					captionPath.AddLine(Left, Top+ captionHeight-captionCurveRadius,
						Left, Top+captionCurveRadius);
				}
				else
				{
					captionPath.AddLine( Right, Top+captionCurveRadius,Right, 
						Top+ captionHeight);
					captionPath.AddLine(Right, Top+captionHeight, Left,
						Top+captionHeight);
					captionPath.AddLine(Left, Top+captionHeight, Left, 
						Top+captionCurveRadius);
					
				}
				captionPath.AddArc(Left, Top, arcSize, arcSize, 180, 90);
			}
			return this.captionPath;
		}

		
		private Image CurrentSign
		{
			get
			{	
				return Expanded ? signMinus : signPlus;
			}
		}
		
		private Pen BorderPen
		{
			get { return IsSelected ? redPen : blackPen; }
		}

	

		private int CheveronTop
		{
			get { return Top+3; }
		}

		private int CheveronLeft
		{
			get { return Left+Width - 20; }
		}

		private Image CurrentCheveron
		{
			get
			{
				if(!CheveronActive)
					return cheveronMissing;
				if (ShowingControl)
					return cheveronUp;
				else
					return cheveronDown;
			}
		}

		public bool ShowingControl
		{
			get { return control!= null ? control.Visible : false; }
		}

		public bool ShouldShouldControl
		{
			get { return shouldShouldControl; }
			set
			{
				shouldShouldControl = value;
				if(control!=null)
				{
					control.Visible = value;
					Fit();
				}
			}
		}

		private void InvalidateBorder()
		{
			int ctrlHeight = control!=null ? control.Height : 0;
			int ctrlWidth = control!=null ? control.Width : 0;
			Rectangle rc = new Rectangle(Left,Top,Width+ctrlWidth,Height+ctrlHeight);
			rc.Inflate(3,3);
			borderPath = null;
			captionPath = null;
			Lithium.Invalidate(rc);
		}

		public override void Fit()
		{
			if(Lithium==null)
				return;
			int ctrlWidth = control!=null ? control.Width+1: 0,
				crlHeight = control!=null && control.Visible? control.Height :0;
			Graphics g = Graphics.FromHwnd(Lithium.Handle);
			Size s = Size.Round(g.MeasureString(string.Concat(Text, "[", 
				childNodes.Count.ToString(), "]"), Font));
			Rectangle = new Rectangle(
				Location, new Size(
				Math.Max(s.Width+50,ctrlWidth),
				captionHeight+crlHeight));
		}

		public override bool Hit(Point p)
		{
			return Rectangle.Contains(p);
		}

		private bool CheveronHit(Point p)
		{
			return new Rectangle(CheveronLeft,CheveronTop,16,16).Contains(p);
		}

		public override void MouseDown(MouseEventArgs e)
		{		
			if(CheveronActive && CheveronHit(new Point(e.X, e.Y)))
				ShouldShouldControl = !ShouldShouldControl;
			if(HasChildren && ToggleHit(new Point(e.X,e.Y)))
			{
				if(Expanded)
					Collapse();
				else
					Expand();
				Lithium.DrawTree();
				Lithium.Invalidate();
			}
		}

		protected virtual bool CheveronActive
		{
			get { return control!=null; }
		}

		public override void Move(Point p)
		{
			borderPath = null;
			captionPath = null;
			base.Move(p);
			if(control!=null)
				control.Location = new Point(Left+1+Lithium.Offset.X, 
					Top+captionHeight+Lithium.Offset.Y);
		}

		private void DrawBorder(Graphics g)
		{
			if(borderPath==null)
			{
				borderPath = new GraphicsPath();
				int arcSize = captionCurveRadius*2;
				borderPath.AddLine(Left+captionCurveRadius, Top, 
					Right-captionCurveRadius, Top);
				borderPath.AddArc( Right-arcSize, Top, arcSize, arcSize, 270, 90);
				if(ShowingControl==false)
				{
					borderPath.AddLine( Right, Top+captionCurveRadius, Right, 
						Bottom-captionCurveRadius);
					borderPath.AddArc(Right-arcSize,Bottom-arcSize,arcSize,arcSize,
						0,90);
					borderPath.AddLine(Right-captionCurveRadius, Bottom, 
						Left+captionCurveRadius, Bottom);
					borderPath.AddArc(Left, Bottom-arcSize,arcSize,arcSize,90,90);
					borderPath.AddLine(Left, Bottom-captionCurveRadius, Left, 
						Top+ captionCurveRadius);
				}
				else
				{
					borderPath.AddLine( Right, Top+captionCurveRadius, Right, Bottom);
					borderPath.AddLine(Right, Bottom, Left, Bottom);
					borderPath.AddLine(Left, Bottom, Left, Top+captionCurveRadius);
				}
				borderPath.AddArc(Left, Top, arcSize, arcSize, 180, 90);
			}	
			g.DrawPath(BorderPen, borderPath);
		}

		protected override void OnSelectionChanged()
		{
			InvalidateBorder();
		}

		private void control_Enter(object sender, EventArgs e)
		{
			Lithium.SelectedShape = this;
		}

		private void control_VisibleChanged(object sender, EventArgs e)
		{
			base.OnVisibleChanged();
			Fit();
			if(Lithium!=null)
				Lithium.DrawTree();
		}

		private bool ToggleHit(Point point)
		{
			return new Rectangle(new Point(ToggleLeft,ToggleTop), new Size(16,16)).Contains(point);
		}

		protected override void OnLithiumChanged()
		{
			Fit();
			if(control!=null)
				Lithium.Controls.Add(control);
		}

		protected override void OnResize()
		{
			base.OnResize();
			control.Location = new Point(Left+1+Lithium.Offset.X,
				Top+Lithium.Offset.Y+captionHeight);
			control.Width = Width-1;
		}

		protected virtual bool HasChildren
		{
			get { return ChildNodes.Count>0; }
		}

		public override void Delete()
		{
			base.Delete ();
			Lithium.Controls.Remove(Control);
		}

	}
}
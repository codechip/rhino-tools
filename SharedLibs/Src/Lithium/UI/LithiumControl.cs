using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Netron.Lithium.Impl.ShapeLayout;
using Netron.Lithium.Interfaces;
using Netron.Lithium.UI;

namespace Netron.Lithium
{
	/// <summary>
	/// Netron's 'Lithium'  tree control
	/// </summary>
	public class LithiumControl : ScrollableControlWithEvents
	{
		#region Events

		/// <summary>
		/// notify the host that an item was hovered by the mouse.
		/// </summary>
		public event ItemHover ItemHovered;

		/// <summary>
		/// occurs when a new node is added to the diagram
		/// </summary>
		public event ShapeData OnNewNode;

		/// <summary>
		/// occurs when a shape is deleted
		/// </summary>
		public event ShapeData OnDeleteNode;

		#endregion

		#region Fields

		/// <summary>
		/// The virtual space that the graph takes.
		/// </summary>
		private Rectangle virtualSpace = new Rectangle(0,0,0,0);

		/// <summary>
		/// The amount of divergance from what is shown in the window.
		/// </summary>
		private Point deltaFromVirtual = Point.Empty;

		/// <summary>
		/// the abstract representation of the graph
		/// </summary>
		protected internal GraphAbstract graphAbstract;

		/// <summary>
		/// the entity hovered by the mouse
		/// </summary>
		protected Entity hoveredEntity;

		/// <summary>
		/// the unique entity currently selected
		/// </summary>
		protected Entity selectedEntity;

		/// <summary>
		/// just a reference point for the OnMouseDown event
		/// </summary>
		protected Point refp;

		/// <summary>
		/// A simple, general purpose random generator
		/// </summary>
		protected Random rnd;

		/// <summary>
		/// simple proxy for the propsgrid of the control
		/// </summary>
		protected Proxy proxy;

		/// <summary>
		/// just the default gridsize used in the paint-background method
		/// </summary>
		protected Size gridSize = new Size(10, 10);

		/// <summary>
		/// the new but volatile connection
		/// </summary>
		protected internal Connection neoCon = null;

		/// <summary>
		/// memory of a connection if the volatile does not end up to a solid connection
		/// </summary>
		protected ShapeBase memChild = null, memParent = null;

		/// <summary>
		/// Amount of time in milliseconds to wait for the ItemHovered event to fire
		/// </summary>
		private int hoverWait = 1500;

		/// <summary>
		/// The layout algorithm for shapes.
		/// </summary>
		private IShapeLayoutStrategy shapeLayoutStrategy = new TreeLayoutStrategy();

		/// <summary>
		/// The layout algorithm for connections
		/// </summary>
		private IConnectionLayoutStrategy connectionLayoutStrategy;

		/// <summary>
		/// The type of the connection between the shapes.
		/// </summary>
		private ConnectionLayout connectionLayout = ConnectionLayout.Traditional;

		protected Color backColor2 = Color.FromKnownColor(KnownColor.InactiveCaption);

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the strategy the tree-layout draws the tree
		/// </summary>
		public IShapeLayoutStrategy ShapeLayoutStrategy
		{
			get { return shapeLayoutStrategy; }
			set
			{
				shapeLayoutStrategy = value;
				connectionLayoutStrategy = shapeLayoutStrategy.CreateConnectionLayoutStrategy(connectionLayout);
				DrawTree();
			}
		}

		/// <summary>
		/// Gets the strategy for drawing the connections.
		/// </summary>
		public IConnectionLayoutStrategy ConnectionLayoutStrategy
		{
			get { return connectionLayoutStrategy; }
		}

		public ConnectionLayout ConnectionLayout
		{
			get { return connectionLayout; }
			set
			{
				connectionLayout = value;
				connectionLayoutStrategy = shapeLayoutStrategy.CreateConnectionLayoutStrategy(connectionLayout);
				DrawTree();
			}
		}

			/// <summary>
			/// Gets or sets the shape collection
			/// </summary>
			[Browsable(false)]
			public ShapeCollection Shapes
			{
				get { return graphAbstract.Shapes; }
				set { graphAbstract.Shapes = value; }
			}
	
			/// <summary>
			/// Gets or sets the connection collection
			/// </summary>
			[Browsable(false)]
			public ConnectionCollection Connections
			{
				get { return graphAbstract.Connections; }
				set { graphAbstract.Connections = value; }
			}

		/// <summary>
		/// Gets the root of the diagram
		/// </summary>
		[Browsable(false)]
		public ShapeBase Root
		{
			get { return graphAbstract.Root; }
		}

		/// <summary>
		/// The amount of time (in milliseconds) that the mouse should rest on an element
		/// before the HoverItem event is fired.
		/// </summary>
		[Browsable(true)]
		public int HoverWait
		{
			get { return hoverWait; }
			set { hoverWait = value; }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Default ctor
		/// </summary>
		public LithiumControl()
		{
			//double-buffering
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);

			connectionLayoutStrategy = shapeLayoutStrategy.CreateConnectionLayoutStrategy(ConnectionLayout.Traditional);

			//init the abstract
			graphAbstract = new GraphAbstract();
			graphAbstract.Shapes.OnShapeAdded += new ShapeData(OnShapeAdded);
			OnDeleteNode+=new ShapeData(LithiumControl_NodeChanges);
			OnNewNode+=new ShapeData(LithiumControl_NodeChanges);
			HorzScrollValueChanged+=new ScrollEventHandler(LithiumControl_ScrollChanged);
			VertScrollValueChanged+=new ScrollEventHandler(LithiumControl_ScrollChanged);
			MouseDown+=new MouseEventHandler(LithiumControl_MouseDown);
			//init the randomizer
			rnd = new Random();

			//init the proxy
			proxy = new Proxy(this);

			//set gradient
			BackColor = Color.FromKnownColor(KnownColor.ActiveCaption);
		}



		#endregion

		#region Methods


		/// <summary>
		/// Passes the event from the abstracts shape collection to the outside.
		/// Having the event in the GraphAbstract being raised centralizes it,
		/// otherwise the event should be raise in various places
		/// </summary>
		/// <param name="shape"></param>
		private void OnShapeAdded(ShapeBase shape)
		{
			if (this.OnNewNode != null)
				OnNewNode(shape);
		}

		/// <summary>
		/// Paints the control
		/// </summary>
		/// <remarks>
		/// If you switch the painting order of Connections and shapes the connection line
		/// will be underneath/above the shape
		/// </remarks>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			//use the best quality, with a performance penalty
			g.SmoothingMode = SmoothingMode.AntiAlias;
			using(Brush backgroundBrush = new LinearGradientBrush(
					  this.ClientRectangle,BackColor, BackColor2,
					  LinearGradientMode.BackwardDiagonal))
			{
				g.FillRectangle(backgroundBrush, ClientRectangle);
			}
			g.TranslateTransform(Offset.X, Offset.Y	);
			DoPainting(g);
			g.TranslateTransform(0,0);
		}

		public Point Offset
		{
			get
			{
				return new Point(DisplayRectangle.X-ClientRectangle.X, 
					DisplayRectangle.Y-ClientRectangle.Y);
			}
		}

		private void DoPainting(Graphics g)
		{	
			for (int i = 0; i < Connections.Count; i++)
			{
				Connections[i].Paint(g);
			}
	
			for (int i = 0; i < Shapes.Count; i++)
			{
				if (Shapes[i].Visible)
					Shapes[i].Paint(g);
			}
		}

		[Browsable(true)]
		public Color BackColor2
		{
			get { return backColor2; }
			set
			{
				backColor2 = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Adds the root of the diagram to the canvas
		/// </summary>
		public ShapeBase AddRoot(ShapeBase root)
		{
			if (Shapes.Count > 0)
				throw new Exception("You cannot set the root unless the diagram is empty");
			root.IsRoot = true;
			root.Visible = true;
			root.Lithium = this;
			Shapes.Add(root);
			//set the root of the diagram
			this.graphAbstract.Root = root;
			CenterRoot();
			return root;
		}


		/// <summary>
		/// Centers the root on the control's canvas
		/// </summary>
		public void CenterRoot()
		{
			graphAbstract.Root.Location = ShapeLayoutStrategy.GetDefaultRootPosition(this, graphAbstract.Root);
			//Invalidate();
			DrawTree();

		}

		/// <summary>
		/// Move with the given vector
		/// </summary>
		/// <param name="p"></param>
		public void MoveDiagram(Point p)
		{
			//move the whole diagram
			deltaFromVirtual.Offset(p.X, p.Y);
			foreach (ShapeBase shape in Shapes)
			{
				shape.Move(p);
			}
			Invalidate();
			return;

		}

		#region Mouse event handlers

		/// <summary>
		/// Handles the mouse-down event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			Point p = new Point(e.X-DisplayRectangle.X, e.Y-DisplayRectangle.Y);
			//set the point for the next round
			refp = p;
			MouseEventArgs offsetEvent = new MouseEventArgs(e.Button,e.Clicks,
				p.X, p.Y,e.Delta);
			
			ShapeBase sh;
			for (int k = 0; k < Shapes.Count; k++)
			{
				sh = Shapes[k];

				if (Shapes[k].Visible && Shapes[k].Hit(p))
				{
					//shapes[k].ShapeColor = Color.WhiteSmoke;
					if (selectedEntity != null)
						selectedEntity.IsSelected = false;
					selectedEntity = Shapes[k];
					selectedEntity.IsSelected = true;
					sh = selectedEntity as ShapeBase;
					Rectangle bounds = sh.Rectangle;
					bounds.Inflate(3, 3);
					sh.MouseDown(offsetEvent);
					Invalidate(bounds);
					return;
				}
			}

			if (selectedEntity != null)
				selectedEntity.IsSelected = false;


			selectedEntity = null;
			Invalidate();

		}

		#endregion

		/// <summary>
		/// Collapses the whole diagram
		/// </summary>
		public void CollapseAll()
		{
			this.Root.Collapse();
		}

		#region Layout algorithm

		/// <summary>
		/// Generic entry point to layout the diagram on the canvas.
		/// The default LayoutDirection is vertical. If you wish to layout the tree in a certain
		/// direction you need to specify this property first. Also, the direction is global, you cannot have 
		/// different parts being drawn in different ways though it can be implemented.
		/// 
		/// </summary>
		public void DrawTree()
		{
			if(graphAbstract==null || graphAbstract.Root==null)
				return;
			//the shift vector difference between the original and the moved root
			Point p = graphAbstract.Root.Location;
			Size size = Size.Empty;
			shapeLayoutStrategy.DrawTree(graphAbstract.Root, 
				graphAbstract.Root.Left, graphAbstract.Root.Top,
				ref size);
			AutoScrollMinSize = new Size(size.Width+10,size.Height+10);
			p = new Point(-graphAbstract.Root.Left + p.X, 
				-graphAbstract.Root.Top + p.Y);
			MoveDiagram(p);
			Invalidate();
		}

		/// <summary>
		/// DFT of the diagram with the given visitor, starting from the given shape
		/// </summary>
		/// <param name="visitor"></param>
		/// <param name="shape"></param>
		public void DepthFirstTraversal(IVisitor visitor, ShapeBase shape)
		{
			graphAbstract.DepthFirstTraversal(visitor, shape);
		}


		/// <summary>
		/// DFT of the diagram with the given visitor, starting from the root
		/// </summary>
		/// <param name="visitor"></param>
		public void DepthFirstTraversal(IVisitor visitor)
		{
			graphAbstract.DepthFirstTraversal(visitor);
		}

		/// <summary>
		/// BFT of the diagram with the given visitor, starting from the root
		/// </summary>
		/// <param name="visitor"></param>
		public void BreadthFirstTraversal(IVisitor visitor)
		{
			graphAbstract.BreadthFirstTraversal(visitor);
		}

		/// <summary>
		/// BFT of the diagram with the given visitor, starting from the given shape
		/// </summary>
		/// <param name="visitor"></param>
		/// <param name="shape"></param>
		public void BreadthFirstTraversal(IVisitor visitor, ShapeBase shape)
		{
			graphAbstract.BreadthFirstTraversal(visitor, shape);
		}


		protected internal void OnItemHover(ShapeBase hoveredShape)
		{
			if (ItemHovered != null)
				ItemHovered(hoveredShape);
		}

	

		public Entity SelectedShape
		{
			get { return selectedEntity; }
			set
			{
				if (selectedEntity != null && selectedEntity != value)
					selectedEntity.IsSelected = false;
				selectedEntity = value;
				if (value != null)
					value.IsSelected = true;
			}
		}

		#endregion

		#endregion


		private void LithiumControl_NodeChanges(ShapeBase shape)
		{
			//The shape is outside our virtual space
            if(!virtualSpace.Contains(shape.Rectangle))
            {
				virtualSpace.X = Math.Min(virtualSpace.X,shape.Rectangle.X);
				virtualSpace.Y = Math.Min(virtualSpace.Y,shape.Rectangle.Y);
				virtualSpace.Width = Math.Max(virtualSpace.X+virtualSpace.Width,
					shape.Rectangle.X+shape.Rectangle.Width);
				virtualSpace.Height= Math.Max(virtualSpace.Y+virtualSpace.Height,
					shape.Rectangle.Y+shape.Rectangle.Height);
            }
		}

		private void LithiumControl_ScrollChanged(object sender, ScrollEventArgs e)
		{
			Invalidate();
		}

		private void LithiumControl_MouseDown(object sender, MouseEventArgs e)
		{
			this.Select();
		}
	}
}
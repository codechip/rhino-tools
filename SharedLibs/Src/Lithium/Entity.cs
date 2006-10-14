using System;
using System.ComponentModel;
using System.Drawing;

namespace Netron.Lithium
{
	/// <summary>
	/// Abstract base class for every object part of the diagram
	/// </summary>
	public abstract class Entity: IDisposable
	{
		#region Fields

		/// <summary>
		/// the control to which the eneity belongs
		/// </summary>
		private LithiumControl lithium;

		/// <summary>
		/// tells whether the entity is selected
		/// </summary>
		private bool isSelected = false;

		private bool visible = true;

		/// <summary>
		/// Default black pen
		/// </summary>
		protected Pen blackPen = new Pen(Brushes.Black, 1f);

		/// <summary>
		/// a red pen
		/// </summary>
		protected Pen redPen = new Pen(Brushes.Red, 2f);

		/// <summary>
		/// The current pen
		/// </summary>
		protected Pen pen;

		/// <summary>
		/// a thicker version of the black pen
		/// </summary>
		protected Pen thickPen = new Pen(Color.Black, 1.7f);

		#endregion

		#region Properties

		public bool Visible
		{
			get { return visible; }
			set
			{
				visible = value;
				OnVisibleChanged();
			}
		}

		/// <summary>
		/// Gets or sets whether the entity is selected
		/// </summary>
		[Browsable(false)]
		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				isSelected = value;
				OnSelectionChanged();
			}
		}

		/// <summary>
		/// Gets or sets the site of the entity
		/// </summary>
		[Browsable(false)]
		public LithiumControl Lithium
		{
			get { return lithium; }
			set
			{
				lithium = value;
				OnLithiumChanged();
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Tests whether the shape is hit by the mouse
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public abstract bool Hit(Point p);

		public abstract void Paint(Graphics g);

		/// <summary>
		/// Moves the entity on the canvas
		/// </summary>
		/// <param name="p">the shifting vector, not an absolute position!</param>
		public abstract void Move(Point p);

		public abstract void Invalidate();

		#endregion

		#region IDisposable Implementation

		void IDisposable.Dispose()
		{
			redPen.Dispose();
			blackPen.Dispose();
			thickPen.Dispose();
		}

		#endregion

		#region Virtual Methods For Children To Override
		protected virtual void OnSelectionChanged()
		{}

		protected virtual void OnVisibleChanged()
		{}

		protected virtual void OnLithiumChanged()
		{}

		#endregion

		
	}
}
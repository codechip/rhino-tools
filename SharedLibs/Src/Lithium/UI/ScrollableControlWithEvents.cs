//This code was taken from
//http://www.codeproject.com/cs/miscctrl/ExScrollableControl.asp
//Thanks, Martin.

using System.Windows.Forms;

namespace Netron.Lithium.UI
{
	/// <summary>
	/// Adds the missing scroll events to the scrollable control!
	/// Written by Martin Randall - Thursday 17th June, 2004
	/// </summary>
	public class ScrollableControlWithEvents : ScrollableControl
	{
		
		private const int WM_MOUSEWHEEL		= 0x20A;
		private const int WM_HSCROLL			= 0x114;
		private const int WM_VSCROLL			= 0x115;

		/// <summary>
		/// Horizontal scroll position has changed event
		/// </summary>
		public event ScrollEventHandler HorzScrollValueChanged;

		/// <summary>
		/// Vertical scroll position has changed event
		/// </summary>
		public event ScrollEventHandler VertScrollValueChanged;
	
		/// <summary>
		/// Intercept scroll messages to send notifications
		/// </summary>
		/// <param name="m">Message parameters</param>
		protected override void WndProc(ref Message m)
		{
			// Was this a horizontal scroll message?
			if ( m.Msg == WM_HSCROLL ) 
			{
				if ( HorzScrollValueChanged != null ) 
				{
					uint wParam = (uint)m.WParam.ToInt32();
					HorzScrollValueChanged( this, 
						new ScrollEventArgs( 
						GetEventType( wParam & 0xffff), (int)(wParam >> 16) ) );
				}
			} 
				// or a vertical scroll message?
			else if ( m.Msg == WM_VSCROLL || m.Msg == WM_MOUSEWHEEL)
			{
				if ( VertScrollValueChanged != null )
				{
					uint wParam = (uint)m.WParam.ToInt32();
					VertScrollValueChanged( this, 
						new ScrollEventArgs( 
						GetEventType( wParam & 0xffff), (int)(wParam >> 16) ) );
				}
			}

			// Let the control process the message
			base.WndProc (ref m);
		}

		// Based on SB_* constants
		private static ScrollEventType [] _events =
			new ScrollEventType[] {
									  ScrollEventType.SmallDecrement,
									  ScrollEventType.SmallIncrement,
									  ScrollEventType.LargeDecrement,
									  ScrollEventType.LargeIncrement,
									  ScrollEventType.ThumbPosition,
									  ScrollEventType.ThumbTrack,
									  ScrollEventType.First,
									  ScrollEventType.Last,
									  ScrollEventType.EndScroll
								  };
		/// <summary>
		/// Decode the type of scroll message
		/// </summary>
		/// <param name="wParam">Lower word of scroll notification</param>
		/// <returns></returns>
		private ScrollEventType GetEventType( uint wParam )
		{
			if ( wParam < _events.Length )
				return _events[wParam];
			else
				return ScrollEventType.EndScroll;
		}
	}

}

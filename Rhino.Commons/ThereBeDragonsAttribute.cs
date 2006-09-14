using System;

namespace Rhino.Commons
{
	/// <summary>
	/// This marker attribute decorate dangerous parts of the code
	/// where for some reason the developer decided to do something in a way
	/// that is not straight-forward.
	/// This usually means that this is using some functionality that is not fully supported.
	/// </summary>
	public class ThereBeDragonsAttribute : Attribute
	{
		public ThereBeDragonsAttribute()
		{
		}

		public ThereBeDragonsAttribute(string whyAreYouUsingThis)
		{

		}
	}
}
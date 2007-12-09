#region Disclaimer/Info
///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at SourceForge at http://sourceforge.net/projects/subtext
// The development mailing list is at subtext-devs@lists.sourceforge.net 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////
#endregion

namespace Blog.Model
{
	/// <summary>
	/// Enumates the various types of comments within the subtext content table.
	/// </summary>
	public enum FeedbackType
	{
		None = 0,
		Comment = 1,
		PingTrack = 2,
		ContactPage = 3, //Only applies if "ContactToFeedback" is set to true.
	}
}
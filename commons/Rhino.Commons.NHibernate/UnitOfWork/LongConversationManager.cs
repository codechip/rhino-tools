using System;
using System.Collections;
using System.Web;
using NHibernate;
using Rhino.Commons.Properties;

namespace Rhino.Commons
{
	/// <summary>
	/// The LongConversationManager class is used internally to manage both "public" 
	/// and "private" NHibernate sessions that span multiple requests. When using UnitOfWork.StartLongConversation 
	/// all future requests will be part of the same session until it is desposed. When using 
	/// UnitOfWork.StartPrivateConversation requests with the correct GUID key will be resumed, otherwise the default
	/// behavior will occur. The GUID key can be passed in the Query String or as a Form variable, and defaults to the name
	/// LongConverstionKey. 
	/// 
	/// Private conversations are recommended for AJAX intensive applications where multiple requests may
	/// be occuring simultaneously, some of which need to be isolated from other requests. When using any type of 
	/// Long Conversations (public or private) it is important to manage transactions and concurrency correctly.
	/// </summary>
	internal static class LongConversationManager
	{
		private const string LongConversationStorageKey = "LongConversationStorage.Key";
		private const string DefaultLongConversationKey = "DefaultLongConversation.Key";

		//defaults to LongConversationKey i.e. http://yourserver.com/page.asp?LongConversationKey=GUIDKEYHERE
		private static readonly string LongConversationRequestKey = Settings.Default.LongConversationRequestKey;

		public static void SaveConversation()
		{
			Hashtable table = new Hashtable();
			IoC.Resolve<IUnitOfWorkFactory>().SaveUnitOfWorkToHashtable(table);
			if (UnitOfWork.LongConversationIsPrivate)
				Conversations.Add(UnitOfWork.CurrentLongConversationId, table);
			else
				Conversations.Add(DefaultLongConversationKey, table);
		}

		public static bool LoadConversation()
		{

			bool privateConversation;
			Hashtable conversation = LoadConversationFromRequest(out privateConversation);
			if (conversation != null)
			{
				IUnitOfWork UoW;
				Guid? longConversationId;
				IoC.Resolve<IUnitOfWorkFactory>().LoadUnitOfWorkFromHashtable(conversation, out UoW, out longConversationId);

				UnitOfWork.LongConversationIsPrivate = privateConversation;
	
				UnitOfWork.Current = UoW;
				UnitOfWork.CurrentLongConversationId = longConversationId;
				UnitOfWork.CurrentSession.Reconnect();

				Conversations.Remove(conversation);
				return true;
			}
			return false;
		}

		public static void EndAllConversations()
		{
			HttpContext.Current.Session[LongConversationStorageKey] = null;
		}

		private static Hashtable LoadConversationFromRequest(out bool privateConversation)
		{
			Hashtable conversation;
			string keyString = HttpContext.Current.Request.QueryString[LongConversationRequestKey] ??
						 HttpContext.Current.Request.Form[LongConversationRequestKey];
			if (!string.IsNullOrEmpty(keyString))
			{
				Guid conversationKey = new Guid(keyString);
				conversation = (Hashtable)Conversations[conversationKey];
				if (conversation == null)
					throw new InvalidOperationException("Attempted to load a specific UnitOfWork that no longer exists.");
				Conversations.Remove(conversationKey);
				privateConversation = true;
			}
			else
			{
				conversation = (Hashtable)Conversations[DefaultLongConversationKey];
				Conversations.Remove(DefaultLongConversationKey);
				privateConversation = false;
			}
			return conversation;
		}

		private static Hashtable Conversations
		{
			get
			{
				Hashtable hashtable = (Hashtable)HttpContext.Current.Session[LongConversationStorageKey];
				if (hashtable == null)
					HttpContext.Current.Session[LongConversationStorageKey] = hashtable = new Hashtable();
				return hashtable;
			}
		}
	}
}
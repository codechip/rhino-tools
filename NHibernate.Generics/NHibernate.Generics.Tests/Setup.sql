	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NBlogs]') AND type in (N'U'))
	BEGIN
	CREATE TABLE [dbo].[NBlogs](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](255) NULL,
		[Author] [nvarchar](255) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
	END
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Blogs]') AND type in (N'U'))
	BEGIN
	CREATE TABLE [dbo].[Blogs](
		[blog_id] [int] IDENTITY(1,1) NOT NULL,
		[blog_name] [varchar](50) NULL,
	 CONSTRAINT [PK_Blogs] PRIMARY KEY CLUSTERED 
	(
		[blog_id] ASC
	)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
	END
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
	BEGIN
	CREATE TABLE [dbo].[Users](
		[user_id] [int] IDENTITY(1,1) NOT NULL,
		[user_name] [varchar](50) NOT NULL,
	 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
	(
		[user_id] ASC
	)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
	END
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Attributes]') AND type in (N'U'))
	BEGIN
	CREATE TABLE [dbo].[Attributes](
		[ItemId] [int] NOT NULL,
		[Name] [nvarchar](50) NOT NULL,
		[Value] [nvarchar](50) NULL,
		[Id] [int] IDENTITY(1,1) NOT NULL,
	 CONSTRAINT [PK_Attributes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
	END
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Posts]') AND type in (N'U'))
	BEGIN
	CREATE TABLE [dbo].[Posts](
		[post_id] [int] IDENTITY(1,1) NOT NULL,
		[post_title] [varchar](50) NULL,
		[post_blogid] [int] NULL,
	 CONSTRAINT [PK_Posts] PRIMARY KEY CLUSTERED 
	(
		[post_id] ASC
	)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
	END
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UsersBlogs]') AND type in (N'U'))
	BEGIN
	CREATE TABLE [dbo].[UsersBlogs](
		[user_id] [int] NOT NULL,
		[blog_id] [int] NOT NULL,
	 CONSTRAINT [PK_UsersBlogs] PRIMARY KEY CLUSTERED 
	(
		[user_id] ASC,
		[blog_id] ASC
	)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
	END
	GO
	SET ANSI_NULLS ON
	GO
	SET QUOTED_IDENTIFIER ON
	GO
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Comments]') AND type in (N'U'))
	BEGIN
	CREATE TABLE [dbo].[Comments](
		[comment_id] [int] IDENTITY(1,1) NOT NULL,
		[comment_text] [varchar](50) NULL,
		[comment_postid] [int] NOT NULL,
		[comment_post_index] [int] NOT NULL,
	 CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED 
	(
		[comment_id] ASC
	)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
	END
	GO
	IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Posts_Blogs]') AND parent_object_id = OBJECT_ID(N'[dbo].[Posts]'))
	ALTER TABLE [dbo].[Posts]  WITH CHECK ADD  CONSTRAINT [FK_Posts_Blogs] FOREIGN KEY([post_blogid])
	REFERENCES [dbo].[Blogs] ([blog_id])
	GO
	IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UsersBlogs_Blogs]') AND parent_object_id = OBJECT_ID(N'[dbo].[UsersBlogs]'))
	ALTER TABLE [dbo].[UsersBlogs]  WITH CHECK ADD  CONSTRAINT [FK_UsersBlogs_Blogs] FOREIGN KEY([blog_id])
	REFERENCES [dbo].[Blogs] ([blog_id])
	GO
	IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UsersBlogs_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[UsersBlogs]'))
	ALTER TABLE [dbo].[UsersBlogs]  WITH CHECK ADD  CONSTRAINT [FK_UsersBlogs_Users] FOREIGN KEY([user_id])
	REFERENCES [dbo].[Users] ([user_id])
	GO
	IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Comments_Posts]') AND parent_object_id = OBJECT_ID(N'[dbo].[Comments]'))
	ALTER TABLE [dbo].[Comments]  WITH CHECK ADD  CONSTRAINT [FK_Comments_Posts] FOREIGN KEY([comment_postid])
	REFERENCES [dbo].[Posts] ([post_id])

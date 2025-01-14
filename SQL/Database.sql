GO
CREATE DATABASE [CrawlDataWatch]
GO

GO
USE [CrawlDataWatch]
GO
/****** Object:  Table [dbo].[ProductImage]    Script Date: 10/6/2022 8:16:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductImage](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NULL,
	[OriginLinkImage] [nvarchar](500) NULL,
	[LocalPathImage] [nvarchar](500) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](300) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedBy] [nvarchar](300) NULL,
	[UpdatedAt] [datetime2](7) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductProperty]    Script Date: 10/6/2022 8:16:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductProperty](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NULL,
	[PropertyName] [nvarchar](255) NULL,
	[PropertyValue] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](300) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedBy] [nvarchar](300) NULL,
	[UpdatedAt] [datetime2](7) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 10/6/2022 8:16:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductName] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Price] [decimal](19, 4) NOT NULL,
	[Discount] [decimal](19, 4) NULL,
	[Currency] [varchar](100) NOT NULL,
	[OriginLinkDetail] [nvarchar](500) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [nvarchar](300) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedBy] [nvarchar](300) NULL,
	[UpdatedAt] [datetime2](7) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[ProductImage] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ProductImage] ADD  DEFAULT ('https://www.code-mega.com') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[ProductImage] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ProductProperty] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ProductProperty] ADD  DEFAULT ('https://www.code-mega.com') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[ProductProperty] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ('https://www.code-mega.com') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
/****** Object:  StoredProcedure [dbo].[spInsUpdCrawlDataWebsite]    Script Date: 10/6/2022 8:16:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
===========================================================================================
  ** AUTHOR			: https://www.code-mega.com
  ** CREATED DATE	: 2022/05/10
  ** DESCRIPTION	: Crawl Data Website
===========================================================================================
*/
 
CREATE PROC [dbo].[spInsUpdCrawlDataWebsite]
	@JInputProductDetail	NVARCHAR(MAX)
	,@JInputProductProperty	NVARCHAR(MAX)
	,@JInputProductImage	NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	SELECT *
	INTO #TempProductDetail
	FROM OPENJSON(@JInputProductDetail)
	WITH
		(
			 Id				NVARCHAR(MAX)
			 ,ProductName	NVARCHAR(MAX)
			 ,Description	NVARCHAR(MAX)
			 ,Price			DECIMAL(19, 4)
			 ,Discount		DECIMAL(19, 4)
			 ,Currency		NVARCHAR(MAX)
			 ,OriginLinkDetail		NVARCHAR(MAX)
		) I

	SELECT *
	INTO #TempProductProperty
	FROM OPENJSON(@JInputProductProperty)
	WITH
		(
			 Id				NVARCHAR(MAX)
			 ,ProductId		NVARCHAR(MAX)
			 ,PropertyName	NVARCHAR(MAX)
			 ,PropertyValue	NVARCHAR(MAX)
		) I

	SELECT *
	INTO #TempProductImage
	FROM OPENJSON(@JInputProductImage)
	WITH
		(
			 Id					NVARCHAR(MAX)
			 ,ProductId			NVARCHAR(MAX)
			 ,OriginLinkImage	NVARCHAR(MAX)
			 ,LocalPathImage	NVARCHAR(MAX)
		) I


	INSERT INTO dbo.Products
	(
	    Id,
	    ProductName,
	    Description,
	    Price,
	    OriginLinkDetail,
		Currency,
		Discount
	)
	SELECT 
		Id,
	    ProductName,
	    Description,
	    Price,
	    OriginLinkDetail,
		Currency,
		Discount
	FROM #TempProductDetail

	INSERT INTO dbo.ProductProperty
	(
	    Id,
	    ProductId,
	    PropertyName,
	    PropertyValue
	)
	SELECT
		Id,
	    ProductId,
	    PropertyName,
	    PropertyValue
	FROM #TempProductProperty

	INSERT INTO dbo.ProductImage
	(
	    Id,
	    ProductId,
	    OriginLinkImage,
	    LocalPathImage
	)
	SELECT
		Id,
	    ProductId,
	    OriginLinkImage,
	    LocalPathImage
	FROM #TempProductImage

	DROP TABLE #TempProductDetail
	DROP TABLE #TempProductProperty
	DROP TABLE #TempProductImage

	SET NOCOUNT OFF
END
         
GO

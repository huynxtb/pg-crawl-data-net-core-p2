DECLARE @ProductId	NVARCHAR(MAX) = (SELECT TOP 1 Id FROM dbo.Products)

SELECT * FROM dbo.Products WHERE Id = @ProductId

SELECT * FROM dbo.ProductProperty WHERE ProductId = @ProductId

SELECT * FROM dbo.ProductImage WHERE ProductId = @ProductId
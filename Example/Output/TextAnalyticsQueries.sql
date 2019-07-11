DECLARE @ItemID int
DECLARE @TypeToAnalyze nvarchar(50)
DECLARE @WindowSize int

SET @ItemID = 20995
SET @TypeToAnalyze = 'Location'
SET @WindowSize = 100

SELECT	o.ItemID, o.PageID, o.Type, o.Name, o.Offset, n.Type, n.Name, n.Offset, n.IsScientificName
FROM	(
		SELECT	* FROM TextAnalytics 
		WHERE	ItemID = @ItemID 
		AND		Type = @TypeToAnalyze
		) o
		INNER JOIN
		(
		SELECT * FROM TextAnalytics
		WHERE	ItemID = @ItemID
		--AND		Type <> 'Location'
		) n ON o.PageID = n.PageID
WHERE	(o.ItemID <> n.ItemID OR o.Seq <> n.Seq)
AND		n.Offset BETWEEN o.Offset - @WindowSize AND o.Offset + @WindowSize
ORDER BY
		o.PageID, o.Name, o.Offset


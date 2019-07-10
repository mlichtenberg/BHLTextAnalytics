DECLARE @TypeToAnalyze nvarchar(50)
SET @TypeToAnalyze = 'Location'
DECLARE @WindowSize int
SET @WindowSize = 100

SELECT	o.PageID, o.Type, o.Name, o.Offset, n.Type, n.Name, n.Offset, n.IsScientificName
FROM	(
		SELECT * FROM TextAnalytics 
		WHERE Type = @TypeToAnalyze
		) o
		INNER JOIN
		(
		SELECT * FROM TextAnalytics
		--WHERE Type <> 'Location'
		) n ON o.PageID = n.PageID
WHERE	o.Seq <> n.Seq
AND		n.Offset BETWEEN o.Offset - @WindowSize AND o.Offset + @WindowSize
ORDER BY
		o.PageID, o.Name, o.Offset

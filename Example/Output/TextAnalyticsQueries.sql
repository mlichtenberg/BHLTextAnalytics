DECLARE @ItemID int
DECLARE @TypeToAnalyze nvarchar(50)
DECLARE @WindowSize int

SET @ItemID = 20995					-- The BHL ID of the book to be analyzed
SET @TypeToAnalyze = 'Location'		-- The Entity type to analyze
SET @WindowSize = 100				-- When determining the proximity of Entities, this is the maximum number
									-- of characters Entities can be away from each other

-- For this book (Item 20995), some entities should be excluded.  This initial select performs the exclusions.
-- This may not be necessary, or valid, or all books.
SELECT	*
INTO	#TextAnalytics
FROM	TextAnalytics
WHERE	Offset > 50				-- Ignore entities found in the page headers by skipping any that appear in the first 50 characters.
AND		(Type <> 'Location'		-- Include all non-location entities
OR			(
				Type = 'Location' AND
				-- Only include locations greater than six characters in length.  Shorter locations are likely to be invalid.
				Length > 6 AND
				-- Only include locations that do not start with lowercase letters.  Lowercase locations are likely invalid.
				(ASCII(SUBSTRING(Name, 1, 1)) < 97 OR ASCII(SUBSTRING(Name, 1, 1)) > 122)
			)
		)

-- Perform the analysis
SELECT	o.ItemID, o.PageID, o.Type, o.Name, o.Offset, n.Type, n.Name, n.Offset, n.IsScientificName
FROM	(
		SELECT	* FROM #TextAnalytics 
		WHERE	ItemID = @ItemID 
		AND		Type = @TypeToAnalyze
		) o
		INNER JOIN
		(
		SELECT * FROM #TextAnalytics
		WHERE	ItemID = @ItemID
		--AND		Type <> 'Location'
		) n ON o.PageID = n.PageID
WHERE	(o.ItemID <> n.ItemID OR o.Seq <> n.Seq)
AND		n.Offset BETWEEN o.Offset - @WindowSize AND o.Offset + @WindowSize
ORDER BY
		o.PageID, o.Name, o.Offset


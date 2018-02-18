
/****** Object:  UserDefinedFunction [dbo].[DamLev]    Script Date: 02/18/2018 07:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Computes and returns the Damerau-Levenshtein edit distance between two strings, 
-- i.e. the number of insertion, deletion, substitution, and transposition edits
-- required to transform one string to the other.  This value will be >= 0, where
-- 0 indicates identical strings. Comparisons use the case-sensitivity configured
-- in SQL Server (case-insensitive by default). This algorithm is basically the
-- Levenshtein algorithm with a modification that considers transposition of two
-- adjacent characters as a single edit.
-- This version differs by including some optimizations, and extending it to the Damerau-
-- Levenshtein algorithm.
-- Note that this is the simpler and faster optimal string alignment (aka restricted edit) distance
-- that difers slightly from the full Damerau-Levenshtein algorithm by imposing the restriction
-- that no substring is edited more than once. So for example, "CA" to "ABC" has an edit distance
-- of 2 by a complete application of Damerau-Levenshtein, but a distance of 3 by this method that
-- uses the optimal string alignment algorithm. See wikipedia article for more detail on this
-- distinction.
-- 
-- @s - String being compared for distance.
-- @t - String being compared against other string.
-- @max - Maximum distance allowed, or NULL if no maximum is desired. Returns NULL if distance will exceed @max.
-- returns int edit distance, >= 0 representing the number of edits required to transform one string to the other.
-- =============================================
 
CREATE FUNCTION [dbo].[DamLev](
 
    @s nvarchar(4000)
  , @t nvarchar(4000)
  , @max int
)
RETURNS int
WITH SCHEMABINDING
AS
BEGIN
    DECLARE @distance int = 0 -- return variable
          , @v0 nvarchar(4000)-- running scratchpad for storing computed distances
          , @v2 nvarchar(4000)-- running scratchpad for storing previous column's computed distances
          , @start int = 1      -- index (1 based) of first non-matching character between the two string
          , @i int, @j int      -- loop counters: i for s string and j for t string
          , @diag int          -- distance in cell diagonally above and left if we were using an m by n matrix
          , @left int          -- distance in cell to the left if we were using an m by n matrix
          , @nextTransCost int-- transposition base cost for next iteration 
          , @thisTransCost int-- transposition base cost (2 distant along diagonal) for current iteration
          , @sChar nchar      -- character at index i from s string
          , @tChar nchar      -- character at index j from t string
          , @thisJ int          -- temporary storage of @j to allow SELECT combining
          , @jOffset int      -- offset used to calculate starting value for j loop
          , @jEnd int          -- ending value for j loop (stopping point for processing a column)
          -- get input string lengths including any trailing spaces (which SQL Server would otherwise ignore)
          , @sLen int = datalength(@s) / datalength(left(left(@s, 1) + '.', 1))    -- length of smaller string
          , @tLen int = datalength(@t) / datalength(left(left(@t, 1) + '.', 1))    -- length of larger string
          , @lenDiff int      -- difference in length between the two strings
    -- if strings of different lengths, ensure shorter string is in s. This can result in a little
    -- faster speed by spending more time spinning just the inner loop during the main processing.
    IF (@sLen > @tLen) BEGIN
        SELECT @v0 = @s, @i = @sLen -- temporarily use v0 for swap
        SELECT @s = @t, @sLen = @tLen
        SELECT @t = @v0, @tLen = @i
    END
    SELECT @max = ISNULL(@max, @tLen)
         , @lenDiff = @tLen - @sLen
    IF @lenDiff > @max RETURN NULL
 
    -- suffix common to both strings can be ignored
    WHILE(@sLen > 0 AND SUBSTRING(@s, @sLen, 1) = SUBSTRING(@t, @tLen, 1))
        SELECT @sLen = @sLen - 1, @tLen = @tLen - 1
 
    IF (@sLen = 0) RETURN @tLen
 
    -- prefix common to both strings can be ignored
    WHILE (@start < @sLen AND SUBSTRING(@s, @start, 1) = SUBSTRING(@t, @start, 1)) 
        SELECT @start = @start + 1
    IF (@start > 1) BEGIN
        SELECT @sLen = @sLen - (@start - 1)
             , @tLen = @tLen - (@start - 1)
 
        -- if all of shorter string matches prefix and/or suffix of longer string, then
        -- edit distance is just the delete of additional characters present in longer string
        IF (@sLen <= 0) RETURN @tLen
 
        SELECT @s = SUBSTRING(@s, @start, @sLen)
             , @t = SUBSTRING(@t, @start, @tLen)
    END
 
    -- initialize v0 array of distances
    SELECT @v0 = '', @j = 1
    WHILE (@j <= @tLen) BEGIN
        SELECT @v0 = @v0 + NCHAR(CASE WHEN @j > @max THEN @max ELSE @j END)
        SELECT @j = @j + 1
    END
     
    SELECT @v2 = @v0 -- copy...doesn't matter what's in v2, just need to initialize its size
         , @jOffset = @max - @lenDiff
         , @i = 1
    WHILE (@i <= @sLen) BEGIN
        SELECT @distance = @i
             , @diag = @i - 1
             , @sChar = SUBSTRING(@s, @i, 1)
             -- no need to look beyond window of upper left diagonal (@i) + @max cells
             -- and the lower right diagonal (@i - @lenDiff) - @max cells
             , @j = CASE WHEN @i <= @jOffset THEN 1 ELSE @i - @jOffset END
             , @jEnd = CASE WHEN @i + @max >= @tLen THEN @tLen ELSE @i + @max END
             , @thisTransCost = 0
        WHILE (@j <= @jEnd) BEGIN
            -- at this point, @distance holds the previous value (the cell above if we were using an m by n matrix)
            SELECT @nextTransCost = UNICODE(SUBSTRING(@v2, @j, 1))
                 , @v2 = STUFF(@v2, @j, 1, NCHAR(@diag))
                 , @tChar = SUBSTRING(@t, @j, 1)
                 , @left = UNICODE(SUBSTRING(@v0, @j, 1))
                 , @thisJ = @j
            SELECT @distance = CASE WHEN @diag < @left AND @diag < @distance THEN @diag    --substitution
                                    WHEN @left < @distance THEN @left                    -- insertion
                                    ELSE @distance                                        -- deletion
                                END
            SELECT @distance = CASE WHEN (@sChar = @tChar) THEN @diag    -- no change (characters match)
                                    WHEN @i <> 1 AND @j <> 1
                                        AND @tChar = SUBSTRING(@s, @i - 1, 1)
                                        AND @thisTransCost < @distance
                                        AND @sChar = SUBSTRING(@t, @j - 1, 1)
                                        THEN 1 + @thisTransCost        -- transposition
                                    ELSE 1 + @distance END
            SELECT @v0 = STUFF(@v0, @thisJ, 1, NCHAR(@distance))
                 , @diag = @left
                 , @thisTransCost = @nextTransCost
                 , @j = case when (@distance > @max) AND (@thisJ = @i + @lenDiff) then @jEnd + 2 else @thisJ + 1 end
        END
        SELECT @i = CASE WHEN @j > @jEnd + 1 THEN @sLen + 1 ELSE @i + 1 END
    END
    RETURN CASE WHEN @distance <= @max THEN @distance ELSE NULL END
END
GO
/****** Object:  UserDefinedFunction [dbo].[Metaphone]    Script Date: 02/18/2018 07:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[Metaphone]
/**
summary:   >
The Metaphone  phonetic algorithm was devised by Lawrence Philips in 1990.
It reduces words to their basic sounds, but produces a more accurate encoding,
than Soundex for matching words that sound similar. 
Metaphone is a built-in operator in a number of systems such as PHP but there
seemed to be no available SQL Version until I wrote this. It is merely
a reverse engineering of the original published algorithm but tweaked to ensure
that it gave the same result.

example: >
	Select dbo.Metaphone ('opportunities')
	--OPRTNTS
Parameters: 
	-- @String (a word -all punctuation will be stripped out)
  A string representing the Metaphone equivalent of the word. 
**/  
(
	@String VARCHAR(30)
)
RETURNS VARCHAR(10)
AS
BEGIN
DECLARE  @New BIT, @ii INT, @Metaphone VARCHAR(28), @Len INT, @where INT;
DECLARE @This CHAR, @Next CHAR, @Following CHAR, @Previous CHAR, @silent BIT;
 
SELECT @String = UPPER(LTRIM(COALESCE(@String, ''))); --trim and upper case
SELECT @where= PATINDEX ('%[^A-Z]%',@String COLLATE Latin1_General_CI_AI ) 
WHILE  @where>0 --strip out all non-alphabetic characters!(Edited. Thanks Ros Presser) 
	BEGIN
	SELECT @String=STUFF(@string,@where,1,'')
	SELECT @where=PATINDEX ('%[^A-Z]%',@String COLLATE Latin1_General_CI_AI ) 
    END
IF(LEN(@String) < 2) RETURN  @String
 
--do the start of string stuff first.
--If the word begins with 'KN', 'GN', 'PN', 'AE', 'WR', drop the first letter.
-- "Aebersold", "Gnagy", "Knuth", "Pniewski", "Wright"
IF SUBSTRING(@String, 1, 2) IN ( 'KN', 'GN', 'PN', 'AE', 'WR' )
  SELECT @String = STUFF(@String, 1, 1, '');
-- Beginning of word: "x" change to "s" as in "Deng Xiaopeng"
IF SUBSTRING(@String, 1, 1) = 'X'
  SELECT @String = STUFF(@String, 1, 1, 'S');
-- Beginning of word: "wh-" change to "w" as in "Whatsoever"
IF @String LIKE 'WH%'
  SELECT @String = STUFF(@String, 1, 1, 'W');
-- Set up for While loop 
SELECT @Len = LEN(@String), @Metaphone = '', -- Initialize the main variable 
  @New = 1, -- this variable only used next 10 lines!!! 
  @ii = 1; --Position counter
--
WHILE((LEN(@Metaphone) <= 8) AND (@ii <= @Len))
  BEGIN --SET up the 'pointers' for this loop-around }
  SELECT @Previous =
    CASE WHEN @ii > 1 THEN SUBSTRING(@String, @ii - 1, 1) ELSE '' END,
    -- originally a nul terminated string }
    @This = SUBSTRING(@String, @ii, 1),
    @Next =
      CASE WHEN @ii < @Len THEN SUBSTRING(@String, @ii + 1, 1) ELSE '' END,
    @Following =
      CASE WHEN((@ii + 1) < @Len) THEN SUBSTRING(@String, @ii + 2, 1) ELSE
                                                                        '' END
   -- 'CC' inside word 
  --SELECT @Previous,@this,@Next,@Following,@New,@ii,@Len,@Metaphone
  /* Drop duplicate adjacent letters, except for C.*/
  IF @This=@Previous AND @This<> 'C' 
	BEGIN
	--we do nothing 
	SELECT @New=0
    END
  /*Drop all vowels unless it is the beginning.*/
  ELSE IF @This IN ( 'A', 'E', 'I', 'O', 'U' )
    BEGIN
    IF @ii = 1 --vowel at the beginning
      SELECT @Metaphone = @This;
    /* B -> B unless at the end of word after "m", as in "dumb", "Comb" */
    END;
  ELSE IF @This = 'B' AND NOT ((@ii = @Len) AND (@Previous = 'M'))
         BEGIN
         SELECT @Metaphone = @Metaphone + 'B';
         END;
         -- -mb is silent 
 /*'C' transforms to 'X' if followed by 'IA' or 'H' (unless in latter case, it is part of '-SCH-',
 in which case it transforms to 'K'). 'C' transforms to 'S' if followed by 'I', 'E', or 'Y'. 
 Otherwise, 'C' transforms to 'K'.*/
  ELSE IF @This = 'C'
         BEGIN -- -sce, i, y = silent 
         IF NOT (@Previous= 'S') AND (@Next IN ( 'H', 'E', 'I', 'Y' )) --front vowel set 
           BEGIN
			   IF(@Next = 'I') AND (@Following = 'A')
				 SELECT @Metaphone = @Metaphone + 'X'; -- -cia- 
			   ELSE IF(@Next IN ( 'E', 'I', 'Y' ))
				 SELECT @Metaphone = @Metaphone + 'S'; -- -ce, i, y = 'S' }
			   ELSE IF(@Next = 'H') AND (@Previous = 'S')
				 SELECT @Metaphone = @Metaphone + 'K'; -- -sch- = 'K' }
			   ELSE IF(@Next = 'H')
				 BEGIN
				   IF(@ii = 1) AND ((@ii + 2) <= @Len) 
					 AND NOT(@Following IN ( 'A', 'E', 'I', 'O', 'U' ))
					   SELECT @Metaphone = @Metaphone + 'K';
				   ELSE
					 SELECT @Metaphone = @Metaphone + 'X';
				   END
           End  
		 ELSE 
           SELECT @Metaphone = @Metaphone +CASE WHEN @Previous= 'S' THEN '' else 'K' end;
         	   -- Else silent 
         END; -- Case C }
  /*'D' transforms to 'J' if followed by 'GE', 'GY', or 'GI'. Otherwise, 'D' 
  transforms to 'T'.*/
  ELSE IF @This = 'D'
         BEGIN
         SELECT @Metaphone = @Metaphone
           + CASE WHEN(@Next = 'G') AND (@Following IN ( 'E', 'I', 'Y' )) --front vowel set 
                 THEN 'J' ELSE 'T' END;
         END;
  ELSE IF @This = 'G'
         /*Drop 'G' if followed by 'H' and 'H' is not at the end or before a vowel. Drop 'G' 
 if followed by 'N' or 'NED' and is at the end.
 'G' transforms to 'J' if before 'I', 'E', or 'Y', and it is not in 'GG'. 
 Otherwise, 'G' transforms to 'K'.*/
    BEGIN
  SELECT @silent = 
    CASE WHEN (@Next = 'H') AND (@Following IN ('A','E','I','O','U'))
	AND (@ii > 1) AND (((@ii+1) = @Len) OR ((@Next = 'n') AND
    (@Following = 'E') AND SUBSTRING(@String,@ii+3,1) = 'D') AND ((@ii+3) = @Len)) 
-- Terminal -gned 
  AND (@Previous = 'i') AND (@Next = 'n')
  THEN 1 
 -- if not start and near -end or -gned.) 
  WHEN (@ii > 1) AND (@Previous = 'D')-- gnuw
    AND (@Next IN ('E','I','Y')) --front vowel set 
  THEN 1 -- -dge, i, y 
  ELSE 0 END
  IF NOT (@silent=1)
    SELECT @Metaphone = @Metaphone 
	+ CASE WHEN (@Next IN ('E','I','Y')) --front vowel set 
      THEN  'J' ELSE  'K' END
  END
  /*Drop 'H' if after vowel and not before a vowel.
  or the second char of  "-ch-", "-sh-", "-ph-", "-th-", "-gh-"*/
 
  ELSE IF @This = 'H'
    BEGIN
    IF NOT ( (@ii= @Len) OR (@Previous IN  ( 'C', 'S', 'T', 'G' ))) 
	   AND (@Next IN ( 'A', 'E', 'I', 'O', 'U' ) )
     SELECT @Metaphone = @Metaphone + 'H';
         -- else silent (vowel follows) }
    END;
  ELSE IF @This IN --some get no substitution
       ( 'F', 'J', 'L', 'M', 'N', 'R' )
     BEGIN
     SELECT @Metaphone = @Metaphone + @This;
     END;
  /*'CK' transforms to 'K'.*/
  ELSE IF @This = 'K'
     BEGIN
     IF(@Previous <> 'C')
       SELECT @Metaphone = @Metaphone + 'K';
     END;
  /*'PH' transforms to 'F'.*/
  ELSE IF @This = 'P'
    BEGIN
      IF(@Next = 'H') SELECT @Metaphone = @Metaphone + 'F', @ii = @ii + 1;
      -- Skip the 'H' 
      ELSE
        SELECT @Metaphone = @Metaphone + 'P';
      END;
  /*'Q' transforms to 'K'.*/
  ELSE IF @This = 'Q'
    BEGIN
      SELECT @Metaphone = @Metaphone + 'K';
    END;
  /*'S' transforms to 'X' if followed by 'H', 'IO', or 'IA'.*/
  ELSE IF @This = 'S'
    BEGIN
    SELECT @Metaphone = @Metaphone + 
	  CASE 
		WHEN(@Next = 'H')
		 OR( (@ii> 1) AND (@Next = 'i') 
		  AND (@Following IN ( 'O', 'A' ) )
		  ) 
		THEN 'X' ELSE 'S' END;
     END;
  /*'T' transforms to 'X' if followed by 'IA' or 'IO'. 'TH' transforms 
to '0'. Drop 'T' if followed by 'CH'.*/
  ELSE IF @This = 'T'
    BEGIN
    SELECT @Metaphone = @Metaphone
      + CASE 
	    WHEN(@ii = 1) AND (@Next = 'H') AND (@Following = 'O') 
	       THEN 'T' -- Initial Tho- }
        WHEN(@ii > 1) AND (@Next = 'i') 
		     AND (@Following IN ( 'O', 'A' )) 
		  THEN 'X'
        WHEN(@Next = 'H') THEN '0'
        WHEN NOT((@Next = 'C') AND (@Following = 'H')) 
		  THEN 'T'
        ELSE '' END;
         -- -tch = silent }
    END;
  /*'V' transforms to 'F'.*/
  ELSE IF @This = 'V'
    BEGIN
    SELECT @Metaphone = @Metaphone + 'F';
    END;
  /*'WH' transforms to 'W' if at the beginning. Drop 'W' if not followed by a vowel.*/
  /*Drop 'Y' if not followed by a vowel.*/
  ELSE IF @This IN ( 'W', 'Y' )
    BEGIN
    IF @Next IN ( 'A', 'E', 'I', 'O', 'U' )
      SELECT @Metaphone = @Metaphone + @This;
     --else silent 
     /*'X' transforms to 'S' if at the beginning. Otherwise, 'X' transforms to 'KS'.*/
    END;
  ELSE IF @This = 'X'
    BEGIN
      SELECT @Metaphone = @Metaphone + 'KS';
    END;
  /*'Z' transforms to 'S'.*/
  ELSE IF @This = 'Z'
     BEGIN
       SELECT @Metaphone = @Metaphone + 'S';
     END;
  ELSE
  RETURN 'error with '''+ @This+ '''';
  -- end
  SELECT @ii = @ii + 1;
  END; -- While 
return @Metaphone 
END
GO
/****** Object:  UserDefinedFunction [dbo].[SplitByComma]    Script Date: 02/18/2018 07:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SplitByComma](@input AS NVARCHAR(4000))
RETURNS
      @Result TABLE(Value nvarchar(MAX))
AS
BEGIN
      DECLARE @str NVARCHAR(MAX)
      DECLARE @ind Int

if right(rtrim(@input),1) = ',' 
begin
set @input = (select substring(rtrim(@input),1,len(rtrim(@input))-1))
end

      IF(@input is not null)
      BEGIN
            SET @ind = CharIndex(',',@input)
            WHILE @ind > 0
            BEGIN
                  SET @str = SUBSTRING(@input,1,@ind-1)
                  SET @input = SUBSTRING(@input,@ind+1,LEN(@input)-@ind)
                  INSERT INTO @Result values (@str)
                  SET @ind = CharIndex(',',@input)
            END
            SET @str = @input
            INSERT INTO @Result values (@str)
      END
      RETURN
END
GO

/****** Object:  UserDefinedFunction [dbo].[OneEditDifferenceTo]    Script Date: 02/18/2018 07:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[OneEditDifferenceTo](@Word Varchar(40))
   /**
  summary:  >
  Returns all common words that are one edit difference away from the input word. The routine generates all the character strings that are one edit distance away and does an inner join with the table of words.
  Select * from dbo.OneEditDifferenceTo('erty')
  returns:  >
   a table containing strings
  **/
  RETURNS @candidates TABLE 
  (
  Candidate VARCHAR(40)
  )
  AS
  -- body of the function
  BEGIN
  DECLARE @characters TABLE ([character] CHAR(1) PRIMARY KEY)
  DECLARE @numbers TABLE (number int PRIMARY KEY)
  INSERT INTO @characters([character])
  SELECT character FROM (VALUES 
    ('a'),('b'),('c'),('d'),('e'),('f'),('g'),('h'),('i'),('j'),('k'),('l'),('m'),
    ('n'),('o'),('p'),('q'),('r'),('s'),('t'),('u'),('v'),('w'),('x'),('y'),('z'))F(character)
  INSERT INTO @numbers(number)
    SELECT number FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),
      (14),(15),(16),(17),(18),(19),(20),(21),(22),(23),(24),(25),(26),(27),(28),(29),
      (30),(31),(32),(33),(34),(35),(36),(37),(38),(39),(40))F(number)
  INSERT INTO @Candidates(candidate)
    SELECT DISTINCT Expertise FROM 
      (--deletes
      SELECT  STUFF(@word,number,1,'') FROM @numbers WHERE number <=LEN(@Word)
      UNION ALL--transposes
      SELECT  STUFF(@word,number,2,SUBSTRING(@word,number+1,1)+SUBSTRING(@word,number,1)) 
       FROM @numbers WHERE number <LEN(@Word)
      UNION ALL --replaces
  	SELECT DISTINCT STUFF(@word,number,1,character) 
  	  FROM @numbers CROSS JOIN @characters WHERE number <=LEN(@Word)
      UNION ALL --inserts 
  	SELECT STUFF(@word,number,0,character) 
  	  FROM @numbers CROSS JOIN @characters WHERE number <=LEN(@Word)
      UNION ALL --inserts at end of string
  	SELECT @word+character 
  	  FROM @characters 
  	)allcombinations(generatedWord)
      INNER JOIN dbo.DoctorExpertise ON generatedWord=Expertise
  	WHERE DoctorExpertise.CanonicalVersion IS null
     RETURN
  END
GO
/****** Object:  UserDefinedFunction [dbo].[FuzzySearchOf]    Script Date: 02/18/2018 07:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[FuzzySearchOf](@Searchterm VARCHAR(40))
  /**
  summary:  >
  Returns all candidate words even if the input word is misspelt
  example:
  Select * from dbo.FuzzySearchOf('sossyjez')
  returns:  >
   a table containing words
  Dependency: 
  dbo.OneEditDifferenceTo(@word)
  dbo.metaphone(@searchterm)
  **/
  RETURNS @candidates TABLE(Candidate VARCHAR(40))
  AS
    -- body of the function
    BEGIN
    DECLARE @Rowcount INT;
    /* The first stage is to see if a word is an alias or a known misspelling.*/
    INSERT INTO @candidates  (Candidate)
      SELECT COALESCE(DoctorExpertise.CanonicalVersion, DoctorExpertise.Expertise)  FROM dbo.DoctorExpertise
        WHERE DoctorExpertise.Expertise = @Searchterm;
    -- If not a known word or an alias, then has it an edit-distance of one to any canonical words 
    -- IN the 'Words' table
    IF @@RowCount = 0
      BEGIN
      INSERT INTO @candidates  (Candidate)
        SELECT OneEditDifferenceTo.Candidate FROM dbo.OneEditDifferenceTo(@Searchterm);
      IF @@RowCount = 0
        BEGIN --If not then does it share a metaphone with any words in your table?
        INSERT INTO @candidates  (Candidate)
          SELECT COALESCE(DoctorExpertise.CanonicalVersion, DoctorExpertise.Expertise) AS candidate
          FROM dbo.DoctorExpertise  WHERE DoctorExpertise.Metaphone = dbo.Metaphone(@Searchterm);
        SELECT @Rowcount = @@RowCount;
        IF @Rowcount > 5 --If yes, and there are too many, then get what there are and 
          BEGIN --take the top few in ascending edit difference.
          DELETE FROM @candidates;
          INSERT INTO @candidates (Candidate)
            SELECT TOP 5 COALESCE(DoctorExpertise.CanonicalVersion, DoctorExpertise.Expertise) AS candidate
            FROM dbo.DoctorExpertise  WHERE DoctorExpertise.Metaphone = dbo.Metaphone(@Searchterm)
            ORDER BY COALESCE(dbo.DamLev(DoctorExpertise.Expertise, @Searchterm, 3), 4); --just do three levels
          END;
        IF @Rowcount = 0
          BEGIN
          /* Get a limited number of words with an edit distance of two, using Steve Hatchett’s 
  		version  of the Damerau-Levenshtein Algorithm, specifying that it abandons its work 
  		on a particular word once it realises that it is more than two edit distances away*/
          INSERT INTO @candidates (Candidate)
            SELECT TOP 5 DoctorExpertise.Expertise
            FROM dbo.DoctorExpertise  WHERE DoctorExpertise.CanonicalVersion IS NULL
  			AND Expertise LIKE LEFT(@Searchterm,1)+'%'
              AND COALESCE(dbo.DamLev(DoctorExpertise.Expertise, @Searchterm, 2), 3) < 3;
          END;
        END;
      END;
    RETURN;
    END;
GO


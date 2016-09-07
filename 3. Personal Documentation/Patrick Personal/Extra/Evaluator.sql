DECLARE @ExpressionToSearch VARCHAR(200)
DECLARE  @ExpressionToFind VARCHAR(200)

IF OBJECT_ID('tempdb..#ParameterDetails') IS NOT NULL DROP TABLE #ParameterDetails

IF OBJECT_ID('tempdb..#XMLwithOpenXML') IS NOT NULL DROP TABLE #XMLwithOpenXML

CREATE TABLE #ParameterDetails(
	[PassValue] [nvarchar](5) NULL,
	[ParameterName] [nvarchar](100) NULL,
	[ClientParameterName] [nvarchar](50) NULL,
	[BestPracticeValue] [nvarchar](100) NULL,
	[IssueType] [nvarchar](50) NULL,
	[IssueSeverity] [nvarchar](15) NULL,
	[Problem] [nvarchar](max) NULL,
	[Recommendation] [nvarchar](max) NULL,
	[Why] [nvarchar](max) NULL,
	[Link] [nvarchar](max) NULL,
	[Link2] [nvarchar](max) NULL
)

CREATE TABLE #XMLwithOpenXML(
	Id INT IDENTITY PRIMARY KEY,
	XMLData XML,
	LoadedDateTime DATETIME
)


INSERT INTO #XMLwithOpenXML(XMLData, LoadedDateTime)
SELECT CONVERT(XML, BulkColumn) AS BulkColumn, GETDATE() 
FROM OPENROWSET(BULK 'C:\XML\SQLServer.xml', SINGLE_BLOB) AS x;


DECLARE @xml xml
SELECT @xml = XMLData FROM #XMLwithOpenXML


-- SQL Server Instance Installation Directory

DECLARE @InstallationDirectory nvarchar(MAX)

SELECT @InstallationDirectory = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Installation Directory'


SET @ExpressionToFind = 'Not System Drive'

SELECT @ExpressionToSearch = @InstallationDirectory

IF @ExpressionToSearch = @ExpressionToFind
    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2])  
    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'SQL Server Instance Installation Directory'
ELSE
	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2]) 
    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'SQL Server Instance Installation Directory'
    


-- SQL Server Version and Service Pack

DECLARE @ProductVersion nvarchar(20)
DECLARE @SQLName nvarchar(50)

SELECT @ProductVersion = doc.col.value('Text[1]', 'nvarchar(50)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(100)') = 'ProductVersion'

IF (LEFT(@ProductVersion ,2) = '10')
   SET @SQLName = 'Microsoft SQL Server 2008 R2'
ELSE IF (LEFT(@ProductVersion ,2) = '11')
   SET @SQLName = 'Microsoft SQL Server 2012'

IF (Select COUNT(1) from [EVALUATOR].[dbo].[ServicePack]WHERE [LatestServicePackValue] != @ProductVersion and [SQLServerVersion] = @SQLName) > 0 
    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2])  
    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'SQL Server Version and Service Pack'
ELSE
	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2]) 
    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'SQL Server Version and Service Pack'
								
								
-- Max Degree Of Parallelism	

DECLARE @MaxDegree nvarchar(2)

SELECT @MaxDegree = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Max Degree Of Parallelism'


SET @ExpressionToFind = 'Not Default'

SELECT @ExpressionToSearch = @MaxDegree

IF @ExpressionToSearch = @ExpressionToFind
    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2])  
    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Max Degree Of Parallelism'
ELSE
	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2]) 
    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Max Degree Of Parallelism'	 
									 
SELECT * from #ParameterDetails
DECLARE @ExpressionToSearch VARCHAR(200)
DECLARE  @ExpressionToFind VARCHAR(200)

IF OBJECT_ID('tempdb..#ParameterDetails') IS NOT NULL DROP TABLE #ParameterDetails

IF OBJECT_ID('tempdb..#ServerDetails') IS NOT NULL DROP TABLE #ServerDetails

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


CREATE TABLE #ServerDetails( 
	[ParameterName] [nvarchar](100) NULL,
	[ParameterDetails] [nvarchar](200) NULL
)



IF OBJECT_ID('tempdb..#XMLwithOpenXML') IS NOT NULL DROP TABLE #XMLwithOpenXML

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



INSERT INTO [#ServerDetails] ([ParameterName],[ParameterDetails])  
    SELECT 'Host Name :', doc.col.value('Text[1]', 'nvarchar(MAX)') FROM @xml.nodes('/NewDataSet/Table') doc(col)
	WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'HostName'

INSERT INTO [#ServerDetails] ([ParameterName],[ParameterDetails])  
    SELECT 'SQL Instance Name :', doc.col.value('Text[1]', 'nvarchar(MAX)') FROM @xml.nodes('/NewDataSet/Table') doc(col)
	WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'InstanceName'

INSERT INTO [#ServerDetails] ([ParameterName],[ParameterDetails])  
    SELECT 'SQL Version :', doc.col.value('Text[1]', 'nvarchar(MAX)') FROM @xml.nodes('/NewDataSet/Table') doc(col)
	WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'SQLVersion'

INSERT INTO [#ServerDetails] ([ParameterName],[ParameterDetails])  
    SELECT 'SQL Product Level :', doc.col.value('Text[1]', 'nvarchar(MAX)') FROM @xml.nodes('/NewDataSet/Table') doc(col)
	WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'ProductLevel'




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

IF (Select COUNT(1) from [EVALUATOR].[dbo].[ServicePack] WHERE [LatestServicePackValue] != @ProductVersion and [SQLServerVersion] = @SQLName) > 0 
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
									 
									 
-- Memory Min Memory

DECLARE @MinMemory nvarchar(2)

SELECT @MinMemory = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Minimum size of server memory (MB)'


SET @ExpressionToFind = '0'

SELECT @ExpressionToSearch = @MinMemory

IF @ExpressionToSearch != @ExpressionToFind
    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2])  
    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Memory - Min Memory'
ELSE
	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2]) 
    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Memory - Min Memory'


-- Memory Max Memory

DECLARE @MaxMemory nvarchar(20)

SELECT @MaxMemory = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Maximum size of server memory (MB)'


SET @ExpressionToFind = '2147483647'

SELECT @ExpressionToSearch = @MaxMemory

IF @ExpressionToSearch = @ExpressionToFind
    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2])  
    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Memory - Max Memory'
ELSE
	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2]) 
    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Memory - Max Memory'
									 								 	 
									 								 	 
																		 
-- Trace flag 2371

DECLARE @Flag2371 nvarchar(2)

SELECT @Flag2371 = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Trace flag 2371'


SET @ExpressionToFind = '1'

SELECT @ExpressionToSearch = @Flag2371

IF @ExpressionToSearch = @ExpressionToFind
    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2])  
    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Trace flag 2371'
ELSE
	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2]) 
    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Trace flag 2371'
									 
									 
-- Trace flag 1117

DECLARE @Flag1117 nvarchar(2)

SELECT @Flag1117 = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Trace flag 1117'


SET @ExpressionToFind = '1'

SELECT @ExpressionToSearch = @Flag1117

IF @ExpressionToSearch = @ExpressionToFind
    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2])  
    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Trace flag 1117'
ELSE
	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2]) 
    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Trace flag 1117'
									 
									 
-- Trace flag 1118

DECLARE @Flag1118 nvarchar(2)

SELECT @Flag1118 = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Trace flag 1118'


SET @ExpressionToFind = '1'

SELECT @ExpressionToSearch = @Flag1118

IF @ExpressionToSearch = @ExpressionToFind
    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2])  
    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Trace flag 1118'
ELSE
	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2]) 
    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Trace flag 1118'
									 
									 
-- Default index fill factor

DECLARE @FillFactor nvarchar(2)

SELECT @FillFactor = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Fill Factor Values in (%)'


SET @ExpressionToFind = '0'

SELECT @ExpressionToSearch = @FillFactor

IF @ExpressionToSearch != @ExpressionToFind
    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2])  
    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Default index fill factor'
ELSE
	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2]) 
    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Default index fill factor'
									 
									 				 
									 
-- Server authentication

DECLARE @SQLAuth nvarchar(100)

SELECT @SQLAuth = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'SQL Server Authentication Mode'


SET @ExpressionToFind = 'Windows Authentication'

SELECT @ExpressionToSearch = @SQLAuth

IF @ExpressionToSearch = @ExpressionToFind
    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2])  
    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Server authentication'
ELSE
	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2]) 
    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'Server authentication'
									 
					
									 
-- SQL Server Network Port

DECLARE @SQLPort nvarchar(10)

SELECT @SQLPort = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'SQL Port'


SET @ExpressionToFind = '1433'

SELECT @ExpressionToSearch = @SQLPort

IF @ExpressionToSearch = @ExpressionToFind
    INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2])  
    SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'SQL Server Network Port'
ELSE
	INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2]) 
    SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
									 [Problem],[Recommendation],[Why],[Link],[Link2] 
									 from [EVALUATOR].[dbo].[ParameterDesc] 
									 WHERE ParameterName = 'SQL Server Network Port'
									 
									 
										 
-- Datafile Location

DECLARE @SQLData nvarchar(20)

SELECT @SQLData = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Datafile Location'

IF @SQLData IS NOT NULL

BEGIN

		SET @ExpressionToFind = 'Not System Drive'

		SELECT @ExpressionToSearch = @SQLData

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Database Data File Configuration'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Database Data File Configuration'
									 
END								 
-- Logfile Location

DECLARE @SQLLog nvarchar(20)

SELECT @SQLLog = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Logfile Location'

IF @SQLLog IS NOT NULL 

BEGIN 

		SET @ExpressionToFind = 'Not System Drive'

		SELECT @ExpressionToSearch = @SQLLog

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Database Log File Configuration'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Database Log File Configuration'

END									 
									 
-- Recovery Model

DECLARE @DBRecovery nvarchar(10)

SELECT @DBRecovery = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Recovery Model'

IF @DBRecovery IS NOT NULL

BEGIN

		SET @ExpressionToFind = 'FULL'

		SELECT @ExpressionToSearch = @DBRecovery

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Recovery Model'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Recovery Model'
									 
END 									 

-- Compatibility Level

DECLARE @DBComp nvarchar(10)

SELECT @DBComp = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Compatibility Level'

IF @DBComp IS NOT NULL 

BEGIN 

		SET @ExpressionToFind = '100 or 110'

		SELECT @ExpressionToSearch = @DBComp

		IF @ExpressionToSearch != @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Compatibility Level'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Compatibility Level'
									 
END 									 


-- Snapshot Isolation

DECLARE @DBIso nvarchar(10)

SELECT @DBIso = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Snapshot Isolation'

IF @DBIso IS NOT NULL

BEGIN

		SET @ExpressionToFind = 'ON'

		SELECT @ExpressionToSearch = @DBIso

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Snapshot Isolation'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Snapshot Isolation'

END									 

										 
-- Read Committed Snapshot 

DECLARE @DBSnap nvarchar(20)

SELECT @DBSnap = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Snapshot Isolation'

IF @DBSnap IS NOT NULL

BEGIN

		SET @ExpressionToFind = '1'

		SELECT @ExpressionToSearch = @DBSnap

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Read Committed Snapshot'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Read Committed Snapshot'
									 
END									 
										 
-- Database Auto growth 

DECLARE @DBGrowth nvarchar(20)

SELECT @DBGrowth = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Datafile Growth'

IF @DBGrowth IS NOT NULL

BEGIN

		SET @ExpressionToFind = 'Fix in size'

		SELECT @ExpressionToSearch = @DBGrowth

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Database Auto growth'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Database Auto growth'
									 
END									 
										 
-- Auto Create Statistics

DECLARE @DBCreateStat nvarchar(2)

SELECT @DBCreateStat = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Auto Create Statistics'

IF @DBCreateStat IS NOT NULL

BEGIN

		SET @ExpressionToFind = '1'

		SELECT @ExpressionToSearch = @DBCreateStat

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Auto Create Statistics'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Auto Create Statistics'


END								 
									 
-- Auto Update Statistics

DECLARE @DBUpdateStat nvarchar(2)

SELECT @DBUpdateStat = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Auto Update Statistics'

IF @DBUpdateStat IS NOT NULL

BEGIN

		SET @ExpressionToFind = '1'

		SELECT @ExpressionToSearch = @DBUpdateStat

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Auto Update Statistics'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Auto Update Statistics'
									 
					
END 									 
-- Auto Shrink

DECLARE @DBAutoShrink nvarchar(2)

SELECT @DBAutoShrink = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Auto Shrink'

IF @DBAutoShrink IS NOT NULL

BEGIN

		SET @ExpressionToFind = '0'

		SELECT @ExpressionToSearch = @DBAutoShrink

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Auto Shrink'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Auto Shrink'
											 
END									 
									 
-- Daily Index Rebuild - Index Optimizations

DECLARE @DBIndex nvarchar(10)

SELECT @DBIndex = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Old Statistics Found'

IF @DBIndex IS NOT NULL

BEGIN

		SET @ExpressionToFind = '0'

		SELECT @ExpressionToSearch = @DBIndex

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Daily Index Rebuild - Index Optimizations'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Daily Index Rebuild - Index Optimizations'
									 
									 
END 									 
-- Daily database Full backupns

DECLARE @DBBackup nvarchar(10)

SELECT @DBBackup = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Last Full Backup'

IF @DBBackup IS NOT NULL

BEGIN

		SET @ExpressionToFind = '0'

		SELECT @ExpressionToSearch = @DBBackup

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Daily database Full backup'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Daily database Full backup'
									 
END									 
									 
-- Blank SQL SA Password

DECLARE @SAPwd nvarchar(50)

SELECT @SAPwd = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'SA Login'

IF @SAPwd IS NOT NULL

BEGIN

		SET @ExpressionToFind = 'Does not have a blank password'

		SELECT @ExpressionToSearch = @SAPwd

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Blank SQL SA Password'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'Blank SQL SA Password'
					
END									 
									 
-- NT AUTHORITY\SYSTEM Administrator

DECLARE @SYSTEMAuth nvarchar(50)

SELECT @SYSTEMAuth = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'NT AUTHORITY\SYSTEM'

IF @SYSTEMAuth IS NOT NULL

BEGIN

		SET @ExpressionToFind = 'Does not have a access'

		SELECT @ExpressionToSearch = @SYSTEMAuth

		IF @ExpressionToSearch = @ExpressionToFind
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2])  
			SELECT 'True',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'NT AUTHORITY\SYSTEM Administrator'
		ELSE
			INSERT INTO [#ParameterDetails] ([PassValue],[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2]) 
			SELECT 'False',[ParameterName],[ClientParameterName],[BestPracticeValue],[IssueType],[IssueSeverity],
											 [Problem],[Recommendation],[Why],[Link],[Link2] 
											 from [EVALUATOR].[dbo].[ParameterDesc] 
											 WHERE ParameterName = 'NT AUTHORITY\SYSTEM Administrator'
											 
									 
END	
									 
									 
									 
									 



									 
										 
										 
									 								 

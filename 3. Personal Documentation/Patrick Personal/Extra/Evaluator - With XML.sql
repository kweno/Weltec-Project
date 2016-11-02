DECLARE @ExpressionToSearch VARCHAR(200)
DECLARE  @ExpressionToFind VARCHAR(200)
DECLARE @xml XML

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



SET @xml = N'<NewDataSet>
  <Table>
    <ProcessInfo>HostName</ProcessInfo>
    <Text>WIN-5BUL46G8J35</Text>
  </Table>
  <Table>
    <ProcessInfo>InstanceName</ProcessInfo>
    <Text>SQL2008R2</Text>
  </Table>
  <Table>
    <ProcessInfo>Installation Directory</ProcessInfo>
    <Text>System Drive</Text>
  </Table>
  <Table>
    <ProcessInfo>SQLVersion</ProcessInfo>
    <Text>Microsoft SQL Server 2008 R2 (RTM) Enterprise Evaluation Edition (64-bit)</Text>
  </Table>
  <Table>
    <ProcessInfo>ProductLevel</ProcessInfo>
    <Text>RTM</Text>
  </Table>
  <Table>
    <ProcessInfo>ProductVersion</ProcessInfo>
    <Text>10.50.1600.1</Text>
  </Table>
  <Table>
    <ProcessInfo>Max Degree Of Parallelism</ProcessInfo>
    <Text>0</Text>
  </Table>
  <Table>
    <ProcessInfo>Minimum size of server memory (MB)</ProcessInfo>
    <Text>0</Text>
  </Table>
  <Table>
    <ProcessInfo>Maximum size of server memory (MB)</ProcessInfo>
    <Text>2147483647</Text>
  </Table>
  <Table>
    <ProcessInfo>Trace Flag 2371</ProcessInfo>
    <Text>1</Text>
  </Table>
  <Table>
    <ProcessInfo>Trace Flag 1117</ProcessInfo>
    <Text>0</Text>
  </Table>
  <Table>
    <ProcessInfo>Trace Flag 1118</ProcessInfo>
    <Text>0</Text>
  </Table>
  <Table>
    <ProcessInfo>Fill Factor Values in (%)</ProcessInfo>
    <Text>0</Text>
  </Table>
  <Table>
    <ProcessInfo>SQL Server Authentication Mode</ProcessInfo>
    <Text>Windows and SQL Server Authentication</Text>
  </Table>
  <Table>
    <ProcessInfo>SQL Port</ProcessInfo>
    <Text>1433</Text>
  </Table>
  <Table>
    <ProcessInfo>Datafile Location</ProcessInfo>
    <Text>System Drive</Text>
  </Table>
  <Table>
    <ProcessInfo>Logfile Location</ProcessInfo>
    <Text>System Drive</Text>
  </Table>
  <Table>
    <ProcessInfo>Recovery Model</ProcessInfo>
    <Text>SIMPLE</Text>
  </Table>
  <Table>
    <ProcessInfo>Compatibility Level</ProcessInfo>
    <Text>100</Text>
  </Table>
  <Table>
    <ProcessInfo>Snapshot Isolation</ProcessInfo>
    <Text>OFF</Text>
  </Table>
  <Table>
    <ProcessInfo>Read Committed Snapshot Isolation</ProcessInfo>
    <Text>0</Text>
  </Table>
  <Table>
    <ProcessInfo>Datafile Growth</ProcessInfo>
    <Text>Fix in size</Text>
  </Table>
  <Table>
    <ProcessInfo>Auto Create Statistics</ProcessInfo>
    <Text>1</Text>
  </Table>
  <Table>
    <ProcessInfo>Auto Update Statistics</ProcessInfo>
    <Text>1</Text>
  </Table>
  <Table>
    <ProcessInfo>Auto Shrink</ProcessInfo>
    <Text>0</Text>
  </Table>
  <Table>
    <ProcessInfo>Old Statistics Found</ProcessInfo>
    <Text>2357</Text>
  </Table>
  <Table>
    <ProcessInfo>Last Full Backup</ProcessInfo>
    <Text>6</Text>
  </Table>
  <Table>
    <ProcessInfo>SA Login</ProcessInfo>
    <Text>Does not have a blank password</Text>
  </Table>
  <Table>
    <ProcessInfo>NT AUTHORITY\SYSTEM</ProcessInfo>
    <Text>Does have a access</Text>
  </Table>
</NewDataSet>'



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


SELECT * FROM [#ServerDetails]

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


SET @ExpressionToFind = 'Windows'

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
									 
									 
-- Logfile Location

DECLARE @SQLLog nvarchar(20)

SELECT @SQLLog = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Logfile Location'


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
									 
									 
-- Recovery Model

DECLARE @DBRecovery nvarchar(10)

SELECT @DBRecovery = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Recovery Model'


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
									 
									 

-- Compatibility Level

DECLARE @DBComp nvarchar(10)

SELECT @DBComp = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Compatibility Level'


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
									 
									 


-- Snapshot Isolation

DECLARE @DBIso nvarchar(10)

SELECT @DBIso = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Snapshot Isolation'


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
									 

										 
-- Read Committed Snapshot 

DECLARE @DBSnap nvarchar(20)

SELECT @DBSnap = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Snapshot Isolation'


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
									 
									 
										 
-- Database Auto growth 

DECLARE @DBGrowth nvarchar(20)

SELECT @DBGrowth = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Datafile Growth'


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
									 
									 
										 
-- Auto Create Statistics

DECLARE @DBCreateStat nvarchar(2)

SELECT @DBCreateStat = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Auto Create Statistics'


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
									 
									 
-- Auto Update Statistics

DECLARE @DBUpdateStat nvarchar(2)

SELECT @DBUpdateStat = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Auto Update Statistics'


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
									 
					
									 
-- Auto Shrink

DECLARE @DBAutoShrink nvarchar(2)

SELECT @DBAutoShrink = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Auto Shrink'


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
									 
									 
									 
-- Daily Index Rebuild - Index Optimizations

DECLARE @DBIndex nvarchar(10)

SELECT @DBIndex = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Old Statistics Found'


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
									 
									 
									 
-- Daily database Full backupns

DECLARE @DBBackup nvarchar(10)

SELECT @DBBackup = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'Last Full Backup'


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
									 
									 
									 
-- Blank SQL SA Password

DECLARE @SAPwd nvarchar(50)

SELECT @SAPwd = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'SA Login'


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
					
									 
									 
-- NT AUTHORITY\SYSTEM Administrator

DECLARE @SYSTEMAuth nvarchar(50)

SELECT @SYSTEMAuth = doc.col.value('Text[1]', 'nvarchar(MAX)') 
FROM @xml.nodes('/NewDataSet/Table') doc(col)
WHERE doc.col.value('ProcessInfo[1]', 'varchar(MAX)') = 'NT AUTHORITY\SYSTEM'


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
									 
									 
									 
									 



									 
										 
										 
									 								 
SELECT * from #ParameterDetails ORDER BY PassValue,IssueSeverity, ParameterName
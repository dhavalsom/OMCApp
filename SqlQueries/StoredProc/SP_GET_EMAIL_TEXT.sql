USE [HealthCare]
GO

/****** Object:  StoredProcedure [dbo].[SP_GET_EMAIL_TEXT]    Script Date: 2/1/2018 9:48:55 PM ******/
DROP PROCEDURE [dbo].[SP_GET_EMAIL_TEXT]
GO

/****** Object:  StoredProcedure [dbo].[SP_GET_EMAIL_TEXT]    Script Date: 2/1/2018 9:48:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_GET_EMAIL_TEXT]
(	
	@EMAIL_TYPE NVARCHAR(200)
)
AS

BEGIN

--EXEC [SP_GET_EMAIL_TEXT] 'GET_ACCESS_CODE'
	SELECT *,
	(SELECT SettingValue FROM [SystemSettings] WHERE [SettingName]='SenderEmailAddress') AS 'SenderEmailAddress',
	(SELECT SettingValue FROM [SystemSettings] WHERE [SettingName]='SenderEmailAddressPassword') AS 'SenderEmailAddressPassword'
	FROM [EmailMaster]
	WHERE EmailType = @EMAIL_TYPE
END







GO



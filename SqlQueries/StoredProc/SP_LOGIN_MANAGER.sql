USE [HealthCare]
GO

/****** Object:  StoredProcedure [dbo].[SP_LOGIN_MANAGER]    Script Date: 1/30/2018 12:10:20 AM ******/
DROP PROCEDURE [dbo].[SP_LOGIN_MANAGER]
GO

/****** Object:  StoredProcedure [dbo].[SP_LOGIN_MANAGER]    Script Date: 1/30/2018 12:10:20 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[SP_LOGIN_MANAGER]
(
	@USERNAME NVARCHAR(100),
	@PASSWORD NVARCHAR(100),
	@IP_ADDRESS NVARCHAR(200),
	@ROLE_ID BIGINT = NULL
)
AS

BEGIN

--EXEC [dbo].[SP_LOGIN_MANAGER] '+919619645344','10dulkar', '101.120.222.557',2

DECLARE @USER_ID AS BIGINT, @USER_ROLE_ID AS BIGINT, @USER_DEVICE_ID AS BIGINT
,@TWO_WAY_AUTH_TIMEOUT_DAYS AS BIGINT
DECLARE @TWO_FACTOR_AUTH_TS AS DATETIME
DECLARE @TWO_FACTOR_AUTH_DONE AS BIT

	SELECT @USER_ID = UD.Id, @USER_ROLE_ID = URM.RoleId FROM UserDetail UD
	INNER JOIN UserRoleMapping URM on URM.UserId = UD.Id
		WHERE (EmailAddress = @USERNAME OR Phonenumber = @USERNAME)
		AND [Password] = @PASSWORD
		AND ((@ROLE_ID IS NULL AND URM.IsDefault = 1) OR @ROLE_ID = URM.RoleId)


/*IF USER IS FOUND WITH SAME USER ID AND PWD, GO AHEAD*/
IF @USER_ID IS NOT NULL
	
	BEGIN

		/*CHECK IF DEVICE IS THERE FOR THE USER OR NOT
		IF NOT MAKE AN ENTRY IN USER DEVICE DETAIL TABLE
		AND SET THE @TWO_FACTOR_AUTH_DONE AND @USER_DEVICE_ID*/

		IF NOT EXISTS (
			SELECT Id FROM DBO.UserDeviceDetail
				WHERE UserId = @USER_ID
				AND IpAddress = @IP_ADDRESS				
		)

		BEGIN

			INSERT INTO [dbo].[UserDeviceDetail]
			   ([UserId]
			   ,[IpAddress]
			   ,[TwoFactorAuthDone]
			   ,[Active]
			   ,[AddedBy]
			   ,[AddedDate])
			VALUES
			   ( @USER_ID
			   , @IP_ADDRESS
			   , 0
			   , 1
			   , @USER_ID
			   , GETDATE())

			SET @USER_DEVICE_ID = @@IDENTITY
			SET @TWO_FACTOR_AUTH_DONE = 0
		END

		ELSE
			BEGIN
				SELECT @USER_DEVICE_ID = Id 
					, @TWO_FACTOR_AUTH_DONE = TwoFactorAuthDone
					, @TWO_FACTOR_AUTH_TS = TwoFactorAuthTimestamp
				FROM DBO.UserDeviceDetail
					WHERE UserId = @USER_ID
					AND IpAddress = @IP_ADDRESS		
			END

		/*CHECK IF TWO FACTOR AUTH IS EXPIRED*/

		IF @TWO_FACTOR_AUTH_DONE = 1 
		BEGIN
			SELECT @TWO_WAY_AUTH_TIMEOUT_DAYS = CONVERT(BIGINT, SettingValue)
			FROM SystemSettings
			WHERE SettingName= 'TwoFactorAuthTimeout'

			IF DATEDIFF(DAY,@TWO_FACTOR_AUTH_TS,GETDATE()) > @TWO_WAY_AUTH_TIMEOUT_DAYS
				BEGIN
					UPDATE UserDeviceDetail
					SET TwoFactorAuthDone = 0, TwoFactorAuthTimestamp = NULL
					WHERE Id = @USER_DEVICE_ID

					SET @TWO_FACTOR_AUTH_DONE = 0
				END
		END
	
		/*INSERT A RECORD IN THE USER LOGIN AUDIT TABLE*/


		INSERT INTO [dbo].[UserLoginAudit]
				   ([UserId]
				   ,[UserDeviceId]
				   ,[IsTwoWayAuthNeeded]
				   ,[AccessCode]
				   ,[IsTwoWayAuthPassed]
				   ,[AttemptCount]
				   ,[TwoFactorAuthTimestamp]
				   ,[SessionId]
				   ,[RoleId]
				   ,[Active]
				   ,[AddedBy]
				   ,[AddedDate])
			 VALUES
				   (@USER_ID
				   , @USER_DEVICE_ID
				   , CASE @TWO_FACTOR_AUTH_DONE WHEN 0 THEN 1 ELSE 0 END
				   , '123456'
				   , @TWO_FACTOR_AUTH_DONE
				   , 0
				   , @TWO_FACTOR_AUTH_TS
				   , NEWID()
				   , @USER_ROLE_ID
				   , 1
				   , @USER_ID
				   , GETDATE())



	END

SELECT @USER_ID AS ID, @USER_ROLE_ID AS RoleId, @TWO_FACTOR_AUTH_DONE AS TWO_FACTOR_AUTH_DONE
,@USER_DEVICE_ID AS USER_DEVICE_ID, @TWO_FACTOR_AUTH_TS AS TWO_FACTOR_AUTH_TS

END



GO



USE [HealthCare]
GO

/****** Object:  StoredProcedure [dbo].[SP_GET_LOGIN_DETAILS]    Script Date: 1/23/2018 9:08:59 PM ******/
DROP PROCEDURE [dbo].[SP_GET_LOGIN_DETAILS]
GO

/****** Object:  StoredProcedure [dbo].[SP_GET_LOGIN_DETAILS]    Script Date: 1/23/2018 9:08:59 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_GET_LOGIN_DETAILS]
(
	@USERNAME NVARCHAR(100),
	@PASSWORD NVARCHAR(100)
)
AS

BEGIN

--EXEC [dbo].[SP_GET_LOGIN_DETAILS] '+919619645344','10dulkar'
	SELECT UD.Id, URM.RoleId FROM UserDetail UD
	INNER JOIN UserRoleMapping URM on URM.UserId = UD.Id
		WHERE (EmailAddress = @USERNAME OR Phonenumber = @USERNAME)
		AND [Password] = @PASSWORD

END




GO



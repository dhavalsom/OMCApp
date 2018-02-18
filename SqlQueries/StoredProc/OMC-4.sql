
/****** Object:  StoredProcedure [dbo].[SP_ConsultDoctor]    Script Date: 02/18/2018 07:27:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[SP_ConsultDoctor] 
	-- Add the parameters for the stored procedure here
	@ProblemId bigint,
	@DoctorId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @lProblemId bigint = @ProblemId,
	@lDoctorId bigint = @DoctorId,
	@id bigint
	

    -- Insert statements for procedure here
	INSERT INTO [dbo].[ProblemsDoctorsMapping]
           ([ProblemId]
           ,[DoctoerId]
           ,[ReviewedByDoctor])
     VALUES
           (@lProblemId
           ,@lDoctorId
           ,0)
           
           set @id = SCOPE_IDENTITY()
           
           select case when @id Is not null then 1 else 0 end

END
GO

--****** Object:  StoredProcedure [dbo].[SP_GetDoctorsFromProblem]    Script Date: 02/18/2018 07:27:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ramanuj Shanishchara
-- Create date: 3rd February, 2018
-- Description:	Get appropriate doctors from problems
-- exec SP_GetDoctorsFromProblem @Problem = 'i am suffering from influenza and having diabetes', @IpAddress = '192.168.0.11'
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetDoctorsFromProblem]
	-- Add the parameters for the stored procedure here
	@Problem varchar(200),
	@IpAddress nvarchar(500)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- parameter sniffing
    declare @lProblem varchar(200) = @Problem,
			@lIpAddress nvarchar(500) = @IpAddress,
			@lProblemSearchTerm  varchar(500)= REPLACE(@Problem, ' ', ','),
			@PatientProblemId bigInt
			
INSERT INTO [dbo].[PatientProblems]
           ([PatientProblem]
           ,[ProblemUniqueId]
           ,[IsSolved]
           ,[PatientIP])
     VALUES
           (@lProblem
           ,newid()
           ,0
           ,@lIpAddress)
           
         set @PatientProblemId = SCOPE_IDENTITY()
			

select (isnull(U.FirstName,'') + ' ' + isnull(U.LastName,'')) as Doctor,
			U.Id as DoctorId
		,STUFF (( SELECT ', ' + UPPER( M.Expertise )
           		 FROM DoctorExpertise M
           		 WHERE M.DoctorId = U.Id
           		 ORDER BY Expertise
           		 FOR XML PATH('')), 1, 1, '' ) AS Expertise         ,@PatientProblemId as ProblemId from UserDetail U where U.Id in (		select abc.DoctorId from (		SELECT good, bad, candidate,DoctorId from (		SELECT DISTINCT Expertise as good,value as bad, DoctorId from DoctorExpertise w 
		OUTER APPLY dbo.[SplitByComma](@lProblemSearchTerm))f(good,bad,DoctorId) 
		OUTER APPLY dbo.FuzzySearchOf(bad) candidate ) abc where abc.good = abc.candidate
)


END
GO
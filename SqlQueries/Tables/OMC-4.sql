
/****** Object:  Table [dbo].[DoctorExpertise]    Script Date: 02/18/2018 07:26:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DoctorExpertise](
	[Expertise] [varchar](200) NOT NULL,
	[CanonicalVersion] [varchar](200) NULL,
	[Metaphone] [varchar](10) NULL,
	[Word_id] [int] IDENTITY(1,1) NOT NULL,
	[DoctorId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Expertise] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Word_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProblemsDoctorsMapping]    Script Date: 02/18/2018 07:26:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProblemsDoctorsMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProblemId] [int] NOT NULL,
	[DoctoerId] [int] NOT NULL,
	[ReviewedByDoctor] [bit] NULL,
 CONSTRAINT [PK_ProblemsDoctorsMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PatientProblems]    Script Date: 02/18/2018 07:26:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PatientProblems](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PatientProblem] [varchar](2000) NOT NULL,
	[ProblemUniqueId] [uniqueidentifier] NOT NULL,
	[IsSolved] [bit] NOT NULL,
	[PatientIP] [nvarchar](500) NULL,
 CONSTRAINT [PK_PatientProblems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO


/****** Object:  ForeignKey [FK_canonical]    Script Date: 02/18/2018 07:26:47 ******/
ALTER TABLE [dbo].[DoctorExpertise]  WITH CHECK ADD  CONSTRAINT [FK_canonical] FOREIGN KEY([CanonicalVersion])
REFERENCES [dbo].[DoctorExpertise] ([Expertise])
GO
ALTER TABLE [dbo].[DoctorExpertise] CHECK CONSTRAINT [FK_canonical]
GO
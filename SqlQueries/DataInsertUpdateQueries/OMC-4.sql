/****** Object:  Table [dbo].[UserDetail]    Script Date: 02/18/2018 08:05:16 ******/
SET IDENTITY_INSERT [dbo].[UserDetail] ON

INSERT [dbo].[UserDetail] ([Id], [FirstName], [LastName], [EmailAddress], [Address], [PhoneNumber], [Gender], [DOB], [Password], [AlternateNo], [EmergencyContactNo], [EmergencyContactPerson], [DLNumber], [SSN], [Active], [AddedBy], [AddedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate]) VALUES (3, N'John', N'Smith', N'johnsmith@gmail.com', NULL, N'1234567980', 1, N'1/14/1980', N'tryOnce', N'9874563210', N'1839257456', N'Smith', N'1234567890', N'1234SD1222', 1, 1, CAST(0x07600AC89EC4D43D0B0000 AS DateTimeOffset), NULL, NULL, NULL, NULL)
INSERT [dbo].[UserDetail] ([Id], [FirstName], [LastName], [EmailAddress], [Address], [PhoneNumber], [Gender], [DOB], [Password], [AlternateNo], [EmergencyContactNo], [EmergencyContactPerson], [DLNumber], [SSN], [Active], [AddedBy], [AddedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate]) VALUES (4, N'David', N'Root', N'Davidroot@gmail.com', NULL, N'1267980345', 1, N'1/14/1982', N'onceTry', N'1839257456', N'9874563210', N'Davidroot', N'1234567890', N'1234SD1222', 1, 1, CAST(0x07600AC89EC4D43D0B0000 AS DateTimeOffset), NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[UserDetail] OFF
/****** Object:  Table [dbo].[DoctorExpertise]    Script Date: 02/18/2018 08:05:16 ******/
SET IDENTITY_INSERT [dbo].[DoctorExpertise] ON
INSERT [dbo].[DoctorExpertise] ([Expertise], [CanonicalVersion], [Metaphone], [Word_id], [DoctorId]) VALUES (N'diabetes', NULL, N'TBTS', 1, 3)
INSERT [dbo].[DoctorExpertise] ([Expertise], [CanonicalVersion], [Metaphone], [Word_id], [DoctorId]) VALUES (N'influenza', NULL, N'INFLNS', 5, 4)
INSERT [dbo].[DoctorExpertise] ([Expertise], [CanonicalVersion], [Metaphone], [Word_id], [DoctorId]) VALUES (N'physician', NULL, N'FSXN', 2, 3)
SET IDENTITY_INSERT [dbo].[DoctorExpertise] OFF
/****** Object:  Table [dbo].[UserRoleMapping]    Script Date: 02/18/2018 08:05:16 ******/
SET IDENTITY_INSERT [dbo].[UserRoleMapping] ON
INSERT [dbo].[UserRoleMapping] ([Id], [UserId], [RoleId], [IsDefault], [Active], [AddedBy], [AddedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate]) VALUES (1, 2, 1, NULL, 1, 1, CAST(0x07600AC89EC4D43D0B0000 AS DateTimeOffset), NULL, NULL, NULL, NULL)
INSERT [dbo].[UserRoleMapping] ([Id], [UserId], [RoleId], [IsDefault], [Active], [AddedBy], [AddedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate]) VALUES (2, 3, 4, NULL, 1, 1, CAST(0x07600AC89EC4D43D0B0000 AS DateTimeOffset), NULL, NULL, NULL, NULL)
INSERT [dbo].[UserRoleMapping] ([Id], [UserId], [RoleId], [IsDefault], [Active], [AddedBy], [AddedDate], [ModifiedBy], [ModifiedDate], [DeletedBy], [DeletedDate]) VALUES (4, 4, 4, NULL, 1, 1, CAST(0x07600AC89EC4D43D0B0000 AS DateTimeOffset), NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[UserRoleMapping] OFF

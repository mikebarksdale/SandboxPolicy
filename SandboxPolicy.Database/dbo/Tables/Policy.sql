create table [Policy] (
	[PolicyId] [int] identity(1,1) not null,
	[Mod] [int] not null,
	[Status] [varchar](30) not null,
	[PolicyNumber] [varchar](50) not null,
	[Description] [varchar](140) null,
	[EffectiveDate] [datetime] not null,
	[ExpirationDate] [datetime] null,
	[TransactionId] [int] not null,
	primary key ([PolicyId]),
	foreign key ([TransactionId]) references [Transaction]([TransactionId])
);
GO
create nonclustered index [PolicyNumberIdx] on [Policy]([PolicyNumber] asc);
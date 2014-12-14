create table [Insured] (
	[InsuredId] [int] identity(1,1) not null,
	[FirstName] [varchar](50) null,
	[MiddleName] [varchar](50) null,
	[LastName] [varchar](75) null,
	[DbaName] [varchar](150) null,
	[InsuredType] [varchar](30) not null,
	[PolicyId] [int] not null,
	[TransactionId] [int] not null,
	primary key ([InsuredId]),
	foreign key ([PolicyId]) references [Policy]([PolicyId]),
	foreign key ([TransactionId]) references [Transaction]([TransactionId])
);
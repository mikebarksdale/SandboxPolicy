create table [PolicyCoverage] (
	[PolicyCoverageId] [int] identity(1,1) not null,
	[PolicyCoverageName] [varchar](100) not null,
	[Limit] [decimal](18,2) null,
	[Deductible] [decimal] (10,2) null,
	[PolicyId] [int] not null,
	[TransactionId] [int] not null,
	primary key ([PolicyCoverageId]),
	foreign key ([PolicyId]) references [Policy]([PolicyId]),
	foreign key ([TransactionId]) references [Transaction]([TransactionId])
);
create table [Transaction] (
	[TransactionId] [int] identity(1,1) not null,
	[TransactionRefId] [int] null,
	[TransactionDate] [datetime] not null
		default getdate(),
	[TransactionType] [varchar](30) not null,
	[ModifiedUser] [varchar](50) not null,
	primary key ([TransactionId])
);
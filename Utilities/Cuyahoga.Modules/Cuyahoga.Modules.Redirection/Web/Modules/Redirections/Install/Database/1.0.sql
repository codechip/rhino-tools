create table cm_redirection
(
	id int identity primary key not null,
	updatetimestamp datetime not null,
	url nvarchar(255) not null,
	title nvarchar(100) not null,
	numberofdownloads int not null,
	datepublished datetime not null,	
	sectionid int not null,
	publisherid int not null
)
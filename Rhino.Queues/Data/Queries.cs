namespace Rhino.Queues.Data
{
	public static class Queries
	{
		public const string CreateTablesForIncomingQueue = @"
CREATE TABLE IncomingMessages
(
	Id VARCHAR(36) NOT NULL,
	Data BLOB NOT NULL,
	InsertedAt TIMESTAMP NOT NULL
);

ALTER TABLE IncomingMessages ADD CONSTRAINT PK_IncomingMessages PRIMARY KEY (Id);

CREATE INDEX IncomingMessages_InsertedAt_Idx ON IncomingMessages (InsertedAt);

CREATE TABLE ArrivedMessagesIds
(
	Id VARCHAR(36) NOT NULL
);
ALTER TABLE ArrivedMessagesIds ADD CONSTRAINT PK_ArrivedMessagesIds PRIMARY KEY (Id);
";
		public const string InsertMessageToIncomingQueue = @"
INSERT INTO IncomingMessages (Id, Data, InsertedAt)
VALUES (@Id, @Data, @InsertedAt);
";
		public const string TrackNewIncomingMessage = @"
INSERT INTO ArrivedMessagesIds (Id)
VALUES (@Id);
";
		public const string GetCountOfArrivedMessagesById = @"
SELECT COUNT(*) FROM ArrivedMessagesIds 
WHERE Id = (@Id);
";

		public const string GetEarliestMessageFromIncomingQueue = @"
SELECT FIRST 1 Id, Data FROM IncomingMessages
ORDER BY InsertedAt ASC
FOR UPDATE WITH LOCK
";
		public const string DeleteMessageFromIncomingQueue = @"
DELETE FROM IncomingMessages
WHERE Id = @Id
";

		public const string CreateTablesForOutgoingQueue = @"
CREATE TABLE OutgoingMessages
(
	Id VARCHAR(36) NOT NULL,
	Destination VARCHAR(256) CHARACTER SET UNICODE_FSS NOT NULL,
	Data BLOB NOT NULL,
	InsertedAt TIMESTAMP NOT NULL,
	SendAt TIMESTAMP NOT NULL,
	FailureCount INT NOT NULL,
	BatchId  VARCHAR(36)
);

ALTER TABLE OutgoingMessages ADD CONSTRAINT PK_OutgoingMessages PRIMARY KEY (Id);

CREATE TABLE FailedMessages
(
	Id VARCHAR(36) NOT NULL,
	Destination VARCHAR(256) CHARACTER SET UNICODE_FSS  NOT NULL,
	Data BLOB NOT NULL,
	FinalFailureAt TIMESTAMP NOT NULL,
	FailureCount INT NOT NULL,
	BatchId VARCHAR(36) NOT NULL,
	LastException BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET UNICODE_FSS NOT NULL
);

ALTER TABLE FailedMessages ADD CONSTRAINT PK_FailedMessages PRIMARY KEY (Id);

";
		public const string InsertMessageToOutgoingQueue = @"
INSERT INTO OutgoingMessages (Id, Destination, Data, InsertedAt, SendAt, FailureCount)
VALUES (@Id, @Destination, @Data, @InsertedAt, @SendAt, 0);
";
		public const string SelectMessagesFromOutgoing = @"
SELECT FIRST 100 Id, Destination, Data FROM OutgoingMessages
WHERE SendAt <= @CurrentTime AND BatchId IS NULL
ORDER BY SendAt ASC
FOR UPDATE WITH LOCK
";
		public const string UpdateBatchId = @"
UPDATE OutgoingMessages
SET BatchId = @BatchId
WHERE Id = @Id
";
		public const string ResetBatchIdForOutgoingQueueByIdAndDestination = @"
UPDATE OutgoingMessages
SET BatchId = NULL
WHERE BatchId = @BatchId AND Destination = @Destination
";
		public const string ResetAllBatchIdForOutgoingQueue = @"
UPDATE OutgoingMessages
SET BatchId = NULL
";

		public const string UpdateFailureCountAndTimeToSend = @"
UPDATE OutgoingMessages
SET 
	FailureCount = @FailureCount,
	SendAt = @SendAt
WHERE Id = @Id
";
		public const string SelectFailureCountAndTime = @"
SELECT
	Id,
	FailureCount
FROM OutgoingMessages
WHERE BatchId = @BatchId AND Destination = @Destination
";

		public const string DeleteSuccessfulBatch = @"
DELETE FROM OutgoingMessages
WHERE BatchId = @BatchId AND Destination = @Destination
";
		public const string MoveAllMessagesInBatchWithFailureCountToFAiledMessages_Part1 = @"
INSERT INTO FailedMessages
           (Id,
            Destination,
            Data,
            FinalFailureAt,
            FailureCount,
            BatchId,
            LastException)
SELECT Id,
       Destination,
       Data,
       @CurrentTime,
	   FailureCount,
       BatchId,
       @LastException
FROM   OutgoingMessages
WHERE  BatchId = @BatchId 
	   AND Destination = @Destination
       AND FailureCount >= @MaxNumberOfFailures;
";

		public const string MoveAllMessagesInBatchWithFailureCountToFAiledMessages_Part2 = @"
DELETE FROM OutgoingMessages
WHERE       BatchId = @BatchId
			AND Destination = @Destination
            AND FailureCount >= @MaxNumberOfFailures;
";
		public const string PurgeAllMessagesFromOutgoing = @"
DELETE FROM OutgoingMessages;
DELETE From FailedMessages;
";
		public const string PurgeAllMessagesFromIncoming = @"
DELETE FROM IncomingMessages;
";

		public const string CreateTablesForQueuesList =
			@"
CREATE TABLE Queues
(
	Name VARCHAR(256) NOT NULL,
	QueueType INTEGER NOT NULL
)
";
		public const string InsertNewQuque = @"
INSERT INTO Queues (Name, QueueType)
VALUES (@Name, @Type);
";

		public const string SelectQueues = @"
SELECT Name FROM Queues 
WHERE QueueType = @Type;
";
	}
}
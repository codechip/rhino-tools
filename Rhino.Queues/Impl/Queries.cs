namespace Rhino.Queues.Impl
{
	public static class Queries
	{
		public const string CreateTablesForIncomingQueue = @"
CREATE TABLE Messages
(
	Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	Data BLOB NOT NULL,
	InsertedAt DATETIME NOT NULL
);
CREATE INDEX MessagesInsertedAt ON Messages ( InsertedAt ASC )
";
		public const string InsertMessageToIncomingQueue = @"
INSERT INTO Messages (Id, Data, InsertedAt)
VALUES (@Id, @Data, @InsertedAt);
";
		public const string GetEarliestMessageFromIncomingQueue = @"
SELECT Id, Data FROM Messages
ORDER BY InsertedAt ASC
LIMIT 1
";
		public const string DeleteMessageFromIncomingQueue = @"
DELETE FROM Messages
WHERE Id = @Id
";

		public const string CreateTablesForOutgoingQueue = @"
CREATE TABLE Messages
(
	Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	Destination NVARCHAR NOT NULL,
	Data BLOB NOT NULL,
	InsertedAt DATETIME NOT NULL,
	SendAt DATETIME NOT NULL,
	FailureCount INT NOT NULL DEFAULT(0),
	BatchId UNIQUEIDENTIFIER NULL
);

CREATE TABLE FailedMessages
(
	Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	Destination NVARCHAR NOT NULL,
	Data BLOB NOT NULL,
	FinalFailureAt DATETIME NOT NULL,
	FailureCount INT NOT NULL,
	BatchId UNIQUEIDENTIFIER NOT NULL,
	LastException NVARCHAR NOT NULL
);
";
		public const string InsertMessageToOutgoingQueue = @"
INSERT INTO Messages (Id, Destination, Data, InsertedAt, SendAt)
VALUES (@Id, @Destination, @Data, @InsertedAt, @SendAt);
";
		// note that here we get limit from the previously called 
		// UpdateBatchId, so we don't worry about it
		public const string SelectMessagesFromOutgoing = @"
SELECT Destination, Data FROM Messages
WHERE BatchId = @BatchId
";
		public const string UpdateBatchId = @"
UPDATE Messages
SET BatchId = @BatchId
WHERE Id IN (
	SELECT Id FROM Messages
	WHERE SendAt <= @CurrentTime AND BatchId IS NULL
	ORDER BY SendAt ASC
	LIMIT 100
)
";
		public const string ResetBatchIdForOutgoingQueueByIdAndDestination = @"
UPDATE Messages
SET BatchId = NULL
WHERE BatchId = @BatchId AND Destination = @Destination
";
		public const string ResetAllBatchIdForOutgoingQueue = @"
UPDATE Messages
SET BatchId = NULL
";

		public const string UpdateFailureCountAndTimeToSend = @"
UPDATE Messages
SET 
	FailureCount = FailureCount + 1,
	SendAt = datetime(
		julianday(SendAt) + ( (FailureCount+1) * (FailureCount+1) * 0.000005)
	)
WHERE BatchId = @BatchId AND Destination = @Destination
";

		public const string DeleteSuccessfulBatch = @"
DELETE FROM Messages
WHERE BatchId = @BatchId AND Destination = @Destination
";
		public const string MoveAllMessagesInBatchWithFailureCountToFAiledMessages = @"
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
FROM   Messages
WHERE  BatchId = @BatchId 
	   AND Destination = @Destination
       AND FailureCount >= @MinNumberOfFailures;

DELETE FROM Messages
WHERE       BatchId = @BatchId
			AND Destination = @Destination
            AND FailureCount >= @MinNumberOfFailures;
";
		public const string PurgeAllMessagesFromOutgoing = @"
DELETE FROM Messages;
DELETE From FailedMessages;
";
		public const string PurgeAllMessagesFromIncoming = @"
DELETE FROM Messages;
";
	}
}
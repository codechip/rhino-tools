namespace Rhino.Etl.Core.Operations
{
    using System;
    using System.Collections.Generic;
    using Commons;
    using Enumerables;

    /// <summary>
    /// Perform a join between two sources
    /// </summary>
    public abstract class JoinOperation : AbstractOperation
    {
        private readonly PartialProcessOperation left = new PartialProcessOperation();
        private readonly PartialProcessOperation right = new PartialProcessOperation();

        /// <summary>
        /// Sets the right part of the join
        /// </summary>
        /// <value>The right.</value>
        public JoinOperation Right(IOperation value)
        {
            right.Register(value);
            return this;
        }


        /// <summary>
        /// Sets the left part of the join
        /// </summary>
        /// <value>The left.</value>
        public JoinOperation Left(IOperation value)
        {
            left.Register(value);
            return this;
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="ignored">Ignored rows</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> ignored)
        {
            Initialize();

            Guard.Against(left == null, "Left branch of a join cannot be null");
            Guard.Against(right == null, "Right branch of a join cannot be null");

            Dictionary<Row, object> matchedRightRows = new Dictionary<Row, object>();
            CachingEnumerable<Row> rightEnumerable = new CachingEnumerable<Row>(right.Execute(null));
            foreach (Row leftRow in left.Execute(null))
            {
                bool leftNeedOuterJoin = true;
                foreach (Row rightRow in rightEnumerable)
                {
                    if (MatchJoinCondition(leftRow, rightRow))
                    {
                        leftNeedOuterJoin = false;
                        matchedRightRows[rightRow] = null;
                        yield return MergeRows(leftRow, rightRow);
                    }
                }
                if (leftNeedOuterJoin)
                {
                    Row emptyRow = new Row();
                    if (MatchJoinCondition(leftRow, emptyRow))
                        yield return MergeRows(leftRow, emptyRow);
                }
            }
            foreach (Row rightRow in rightEnumerable)
            {
                if (matchedRightRows.ContainsKey(rightRow))
                    continue;
                Row emptyRow = new Row();
                if (MatchJoinCondition(emptyRow, rightRow))
                    yield return MergeRows(emptyRow, rightRow);
            }
        }

        /// <summary>
        /// Merges the two rows into a single row
        /// </summary>
        /// <param name="leftRow">The left row.</param>
        /// <param name="rightRow">The right row.</param>
        /// <returns></returns>
        protected abstract Row MergeRows(Row leftRow, Row rightRow);

        /// <summary>
        /// Check if the two rows match to the join condition.
        /// </summary>
        /// <param name="leftRow">The left row.</param>
        /// <param name="rightRow">The right row.</param>
        /// <returns></returns>
        protected abstract bool MatchJoinCondition(Row leftRow, Row rightRow);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            left.Dispose();
            right.Dispose();
        }


        /// <summary>
        /// Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            left.PrepareForExecution(pipelineExecuter);
            right.PrepareForExecution(pipelineExecuter);
        }

        /// <summary>
        /// Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Exception> GetAllErrors()
        {
            foreach (Exception error in left.GetAllErrors())
            {
                yield return error;
            }
            foreach (Exception error in right.GetAllErrors())
            {
                yield return error;
            }
        }
    }
}
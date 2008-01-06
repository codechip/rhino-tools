namespace Rhino.Etl.Core.Operations
{
    using System;
    using System.Threading;

    /// <summary>
    /// Contains the statistics for an operation
    /// </summary>
    public class OperationStatistics
    {
        private DateTime? start;
        private DateTime? end;
        private int processedRows = 0;

        /// <summary>
        /// Gets number of the processed rows.
        /// </summary>
        /// <value>The processed rows.</value>
        public int ProcessedRows
        {
            get { return processedRows; }
        }

        /// <summary>
        /// Gets the duration this operation has executed
        /// </summary>
        /// <value>The duration.</value>
        public TimeSpan Duration
        {
            get
            {
                if( start == null || end == null)
                    return new TimeSpan();

                return end.Value - start.Value;
            }
        }

        /// <summary>
        /// Mark the start time
        /// </summary>
        public void MarkStarted()
        {
            start = DateTime.Now;
        }

        /// <summary>
        /// Mark the end time
        /// </summary>
        public void MarkFinished()
        {
            end = DateTime.Now;
        }

        /// <summary>
        /// Marks a processed row.
        /// </summary>
        public void MarkRowProcessed()
        {
            Interlocked.Increment(ref processedRows);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return ProcessedRows + " Rows in " + Duration;
        }
    }
}
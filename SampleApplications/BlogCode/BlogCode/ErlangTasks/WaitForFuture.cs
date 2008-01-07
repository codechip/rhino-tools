namespace pipelines
{
    public class WaitForFuture
    {
        private readonly Future future;
        private readonly Condition condition;

        public WaitForFuture(Condition condition)
        {
            this.condition = condition;
        }

        public WaitForFuture(Future future)
        {
            this.future = future;
        }

        public bool Condition()
        {
            if (condition != null)
                return condition();
            return future.HasValue;
        }

        public static bool operator false(WaitForFuture f)
        {
            return false;
        }

        public static bool operator true(WaitForFuture f)
        {
            return false;
        }

        public static WaitForFuture operator |(WaitForFuture left, WaitForFuture right)
        {
            return new WaitForFuture(delegate { return left.Condition() || right.Condition(); });
        }

        public static WaitForFuture operator &(WaitForFuture left, WaitForFuture right)
        {
            return new WaitForFuture(delegate { return left.Condition() && right.Condition(); });
        }

        public static implicit operator Condition(WaitForFuture f)
        {
            return f.Condition;
        }
    }
}
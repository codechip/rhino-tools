namespace Rhino.Igloo
{
    /// <summary>
    /// Tye type of scope to use
    /// </summary>
    public enum ScopeType
    {
        /// <summary>
        /// The current input scope
        /// </summary>
        Input,
        /// <summary>
        /// The current input scope for an array
        /// </summary>
        Inputs,
        /// <summary>
        /// The session scope
        /// </summary>
        Session,
        /// <summary>
        /// The flash scope
        /// </summary>
        Flash
    }
}
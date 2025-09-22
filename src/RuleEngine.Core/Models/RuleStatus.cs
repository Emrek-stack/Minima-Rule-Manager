namespace RuleEngine.Core.Models
{
    /// <summary>
    /// Represents the status of a rule
    /// </summary>
    public enum RuleStatus
    {
        /// <summary>
        /// Rule is in draft state and not active
        /// </summary>
        Draft = 0,

        /// <summary>
        /// Rule is active and can be executed
        /// </summary>
        Active = 1,

        /// <summary>
        /// Rule is disabled and cannot be executed
        /// </summary>
        Disabled = 2
    }
}

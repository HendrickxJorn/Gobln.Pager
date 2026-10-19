namespace Gobln.Pager.Blazor;

/// <summary>
/// When to show a part of the pager. A copy of the Mvc package's enum of the same name, so the
/// two pagers are configured alike without one package depending on the other.
/// </summary>
public enum WhenToShowEnum
{
    /// <summary>
    /// Never shown
    /// </summary>
    Never = 0,

    /// <summary>
    /// Only shown when it does something: the pager only when there is more than one page,
    /// First and Previous only when not on the first page, Next and Last only when not on the last
    /// </summary>
    IfNecessary = 1,

    /// <summary>
    /// Always shown, disabled where it can't do anything
    /// </summary>
    Always = 2,
}

namespace BuildNotifications.ViewModel.Utils
{
    /// <summary>
    /// Interface for items which can track whether are getting removed from a list
    /// </summary>
    public interface IRemoveTracking
    {
        bool IsRemoving { get; set; }
    }
}
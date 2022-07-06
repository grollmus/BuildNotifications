using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildNotifications.ViewModel.Utils;

public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
{
    public NotifyTaskCompletion(Task<TResult> task)
    {
        Task = task;
        TaskCompletion = WatchTaskAsync(task);
    }

    public string? ErrorMessage => InnerException?.Message;

    public AggregateException? Exception => Task.Exception;

    public Exception? InnerException => Exception?.InnerException;

    public bool IsCanceled => Task.IsCanceled;
    public bool IsCompleted => Task.IsCompleted;
    public bool IsFaulted => Task.IsFaulted;
    public bool IsNotCompleted => !Task.IsCompleted;

    public bool IsSuccessfullyCompleted => Task.Status ==
                                           TaskStatus.RanToCompletion;

    public TResult Result => Task.Status == TaskStatus.RanToCompletion ? Task.Result : Activator.CreateInstance<TResult>();

    public TaskStatus Status => Task.Status;
    public Task<TResult> Task { get; }
    public Task TaskCompletion { get; }

    private async Task WatchTaskAsync(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            // ignored
        }

        var propertyChanged = PropertyChanged;
        if (propertyChanged == null)
            return;

        propertyChanged(this, new PropertyChangedEventArgs(nameof(Status)));
        propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
        propertyChanged(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));
        if (task.IsCanceled)
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
        else if (task.IsFaulted)
        {
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(Exception)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(InnerException)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }
        else
        {
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(Result)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
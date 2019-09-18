using System;

namespace BuildNotifications.PluginInterfacesLegacy.Notification
{
    public class FeedbackEventArgs : EventArgs
    {
        public FeedbackEventArgs(string feedbackArguments)
        {
            FeedbackArguments = feedbackArguments;
        }

        public string FeedbackArguments { get; }
    }
}
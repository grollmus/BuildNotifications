using System;

namespace BuildNotifications.PluginInterfacesLegacy.Notification
{
    public class FeedbackEventArgs : EventArgs
    {
        public string FeedbackArguments { get; }

        public FeedbackEventArgs(string feedbackArguments)
        {
            FeedbackArguments = feedbackArguments;
        }
    }
}
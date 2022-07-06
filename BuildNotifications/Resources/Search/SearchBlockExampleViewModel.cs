using BuildNotifications.ViewModel;

namespace BuildNotifications.Resources.Search;

internal class SearchBlockExampleViewModel : BaseViewModel
{
    public SearchBlockExampleViewModel(string keyword, string exampleText)
    {
        Keyword = keyword;
        ExampleText = exampleText;
    }

    public string ExampleText { get; }
    public string Keyword { get; }
}
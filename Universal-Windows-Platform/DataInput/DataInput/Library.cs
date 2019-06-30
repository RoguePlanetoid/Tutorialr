using Windows.Storage;

public class Library
{
    public string LoadSetting(string key)
    {
        return (string)(ApplicationData.Current.LocalSettings.Values[key] 
        ?? string.Empty);
    }

    public void SaveSetting(string key, string value)
    {
        ApplicationData.Current.LocalSettings.Values[key] = value;
    }
}
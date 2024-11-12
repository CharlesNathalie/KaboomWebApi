using System.Globalization;

namespace KaboomWebApi.Services;

public interface ILanguageService
{
    string Language { get; set; }
    List<string> AvailableLanguage { get; set; }
    void SetLanguage(string lang);
}
public class LanguageService : ILanguageService
{
    public List<string> AvailableLanguage { get; set; }
    public string Language { get; set; }

    public LanguageService()
    {
        AvailableLanguage = new List<string> { "en", "fr" };
        Language = AvailableLanguage[0]; // en    
    }

    public void SetLanguage(string lang)
    {
        if (!AvailableLanguage.Contains(lang))
        {
            Language = AvailableLanguage[0]; // en
        }
        else
        {
            Language = lang;
        }

        Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

        AuthControllerRes.Culture = new CultureInfo(lang);
        GameRes.Culture = new CultureInfo(lang);
        UserRes.Culture = new CultureInfo(lang);
    }
}

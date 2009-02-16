using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace WOP {
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {

      public static App MainApp;
      public static ResourceDictionary blackTheme;
      public static ResourceDictionary whiteTheme;

      private void Application_Startup(object sender, StartupEventArgs e)
      {
        string blackThemeStr = File.ReadAllText("Styles/BlackTheme.xaml");
        StringReader sr = new StringReader(blackThemeStr);
        XmlReader xr = XmlReader.Create(sr);
        blackTheme = (ResourceDictionary)XamlReader.Load(xr);
        string whiteThemeStr = File.ReadAllText("Styles/WhiteTheme.xaml");
        sr = new StringReader(whiteThemeStr);
        xr = XmlReader.Create(sr);
        whiteTheme = (ResourceDictionary)XamlReader.Load(xr);
        ActivateBlackTheme();
        MainApp = this;
      }

      public void ActivateBlackTheme()
      {
        Application.Current.Resources.MergedDictionaries.Remove(whiteTheme);
        Application.Current.Resources.MergedDictionaries.Remove(blackTheme);
        Application.Current.Resources.MergedDictionaries.Add(blackTheme);
      }

      public void ActivateWhiteTheme()
      {
        Application.Current.Resources.MergedDictionaries.Remove(whiteTheme);
        Application.Current.Resources.MergedDictionaries.Remove(blackTheme);
        Application.Current.Resources.MergedDictionaries.Add(whiteTheme);
      }
    }
}
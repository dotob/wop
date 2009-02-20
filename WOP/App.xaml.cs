using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using NLog;

namespace WOP
{
  /// <summary>
  /// Interaktionslogik für "App.xaml"
  /// </summary>
  public partial class App : Application
  {
    protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
    public static ResourceDictionary blackTheme;
    public static App MainApp;
    public static ResourceDictionary whiteTheme;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
      logger.Debug("wop started, trying to load themes");
      try {
        string blackThemeStr = File.ReadAllText("Styles/BlackTheme.xaml");
        StringReader sr = new StringReader(blackThemeStr);
        XmlReader xr = XmlReader.Create(sr);
        blackTheme = (ResourceDictionary)XamlReader.Load(xr);
        string whiteThemeStr = File.ReadAllText("Styles/WhiteTheme.xaml");
        sr = new StringReader(whiteThemeStr);
        xr = XmlReader.Create(sr);
        whiteTheme = (ResourceDictionary)XamlReader.Load(xr);
        this.ActivateBlackTheme();
      } catch(Exception ex) {
        logger.ErrorException("error while loading themes", ex);
      }
      MainApp = this;
    }

    public void ActivateBlackTheme()
    {
      if (whiteTheme != null && blackTheme != null) {
        Application.Current.Resources.MergedDictionaries.Remove(whiteTheme);
        Application.Current.Resources.MergedDictionaries.Remove(blackTheme);
        Application.Current.Resources.MergedDictionaries.Add(blackTheme);
      }
    }

    public void ActivateWhiteTheme()
    {
      if (whiteTheme != null && blackTheme != null) {
        Application.Current.Resources.MergedDictionaries.Remove(whiteTheme);
        Application.Current.Resources.MergedDictionaries.Remove(blackTheme);
        Application.Current.Resources.MergedDictionaries.Add(whiteTheme);
      }
    }
  }
}
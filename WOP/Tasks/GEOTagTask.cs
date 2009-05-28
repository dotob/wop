using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Xml;
using System.Xml.XPath;
using WOP.Objects;
using WOP.TasksUI;
using WOP.Util;

namespace WOP.Tasks {
  public class GEOTagTask : SkeletonTask {
    private ObservableCollection<FileInfo> gpxFiles = new ObservableCollection<FileInfo>();
    private bool inited;
    private List<WayPoint> wayPointList;
    private UserControl ui;

    public GEOTagTask()
    {
      this.Name = "GEO-Tagging";
    }

    public override UserControl UI
    {
      get
      {
        this.ui = new GEOTagTaskUI();
        this.ui.DataContext = this;
        return this.ui;
      }
      set { this.ui = value; }
    }

    public ObservableCollection<FileInfo> GpxFiles
    {
      get { return this.gpxFiles; }
      set { this.gpxFiles = value; }
    }

    public override ITask CloneNonDynamicStuff()
    {
      GEOTagTask t = new GEOTagTask();
      t.IsEnabled = this.IsEnabled;
      foreach (FileInfo file in this.gpxFiles) {
        t.gpxFiles.Add(new FileInfo(file.FullName));
      }
      return t;
    }

    public override bool Process(ImageWI iwi)
    {
      if (!this.inited) {
        this.wayPointList = initGPXFiles(this.gpxFiles);
        this.inited = true;
      }
      // find 
      WayPoint wp = findWaypoint4Date(iwi.ExifDate, this.wayPointList);
      if (wp != null) {
        ImageWorker.WriteGPSDateIntoImage(iwi.CurrentFile, null, wp);
      } else {
        // tell anyone we didnt found a suitable waypoint
      }
      return true;
    }

    public static WayPoint findWaypoint4Date(DateTime? dt, List<WayPoint> wayPointList)
    {
      WayPoint last = null;
      if (dt != null) {
        foreach (WayPoint current in wayPointList) {
          if (last != null) {
            // check if our time is between the last and the current point
            if (dt < current.LocalTime && dt > last.LocalTime) {
              // check which of the two points is nearer
              if ((current.LocalTime - dt) < (dt - last.LocalTime)) {
                return current;
              }
              return last;
            }
          }
          last = current;
        }
      }
      return null;
    }

    public static List<WayPoint> initGPXFiles(IEnumerable<FileInfo> files)
    {
      List<WayPoint> list = new List<WayPoint>();
      // read gpx files in
      foreach (FileInfo fi in files) {
        var doc = new XPathDocument(fi.FullName);
        XPathNavigator navi = doc.CreateNavigator();
        var xman = new XmlNamespaceManager(navi.NameTable);
        xman.AddNamespace("g", "http://www.topografix.com/GPX/1/1");
        XPathNodeIterator nodes = navi.Select("/g:gpx/g:wpt", xman);
        while (nodes.MoveNext()) {
          var wp = new WayPoint();
          wp.Latitude = Convert.ToDouble(nodes.Current.GetAttribute("lat", string.Empty));
          wp.Longitude = Convert.ToDouble(nodes.Current.GetAttribute("lon", string.Empty));
          XPathNavigator eleNode = nodes.Current.SelectSingleNode("g:ele", xman);
          if (eleNode != null) {
            wp.Elevation = eleNode.ValueAsDouble;
          }
          XPathNavigator timeNode = nodes.Current.SelectSingleNode("g:time", xman);
          if (timeNode != null) {
            wp.Time = timeNode.ValueAsDateTime;
          }
          list.Add(wp);
        }
      }
      list.Sort((a, b) => a.Time.CompareTo(b.Time));
      return list;
    }
  }
}
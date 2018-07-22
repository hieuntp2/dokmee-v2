using Dokmee.Dms.Connector.Advanced.Core.Data;
using System.Collections.Generic;
using System.ComponentModel;
using Services.AuthService.Models;

namespace Services.AuthService.Models
{
  public class ConnectorModel : INotifyPropertyChanged
  {
    public ConnectorModel()
    {
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    private Visibility isProgressVisible = Visibility.Collapsed;

    public Visibility IsProgressVisible
    {
      get { return isProgressVisible; }
      set
      {
        isProgressVisible = value;
        OnPropertyChanged("IsProgressVisible");
      }
    }

    private Visibility isUrlVisible = Visibility.Collapsed;

    public Visibility IsUrlVisible
    {
      get { return isUrlVisible; }
      set
      {
        isUrlVisible = value;
        OnPropertyChanged("IsUrlVisible");
      }
    }

    private Visibility isServerVisible = Visibility.Collapsed;

    public Visibility IsServerVisible
    {
      get { return isServerVisible; }
      set
      {
        isServerVisible = value;
        OnPropertyChanged("IsServerVisible");
      }
    }

    private Visibility isSignoutVisible = Visibility.Collapsed;

    public Visibility IsSignoutVisible
    {
      get { return isSignoutVisible; }
      set
      {
        isSignoutVisible = value;
        OnPropertyChanged("IsSignoutVisible");
      }
    }

    private Visibility isSigninVisible = Visibility.Collapsed;

    public Visibility IsSigninVisible
    {
      get { return isSigninVisible; }
      set
      {
        isSigninVisible = value;
        IsSignin = isSigninVisible == Visibility.Visible;
        IsSignoutVisible = isSigninVisible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        OnPropertyChanged("IsSigninVisible");
      }
    }

    private bool isSignin = true;

    public bool IsSignin
    {
      get { return isSignin; }
      set
      {
        isSignin = value;
        OnPropertyChanged("IsSignin");
      }
    }

    private bool isPageEnabled = false;

    public bool IsPageEnabled
    {
      get { return isPageEnabled; }
      set
      {
        isPageEnabled = value;
        OnPropertyChanged("IsPageEnabled");
      }
    }

    private bool isFolderSelected = true;

    public bool IsFolderSelected
    {
      get { return isFolderSelected; }
      set
      {
        isFolderSelected = value;
        OnPropertyChanged("IsFolderSelected");
      }
    }

    private bool isFileSelected;

    public bool IsFileSelected
    {
      get { return isFileSelected; }
      set
      {
        isFileSelected = value;
        OnPropertyChanged("IsFileSelected");
      }
    }

    private string hostUrl = @"http://localhost/dokmeedms";

    public string HostUrl
    {
      get { return hostUrl; }
      set
      {
        hostUrl = value;
        OnPropertyChanged("HostUrl");
      }
    }

    private string server = @"localhost\sqlexpress";

    public string Server
    {
      get { return server; }
      set
      {
        server = value;
        OnPropertyChanged("Server");
      }
    }

    private string userName = "admin";

    public string UserName
    {
      get { return userName; }
      set
      {
        userName = value;
        OnPropertyChanged("UserName");
      }
    }

    private string password = "admin";

    public string Password
    {
      get { return password; }
      set
      {
        password = value;
        OnPropertyChanged("Password");
      }
    }

    private IEnumerable<Node> nodes;

    public IEnumerable<Node> Nodes
    {
      get { return nodes; }
      set
      {
        nodes = value;
        OnPropertyChanged("Nodes");
      }
    }

    private Node selectedFile;

    public Node SelectedFile
    {
      get { return selectedFile; }
      set
      {
        selectedFile = value;
        OnPropertyChanged("SelectedFile");
      }
    }

    private IEnumerable<Node> files;

    public IEnumerable<Node> Files
    {
      get { return files; }
      set
      {
        files = value;
        OnPropertyChanged("Files");
      }
    }

    private Node selectedFolder;

    public Node SelectedFolder
    {
      get { return selectedFolder; }
      set
      {
        selectedFolder = value;
        OnPropertyChanged("SelectedFolder");
      }
    }

    private IEnumerable<Node> folders;

    public IEnumerable<Node> Folders
    {
      get { return folders; }
      set
      {
        folders = value;
        OnPropertyChanged("Folders");
      }
    }

    private IEnumerable<Cabinet> cabinets;

    public IEnumerable<Cabinet> Cabinets
    {
      get { return cabinets; }
      set
      {
        cabinets = value;
        OnPropertyChanged("Cabinets");
      }
    }

    private Cabinet selectedCabinets;

    public Cabinet SelectedCabinets
    {
      get { return selectedCabinets; }
      set
      {
        selectedCabinets = value;
        OnPropertyChanged("SelectedCabinets");
      }
    }

    private LookupResults lookupResults;

    public LookupResults LookupResults
    {
      get { return lookupResults; }
      set
      {
        lookupResults = value;
        OnPropertyChanged("LookupResults");
      }
    }

    private ConnectorTypes selectedConnectorType;

    public ConnectorTypes SelectedConnectorType
    {
      get { return selectedConnectorType; }
      set
      {
        selectedConnectorType = value;
        OnPropertyChanged("SelectedConnectorType");
      }
    }

    public IEnumerable<ConnectorTypes> ConnectorTypes
    {
      get
      {
        return new List<ConnectorTypes> { new ConnectorTypes { Name = "DMS", CType = ConnectorType.DMS },
                                                  new ConnectorTypes { Name = "WEB", CType = ConnectorType.WEB },
                                                  new ConnectorTypes { Name = "CLOUD", CType = ConnectorType.CLOUD }};
      }
    }

    private double listViewWidth;

    public double ListViewWidth
    {
      get { return listViewWidth; }
      set
      {
        listViewWidth = value;
        OnPropertyChanged("ListViewWidth");
      }
    }

    private double listViewHeight;

    public double ListViewHeight
    {
      get { return listViewHeight; }
      set
      {
        listViewHeight = value;
        OnPropertyChanged("ListViewHeight");
      }
    }
  }

  public class ConnectorTypes
  {
    public string Name { get; set; }
    public ConnectorType CType { get; set; }
  }
}

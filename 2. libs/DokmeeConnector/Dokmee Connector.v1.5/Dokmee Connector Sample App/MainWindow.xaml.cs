
using MahApps.Metro.Controls;
using sample_wpf_2.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Dokmee.Dms.Advanced.WebAccess.Data;
using Dokmee.Dms.Connector.Advanced;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using Dokmee.Dms.Connector.Advanced.Extension;

namespace sample_wpf_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private DmsConnector dmsConnector;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ConnectorModel();
            ConnectorVM.SelectedConnectorType = ConnectorVM.ConnectorTypes.First();
            ConnectorVM.IsSigninVisible = System.Windows.Visibility.Visible;

            Dispatcher.BeginInvoke(DispatcherPriority.Input,
                           new Action(delegate()
                           {
                               tbUserName.Focus();
                               Keyboard.Focus(tbUserName);
                           }));
        }

        private ConnectorModel ConnectorVM
        {
            get { return DataContext as ConnectorModel; }
        }

        private void pbPassword_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            ConnectorVM.Password = pb.Password;
        }

        private void signIn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ConnectorVM.IsProgressVisible = System.Windows.Visibility.Visible;

            // initialize connector
            DokmeeApplication dApp = DokmeeApplication.DokmeeDMS;

            if (ConnectorVM.SelectedConnectorType.CType == ConnectorType.DMS)
            {
                ConnectionInfo connInfo = new ConnectionInfo();
                connInfo.ServerName = ConnectorVM.Server;
                connInfo.UserID = "sa";
                connInfo.Password = "123456";

                // register connection
                dApp = DokmeeApplication.DokmeeDMS;
                dmsConnector = new DmsConnector(dApp);
                dmsConnector.RegisterConnection<ConnectionInfo>(connInfo);
            }
            else if (ConnectorVM.SelectedConnectorType.CType == ConnectorType.WEB)
            {
                // register connection
                dApp = DokmeeApplication.DokmeeWeb;
                dmsConnector = new DmsConnector(dApp);
                dmsConnector.RegisterConnection<string>(ConnectorVM.HostUrl);
            }
            else if (ConnectorVM.SelectedConnectorType.CType == ConnectorType.CLOUD)
            {
                // register connection
                dApp = DokmeeApplication.DokmeeCloud;
                dmsConnector = new DmsConnector(dApp);
                dmsConnector.RegisterConnection<string>("https://www.dokmeecloud.com");
            }

            BackgroundWorker bgWorker = new BackgroundWorker();
            bgWorker.DoWork += (s, ev) =>
            {
                var connectVM = ev.Argument as ConnectorModel;
                var cabinets = dmsConnector.Login(new LogonInfo { Username = connectVM.UserName, Password = connectVM.Password });
                ev.Result = cabinets.DokmeeCabinets.Select(x => new Cabinet { Name = x.CabinetName, ID = x.CabinetID });
            };
            bgWorker.RunWorkerCompleted += (s, ev) =>
            {
                ConnectorVM.IsProgressVisible = System.Windows.Visibility.Collapsed;

                if (ev.Error != null)
                {
                    MessageBox.Show(ev.Error.Message);
                    return;
                }

                ConnectorVM.Cabinets = ev.Result as IEnumerable<Cabinet>;
                ConnectorVM.IsPageEnabled = true;
                ConnectorVM.IsSigninVisible = System.Windows.Visibility.Collapsed;
            };

            if (!bgWorker.IsBusy)
                bgWorker.RunWorkerAsync(ConnectorVM);
        }

        private void ConnectorType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            ConnectorVM.SelectedConnectorType = cb.SelectedItem as ConnectorTypes;
            ConnectorVM.IsUrlVisible = System.Windows.Visibility.Collapsed;
            ConnectorVM.IsServerVisible = System.Windows.Visibility.Collapsed;

            if (ConnectorVM.SelectedConnectorType.CType == ConnectorType.DMS)
            {
                ConnectorVM.IsServerVisible = System.Windows.Visibility.Visible;
                ConnectorVM.IsUrlVisible = System.Windows.Visibility.Collapsed;
            }
            else if (ConnectorVM.SelectedConnectorType.CType == ConnectorType.WEB)
            {
                ConnectorVM.IsServerVisible = System.Windows.Visibility.Collapsed;
                ConnectorVM.IsUrlVisible = System.Windows.Visibility.Visible;
            }
        }

        private void tbUsername_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.SelectAll();
        }

        private void pbPassword_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            pb.SelectAll();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectorVM.SelectedCabinets == null)
                return;
            else if (ConnectorVM.IsFolderSelected && ConnectorVM.SelectedFolder == null)
                return;
            else if (!ConnectorVM.IsFolderSelected && ConnectorVM.SelectedFile == null)
                return;

            ConnectorVM.IsProgressVisible = System.Windows.Visibility.Visible;

            BackgroundWorker bgWorker = new BackgroundWorker();
            bgWorker.DoWork += (s, ev) =>
            {
                var connectVM = ev.Argument as ConnectorModel;

                ev.Result = (connectVM.IsFolderSelected)
                            ? dmsConnector.Search(SearchFieldType.TextIndex, connectVM.SelectedFolder.Name, "Folder Title")
                            : dmsConnector.Search(SearchFieldType.ByNodeID, dmsConnector.GetFsNodesByName(SubjectTypes.Document, connectVM.SelectedFile.Name).First().ID.ToString());
            };
            bgWorker.RunWorkerCompleted += (s, ev) =>
            {
                ConnectorVM.IsProgressVisible = System.Windows.Visibility.Collapsed;

                if (ev.Error != null)
                {
                    MessageBox.Show(ev.Error.Message);
                    return;
                }

                var nodes = ev.Result as LookupResults;
                BuildSearchResultTable(nodes.DmsFilesystem);
                ConnectorVM.LookupResults = nodes;
                ConnectorVM.IsPageEnabled = true;
            };

            if (!bgWorker.IsBusy)
                bgWorker.RunWorkerAsync(ConnectorVM);
        }

        private void tbHostUrl_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.SelectAll();
        }

        private void tbServer_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.SelectAll();
        }

        private void Cabinet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem != null)
            {
                Cabinet cabinet = cb.SelectedItem as Cabinet;
                dmsConnector.RegisterCabinet(cabinet.ID);

                ConnectorVM.SelectedCabinets = cabinet;
                ConnectorVM.IsProgressVisible = System.Windows.Visibility.Visible;
                ConnectorVM.LookupResults = null;

                BackgroundWorker bgWorker = new BackgroundWorker();
                bgWorker.DoWork += (s, ev) =>
                {
                    ev.Result = dmsConnector.GetFsNodesByName();
                };
                bgWorker.RunWorkerCompleted += (s, ev) =>
                {
                    ConnectorVM.IsProgressVisible = System.Windows.Visibility.Collapsed;

                    if (ev.Error != null)
                    {
                        MessageBox.Show(ev.Error.Message);
                        return;
                    }

                    var nodes = ev.Result as IEnumerable<DmsNode>;
                    ConnectorVM.Folders = nodes.Where(x => x.IsFolder).Select(x => new Node { Name = x.Name, ID = x.ID });
                    ConnectorVM.Files = nodes.Where(x => !x.IsFolder).Select(x => new Node { Name = x.Name, ID = x.ID });
                    ConnectorVM.IsPageEnabled = true;
                };

                if (!bgWorker.IsBusy)
                    bgWorker.RunWorkerAsync();
            }
        }

        private void signOut_Click(object sender, RoutedEventArgs e)
        {
            dmsConnector.Logout();
            ConnectorVM.IsSigninVisible = System.Windows.Visibility.Visible;
            ConnectorVM.IsPageEnabled = false;
            ConnectorVM.LookupResults = null;
            ConnectorVM.SelectedCabinets = null;
        }

        private void folder_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if ((bool)rb.IsChecked)
            {
                ConnectorVM.IsFolderSelected = true;
                ConnectorVM.IsFileSelected = false;
            }
        }

        private void file_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if ((bool)rb.IsChecked)
            {
                ConnectorVM.IsFolderSelected = false;
                ConnectorVM.IsFileSelected = true;
            }
        }

        private void searchDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void searchDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (searchDataGrid.SelectedItems.Count == 1)
            {
                var config = Assembly.GetExecutingAssembly().Location;
                var document = searchDataGrid.SelectedItems[0] as DokmeeFilesystem;
                Process.Start(dmsConnector.ViewFile(document.FsGuid.ToString()), config);
            }
        }

        public void BuildSearchResultTable(IEnumerable<DokmeeFilesystem> results)
        {
            if (results == null)
                return;

            if (results.Count() == 0)
                return;

            GridView gv = searchDataGrid.View as GridView;
            int idx = gv.Columns.Count;

            //clear out other columns beside first column
            while (idx > 1)
            {
                gv.Columns.RemoveAt(1);
                idx--;
            }

            GridViewColumn gvc = new GridViewColumn();
            int indexPairCount = results.First().IndexFieldPairCollection.Count();
            for (int i = 0; i < indexPairCount; i++)
            {
                gvc = new GridViewColumn();
                gvc.DisplayMemberBinding = new Binding(string.Format("IndexFieldPairCollection[{0}].IndexValue", i));
                gvc.Header = results.First().IndexFieldPairCollection[i].IndexName;
                gv.Columns.Add(gvc);
            }

            gvc = new GridViewColumn();
            gvc.DisplayMemberBinding = new Binding("FullPath");
            gvc.Header = "Location";
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvc.DisplayMemberBinding = new Binding("DateCreated");
            gvc.Header = "DateCreated";
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvc.DisplayMemberBinding = new Binding("PageCount");
            gvc.Header = "Pages";
            gv.Columns.Add(gvc);
        }

        private void app_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ConnectorVM.ListViewHeight = appGrid.ActualHeight - 230;
            ConnectorVM.ListViewWidth = appGrid.ActualWidth - 5;
        }

        private void ExporttResults_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Type,");

            int indexPairCount = ConnectorVM.LookupResults.DmsFilesystem.First().IndexFieldPairCollection.Count();
            for (int i = 0; i < indexPairCount; i++)
            {
                sb.Append(ConnectorVM.LookupResults.DmsFilesystem.First().IndexFieldPairCollection[i].IndexName + ",");
            }

            sb.Append("FullPath, DateCreated, Pages\r\n");

            foreach (var item in ConnectorVM.LookupResults.DmsFilesystem)
            {
                sb.Append(item.FileType + ",");

                foreach (var index in item.IndexFieldPairCollection)
                {
                    sb.Append(index.IndexValue + ",");
                }

                sb.Append(item.FullPath + "," + item.DateCreated + "," + item.PageCount + "\r\n");
            }

            string reportName = string.Format("report_{0}.csv", string.Format("{0}_{1}_{2}_{3}_{4}_{5}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
            File.WriteAllText(reportName, sb.ToString());
            Process.Start(reportName);
        }

        private void ViewFile_Click(object sender, RoutedEventArgs e)
        {
            if (searchDataGrid.SelectedItems.Count == 1)
            {
                var config = Assembly.GetExecutingAssembly().Location;
                var document = searchDataGrid.SelectedItems[0] as DokmeeFilesystem;
                Process.Start(dmsConnector.ViewFile(document.FsGuid.ToString()), config);
            }
        }

        private void Folder_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectorVM.SelectedCabinets != null)
            {
                ConnectorVM.IsFolderSelected = true;
                ConnectorVM.IsFileSelected = false;
            }
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectorVM.SelectedCabinets != null)
            {
                ConnectorVM.IsFolderSelected = false;
                ConnectorVM.IsFileSelected = true;
            }
        }
    }
}
using Renci.SshNet;
using System;
using System.IO;
using System.Windows;


//Код от 2018 года. Хоть и писался за несколько часов на коленке, но ... =((((
//Епрст, так еще и все майн классе..... Надо будет перепилить.
namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DirectoryInfo dir = new DirectoryInfo(@"C:\UTM_LOG\");
        SftpClient Client;
        public MainWindow()
        {
            InitializeComponent();
            
            if (!Directory.Exists(@"C:\UTM_LOG\"))
            {
                dir.Create();
            }
        }

        /// <summary>
        /// Копирование логов Transport
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyTransLog(object sender, RoutedEventArgs e)
        {
            try
            {
                DownloadLog("transport");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            
        }

        /// <summary>
        /// Копирование Логов Updater
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyUpdLog(object sender, RoutedEventArgs e)
        {
            try
            {
                DownloadLog("updater");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }

        /// <summary>
        /// Копирование Лога Monitoring
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyMonLog(object sender, RoutedEventArgs e)
        {
            try
            {
                DownloadLog("monitoring");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            
        }
        /// <summary>
        /// Событие для удаления базы УТМ. Закоменчено т.к. были случаи удаления базы... 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickDelDB(object sender, RoutedEventArgs e)
        {
            //try
            //{
                
            //    using (var Client = new SshClient(ServerAdr.Text, Convert.ToInt32(ServerPort.Text), "***", "***"))
            //    {
            //        Client.Connect();
            //        if (Client.IsConnected)
            //        {
            //            Client.RunCommand("sudo rm -rf /opt/utm/transport/transportDB/");
            //        }
            //        else
            //        {
            //            MessageBox.Show("Нет поключения.");
            //        }
            //    }
            //}
            //catch(Exception err)
            //{
            //    MessageBox.Show(err.Message);
            //}
        }

        void DownloadLog(string LogName)
        {

                if (Client.IsConnected)
                {
                    var LogDir = dir.CreateSubdirectory(LogName + " " + DateTime.Now.ToString("yy-MM-dd_HH-mm")).FullName;
                    var Files = Client.ListDirectory("/opt/utm/" + LogName + "/l/");
                    foreach (var f in Files)
                    {
                        if ((!f.Name.StartsWith(".")) && (f.LastWriteTime.Date == DateTime.Today))
                        {
                            using (Stream FStream = File.OpenWrite(LogDir + "\\" + f.Name))
                            {
                                Client.DownloadFile("/opt/utm/" + LogName + "/l/" + f.Name, FStream);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Нет поключения.");
                }


            

        }
        // костыли что бы понять соединился с устройством или нет.
        private void ConnectionClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectSsh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void ConnectSsh()
        {
            if (Client != null)
            {
                if (Client.IsConnected)
                {
                    Client.Disconnect();
                    MessageBox.Show("Status: " + Client.IsConnected);
                }
                else
                {
                    Client = new SftpClient(ServerAdr.Text, Convert.ToInt32(ServerPort.Text), "***", "***");
                    Client.Connect();
                    MessageBox.Show("Status: " + Client.IsConnected);

                }
            }
            else
            {
                Client = new SftpClient(ServerAdr.Text, Convert.ToInt32(ServerPort.Text), "***", "***");
                if (!Client.IsConnected) { ConnectSsh(); }
            }
        }

    }
}

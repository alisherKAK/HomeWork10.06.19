using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;
using System.ComponentModel;

namespace HomeWork10_06_19
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<ProcessDescription> _processDescriptions = new ObservableCollection<ProcessDescription>();

        public MainWindow()
        {
            InitializeComponent();

            processesDataGrid.IsReadOnly = true;

            UpdateProcessDataGrid();
        }

        private string GetProcessOwner(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    return argList[1] + "\\" + argList[0];
                }
            }

            return "";
        }

        private string GetCpuUsage(string processName)
        {
            using (PerformanceCounter pcProcess = new PerformanceCounter("Process", "% Processor Time", processName))
            {
                return pcProcess.NextValue().ToString();
            }
        }

        private void UpdateProcessDataGrid()
        {
            _processDescriptions.Clear();

            var processes = System.Diagnostics.Process.GetProcesses().OrderBy(process => process.ProcessName).ToList();
            foreach (var process in processes)
            {
                ProcessDescription newProcessDescription = new ProcessDescription();
                try
                {
                    newProcessDescription.Name = process.ProcessName;
                    newProcessDescription.Id = process.Id;
                    newProcessDescription.State = process.Responding ? "Running" : "Stoped";
                    newProcessDescription.Memory = process.VirtualMemorySize64 / 1024;
                    newProcessDescription.Owner = GetProcessOwner(process.Id);
                    newProcessDescription.CpuUsage = GetCpuUsage(process.ProcessName);
                    newProcessDescription.Despription = process.MainModule.FileVersionInfo.FileDescription;

                    _processDescriptions.Add(newProcessDescription);
                }
                catch (Win32Exception)
                {
                    _processDescriptions.Add(newProcessDescription);
                }
                catch (InvalidOperationException)
                {
                    _processDescriptions.Add(newProcessDescription);
                }
            }

            processesDataGrid.ItemsSource = _processDescriptions;
        }

        private void ProcessesDataGridPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedProcess = processesDataGrid.SelectedItem as ProcessDescription;

                try
                {
                    Process.GetProcessById(selectedProcess.Id).Kill();
                }
                catch (Win32Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                catch (InvalidOperationException exception)
                {
                    MessageBox.Show(exception.Message);
                }
                catch (NotSupportedException exception)
                {
                    MessageBox.Show(exception.Message);
                }
                catch(ArgumentException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        //private void processesDataGrid_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Delete)
        //    {
        //        var selectedProcess = processesDataGrid.SelectedItem as ProcessDescription;

        //        try
        //        {
        //            Process.GetProcessById(selectedProcess.Id).Kill();
        //        }
        //        catch (Win32Exception exception)
        //        {
        //            MessageBox.Show(exception.Message);
        //        }
        //        catch (InvalidOperationException exception)
        //        {
        //            MessageBox.Show(exception.Message);
        //        }
        //        catch (NotSupportedException exception)
        //        {
        //            MessageBox.Show(exception.Message);
        //        }
        //        catch (ArgumentException exception)
        //        {
        //            MessageBox.Show(exception.Message);
        //        }
        //    }
        //}
    }
}

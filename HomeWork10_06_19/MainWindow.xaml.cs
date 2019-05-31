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

            var processes = System.Diagnostics.Process.GetProcesses();
            foreach(var process in processes)
            {
                ProcessDescription newProcessDescription = new ProcessDescription();
                newProcessDescription.Name = process.ProcessName;
                newProcessDescription.Id = process.Id;
                newProcessDescription.State = process.Responding ? "Running" : "Stoped";

                _processDescriptions.Add(newProcessDescription);
            }

            processesDataGrid.ItemsSource = _processDescriptions;
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

            return "NO OWNER";
        }
    }
}

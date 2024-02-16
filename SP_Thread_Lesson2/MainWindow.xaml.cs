using Microsoft.Win32;
using SP_Thread_Lesson2.Commands;
using SP_Thread_Lesson2.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

namespace SP_Thread_Lesson2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, NotificationService
    {
        private string from;

        public event PropertyChangedEventHandler? PropertyChanged;

        private long Max;

        public long Maximum { get { return Max; } set { Max = value; OnPropertyChanged(); } }

        private long valu;

        public long Values { get { return valu; } set { valu = value; OnPropertyChanged(); } }
        public string From { get => from; set { from = value; OnPropertyChanged(); } }
        public string To { get; set; }

        public ICommand FromCommand { get; set; }
        public ICommand ToCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public MainWindow()
        {

            InitializeComponent();
            DataContext = this;
            FromCommand = new RelayCommand(Exucute, CanExucute);
            ToCommand = new RelayCommand(ToExucute, ToCanExucute);
            CopyCommand = new RelayCommand(CopyExucute, CopyCanExucute);
            Maximum = 1;
        }

        


        public void Copy_S()
        {
           
            string srcPath = From;
            string destPath = To;

            // Delete the file if it exists.
            if (!File.Exists(srcPath))
            {
                Console.WriteLine("File not exists");
                return;
            }

            //Create the file.
            using (FileStream fsRead = new FileStream(srcPath, FileMode.Open, FileAccess.Read))
            {
                Console.WriteLine($"Size {fsRead.Length} bytes");
                using (FileStream fsWrite = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                {
                    var len = 10;
                    var fileSize = fsRead.Length;
                    byte[] buffer = new byte[len];
                    Maximum = fsRead.Length;
                    do
                    {
                        len = fsRead.Read(buffer, 0, buffer.Length); // 8
                        fsWrite.Write(buffer, 0, len);

                        Console.WriteLine(fileSize);
                        fileSize -= len;

                        Thread.Sleep(500);

                        Values = fsWrite.Length;
                    } while (len != 0);

                }
            }
        }

        private void CopyExucute(object? obj)
        {
            Thread thread = new Thread(Copy_S);
            thread.Start();
        }

        private bool CopyCanExucute(object? obj)
        {
            return true;
        }

        private bool ToCanExucute(object? obj)
        {
            return true;
        }

        private void ToExucute(object? obj)
        {
            To = Paths();
            ((TextBox)obj).Text = To;
        }

        private void Exucute(object? obj)
        {
            From = Paths();
            ((TextBox)obj).Text = From;
        }

        private bool CanExucute(object? obj)
        {
            return true;
        }

        public string Paths()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            return null;
        }

        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

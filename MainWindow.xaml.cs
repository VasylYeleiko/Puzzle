using Microsoft.Win32;
using System;
using System.Windows;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using Rectangle = System.Drawing.Rectangle;
using Point = System.Drawing.Point;
using System.IO;
using System.Collections.ObjectModel;

namespace Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		public MainWindow()
		{			
            MyCollection.collection = new ObservableCollection<Puzz>();
            InitializeComponent();            
            if (MyCollection.GetCollection() != null)
            {
                listbox.ItemsSource = MyCollection.GetCollection();
            }                
        }
        
        private void NewPick_Click(object sender, RoutedEventArgs e)
        {
            int divider = ((bool)r9.IsChecked) ? 4 : ((bool)r25.IsChecked) ? 6 : ((bool)r49.IsChecked) ? 8 : -1;            
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp";
            if (ofd.ShowDialog() != DialogResult.HasValue)
            {                
                Puzz p = new Puzz(ofd.FileName, divider);
                p.CreatePuzzle();                               
            }            
        }

        private void listbox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selected = (Puzz)listbox.SelectedItem;
            pc1.Source = new BitmapImage(new Uri(selected.path));
        }

        private void BeginPlay_Click(object sender, RoutedEventArgs e)
        {
            if ((Puzz)listbox.SelectedItem == null)
            {
                MessageBox.Show("Please select your puzzle","NOT SELECTED!",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
            else {
                Play play = new Play((Puzz)listbox.SelectedItem);
                this.Hide();
                play.ShowDialog();
                this.Show();
            }
        }
    }
}

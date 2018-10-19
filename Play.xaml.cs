using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;

namespace Puzzle
{    
    public partial class Play : Window
    {        
        private Puzz puzz = null;
        private List<Image> selected = new List<Image>();
        private List<Image> loaded = new List<Image>();
        private static Image global_sender;
        private List<Posted> posted = new List<Posted>();

        public Play(Puzz item)
        {
            InitializeComponent();            
            puzz = item;
            font.Source = new BitmapImage(new Uri(puzz.path));
            listPieces.ItemsSource = AddItems();
            PrepareField();            
        }

        private List<Piece> AddItems()
        {
            List<Piece> tmp = puzz.Pieces;
            for (int i = 0; i < tmp.Count / 2; i++)
            {                
                var a = tmp[0];
                if (i % 2 == 0)
                {
                    a = tmp[i];
                    tmp[i] = tmp[tmp.Count - 1 - i];
                    tmp[tmp.Count - 1 - i] = a;
                }
            }
            return tmp;
        }

        private void beginner_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)beginner.IsChecked)
                font.Opacity = 0.1;
            if ((bool)proff.IsChecked)
                font.Opacity = 0;
        }

        private void PrepareField()
        {
            var size = (puzz.Divider == 4) ? 192 : (puzz.Divider == 6) ? 128 : (puzz.Divider == 8) ? 96 : -1;
            
            int n = 0;
            for (var i = 0; i < puzz.Divider; i++)
            {
                for(var j = 0; j < puzz.Divider; j++)
                {
                    Image image = new Image();
                    image.Height = image.Width = size;
                    image.Name = "img_"+n;
                    image.Margin = new Thickness(i*size,j*size , 0, 0);                                        
                    image.AllowDrop = true;  
                    can.Children.Add(image);                    
                    ((Image)can.Children[n]).DragEnter += Play_DragEnter;
                    ((Image)can.Children[n]).Drop += Play_Drop;
                    ((Image)can.Children[n]).MouseLeftButtonDown += Play_MouseLeftButtonDown;                    
                    ((Image)can.Children[n]).Source = new BitmapImage(new Uri(puzz.Pieces[1].Minipath));                    
                    ((Image)can.Children[n]).Opacity = 0;
                    n++;
                }                
            }
        }        

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image lbl = sender as Image;
            global_sender = lbl;
            DragDrop.DoDragDrop(lbl, lbl.Source, DragDropEffects.Move);
            ((Image)sender).Opacity = 0.4;
            CheckAvaible();
        }

        private void Play_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((Image)sender).Opacity != 0)
            {
                Image lbl = sender as Image;
                global_sender = lbl;
                DragDrop.DoDragDrop(lbl, lbl.Source, DragDropEffects.All);
                ((Image)sender).Opacity = 0;
            }
            CheckAvaible();
        }

        private void Play_Drop(object sender, DragEventArgs e)
        {            
            ((Image)sender).Source = global_sender.Source;
            ((Image)sender).Opacity = 1;
            var senderId = ((Image)sender).Name.Split('_').Last();
            var targetId = global_sender.Source.ToString().Split('/').Last().Split('-').First();
            if (senderId == targetId)
            {
                ((Image)sender).AllowDrop = false;
                ((Image)sender).MouseLeftButtonDown -= Play_MouseLeftButtonDown;
                ((Image)sender).Drop -= Play_Drop;
                ((Image)sender).DragEnter -= Play_DragEnter;
            }
             if(CheckWin())
                MessageBox.Show("Congratulations");            
        }
         
        private void Play_DragEnter(object sender, DragEventArgs e)
        {           
            e.Effects = DragDropEffects.All;            
        }

        private bool CheckWin()
        {
            int res = 0;
            string ImName;
            string CanPic;
            for (var i = 0; i < can.Children.Count; i++)
            {
                ImName = ((Image)can.Children[i]).Source.ToString().Split('/').Last().Split('-').First();
                CanPic = ((Image)can.Children[i]).Name.Split('_').Last();
                if (ImName == CanPic)
                    res++;
            }
            if (res == (puzz.Divider * puzz.Divider))
                return true;
            return false;
        }

        private void Again_btn_Click(object sender, RoutedEventArgs e)
        {
            foreach(var item in loaded)
            {
                item.Opacity = 1;
            }
            can.Children.Clear();
            posted.Clear();
            PrepareField();
        }

        private void CheckAvaible()
        {
            for (var i = 0; i < selected.Count; i++)
            {
                bool check = false;

                for (var j = 0; j < can.Children.Count; j++)
                {
                    if (selected[i].Source == ((Image)can.Children[j]).Source) check = true;
                }
                if (!check)
                {
                    selected[i].Opacity = 1;
                }
            }              
        }

        private void im_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(!selected.Contains((Image)sender))
                selected.Add((Image)sender);         
        }

        private async void AutoComplete_btn_Click(object sender, RoutedEventArgs e)
        {            
            for (var i = 0; i < loaded.Count; i++)
            {
                var loadedName = loaded[i].Source.ToString().Split('/').Last().Split('-').First();
                var targetName = "";
                
                for (var j = 0; j < can.Children.Count; j++)
                {                    
                    targetName = ((Image)can.Children[j]).Name.Split('_').Last();
                    if (loadedName == targetName)
                    {                       
                        await DoWork2(i, j);
                        Thread.Sleep(100);   
                        break;                        
                    }                    
                }
            }
            if(CheckWin())
                MessageBox.Show("Congratulations");           
        }             

        private void im_Loaded(object sender, RoutedEventArgs e)
        {
            loaded.Add((Image)sender);
        }


        /*-------------- my Second chance ---------------*/

        //put correct puzzle on field
        private async Task DoWork2(int position,int index)
        {
            await DoShow(position, index - 1);
            await DoShow(position, index - 2);
            await DoShow(position, index - 3);

            await Task.Run(() => 
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    ((Image)can.Children[position]).Source = loaded[index].Source;
                    can.Children[position].Opacity = 1;
                    loaded[index].Opacity = 0.4;
                }), DispatcherPriority.ContextIdle);
            });
        }

        //put together puzzles via color detection
        private async void newAutoComplete_Click(object sender, RoutedEventArgs e)
        {
            int position = puzz.Pieces.Count / 2+puzz.Divider/2;
            int index = 0;
            int tryAnother = 0;
            //ставимо першу пузлю
            posted.Add(new Posted(position, index));
            ((Image)can.Children[position]).Source = loaded[index].Source;
            can.Children[position].Opacity = 1;
            
            var last = posted.Last();
            loaded[last.Index].Opacity = 0.4;

            while (posted.Count != puzz.Pieces.Count)
            {
                int postedCount = posted.Count();

                for (int i = 0; i < puzz.Pieces.Count; i++)
                {
                    bool canSearch = true;
                    for (int j = 0; j < posted.Count; j++)
                    {
                        if (i == posted[j].Index) canSearch = false;
                    }
                    if (!canSearch) continue;
                    switch (IsNeighbor(puzz.Pieces[last.Index], puzz.Pieces[i]))
                    {
                        case "left":
                            Thread.Sleep(100);

                            if (position < puzz.Divider)
                            {
                                if (MoveRight())
                                    position += puzz.Divider;
                                else break;
                            }

                            await DoWork2(position - puzz.Divider, i);
                            position -= puzz.Divider;                            
                            postedAddNew(position, i);                            
                            last = posted.Last();
                            break;
                        case "right":
                            Thread.Sleep(100);

                            if (position > can.Children.Count - puzz.Divider)
                            {
                                if (MoveLeft())
                                    position -= puzz.Divider;
                                else break;
                            }

                            await DoWork2(position + puzz.Divider, i);
                            position += puzz.Divider;                            
                            postedAddNew(position, i);                            
                            last = posted.Last();
                            break;
                        case "top":
                            Thread.Sleep(100);

                            if (position % puzz.Divider == 0)
                            {
                                if (MoveDown())
                                    position++;
                                else break;
                            }

                            await DoWork2(position -1, i);
                            position--;                            
                            postedAddNew(position, i);                            
                            last = posted.Last();
                            break;
                        case "bottom":
                            Thread.Sleep(100);

                            if (((position + 1) % puzz.Divider) == 0)
                            {
                                if (MoveUp())
                                    position--;
                                else break;
                            }
                            
                            await DoWork2(position + 1, i);
                            position++;                            
                            postedAddNew(position, i);                            
                            last = posted.Last(); 
                            break;                        
                    }
                }
                if (posted.Count == postedCount)
                {
                    last = posted[tryAnother];
                    position = last.Place;
                    tryAnother = (tryAnother < posted.Count-1) ? tryAnother + 1 : 0;
                }
                // Check();
                if (CheckWin()) MessageBox.Show("Congratulations");
            }
        }

        //do show before it put correct puzzl
        private async Task DoShow(int position,int index)
        {
            await Task.Run(() =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                try
                {
                    ((Image)can.Children[position]).Source = loaded[index].Source;
                        can.Children[position].Opacity = 1;
                        Thread.Sleep(50);
                    }
                    catch (Exception ex) { }                   
                   
                }), DispatcherPriority.ContextIdle);
            });
        }
        
        //check if two pictures are neighbor and return mutual side or "null" if they ar'nt neighbors
        private string IsNeighbor(Piece a, Piece b)
        {
            int l = 0;
            int r = 0;
            int t = 0;
            int d = 0;

            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < a.LeftSide.Count; j++)
                {
                    switch (i)
                    {
                        case 0:
                            if (Same(a.LeftSide[j], b.RightSide[j])) l++;
                            break;
                        case 1:
                            if (Same(a.RightSide[j], b.LeftSide[j])) r++;
                            break;
                        case 2:
                            if (Same(a.UpSide[j], b.BottomSide[j])) t++;
                            break;
                        case 3:
                            if (Same(a.BottomSide[j], b.UpSide[j])) d++;
                            break;
                    }
                }
            }
            if (l > r && l > t && l > d && l >= a.LeftSide.Count - a.LeftSide.Count / 2) return "left";
            if (r > l && r > t && r > d && r >= a.LeftSide.Count - a.LeftSide.Count / 2) return "right";
            if (t > r && t > l && t > d && t >= a.LeftSide.Count - a.LeftSide.Count / 2) return "top";
            if (d > r && d > t && d > l && d >= a.LeftSide.Count - a.LeftSide.Count / 2) return "bottom";

            return "null";
        }

        //check if the two pixels are semi equivalent return true or false        
        private bool Same(MyColor a,MyColor b)
        {
            if (a.a >= b.a - 3 && a.a <= b.a + 3 && a.r >= b.r - 3 && a.r <= b.r + 3 && a.g >= b.g - 3 && a.g <= b.g + 3 && a.b >= b.b - 3 && a.b <= b.b + 3)
            {
                return true;
            }            
            return false;
        }

        //move all picture up
        private bool MoveUp()
        {
            var Can = true;
            for(var i = 0; i < puzz.Divider; i++)
            {
                if (can.Children[i * puzz.Divider].Opacity == 1) Can = false;
            }
            if (Can)
            {
                for(int i = 0; i < can.Children.Count; i++)
                {
                    if (can.Children[i].Opacity == 1)
                    {
                        ((Image)can.Children[i - 1]).Source = ((Image)can.Children[i]).Source;
                        can.Children[i - 1].Opacity = 1;
                        can.Children[i].Opacity = 0;
                    }
                }
                for(int i = 0; i < posted.Count; i++)
                {
                    posted[i].Place -= 1;
                }
            }
            return Can;
        }

        //move all picture down
        private bool MoveDown()
        {
            var Can = true;
            for (var i = 0; i < puzz.Divider; i++)
            {
                if (can.Children[(i * puzz.Divider+puzz.Divider-1)].Opacity == 1) Can = false;
            }
            if (Can)
            {
                for (int i = can.Children.Count-1; i >=0 ; i--)
                {
                    if (can.Children[i].Opacity == 1)
                    {
                        ((Image)can.Children[i + 1]).Source = ((Image)can.Children[i]).Source;
                        can.Children[i + 1].Opacity = 1;
                        can.Children[i].Opacity = 0;
                    }
                }
                for (int i = 0; i < posted.Count; i++)
                {
                    posted[i].Place += 1;
                }
            }
            return Can;
        }

        //move all picture left
        private bool MoveLeft()
        {
            var Can = true;
            for (var i = 0; i < puzz.Divider; i++)
            {
                    if (can.Children[i].Opacity == 1) Can = false;
            }
            if (Can)
            {
                for (int i = 0; i <can.Children.Count; i++)
                {
                    if (can.Children[i].Opacity == 1)
                    {
                        ((Image)can.Children[i -puzz.Divider]).Source = ((Image)can.Children[i]).Source;
                        can.Children[i -puzz.Divider].Opacity = 1;
                        can.Children[i].Opacity = 0;

                    }
                }
                for (int i = 0; i < posted.Count; i++)
                {
                    posted[i].Place -= puzz.Divider;
                }
            }
            return Can;
        }

        //move all picture right
        private bool MoveRight()
        {
            var Can = true;
            for (var i = can.Children.Count-1; i >= can.Children.Count - puzz.Divider; i--)
            {
                if (can.Children[i].Opacity == 1) Can = false;
            }
            if (Can)
            {
                for (int i = can.Children.Count-1; i >=0 ; i--)
                {
                    if (can.Children[i].Opacity == 1)
                    {
                        ((Image)can.Children[i + puzz.Divider]).Source = ((Image)can.Children[i]).Source;
                        can.Children[i + puzz.Divider].Opacity = 1;
                        can.Children[i].Opacity = 0;
                    }
                }
                for (int i = 0; i < posted.Count; i++)
                {
                    posted[i].Place += puzz.Divider;
                }
            }
            return Can;
        }

        //add new posted item to posted list or rewright current if their place are same
        private void postedAddNew(int place,int index)
        {
            bool add = true;
            for(int i = 0; i < posted.Count; i++)
            {
                if (posted[i].Place == place)
                {
                    posted[i].Index = index;
                    add = false;
                    break;
                }               
            }
            if (add)
            {
                posted.Add(new Posted(place, index));
            }
        }

        //private void Check()
        //{

        //    for (int i = 0; i < posted.Count; i++)
        //    {
        //        int countNeighbor = 0;
        //        int countFriend = 0;
        //        int up = -1;
        //        int down = -1;
        //        int left = -1;
        //        int right = -1;
        //        bool leftFriend = false;
        //        bool rightFriend = false;
        //        bool upFriend = false;
        //        bool downFriend = false;


        //        //if current puzzl position is on any corner
        //        if (posted[i].Place == 0)
        //        {
        //            right = GetRightNeighbor(i);
        //            if (right >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[right]) != "null")
        //                {
        //                    rightFriend = true;
        //                    countFriend++;
        //                }
        //            }
        //            down = GetBottomNeighbor(i);
        //            if (down >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[down]) != "null")
        //                {
        //                    countFriend++;
        //                    downFriend = true;
        //                }
        //            }
        //            MessageBox.Show("Place-" + posted[i].Place + " Picture-" + posted[i].Index + " countNeighbor-" + countNeighbor + " countFriends-" + countFriend);
        //            if (countFriend != countNeighbor)
        //            {
        //                if (!rightFriend)
        //                    RemoveFromField(right);
        //                if (!downFriend)
        //                    RemoveFromField(down);
        //            }
        //        }

        //        if (posted[i].Place == puzz.Pieces.Count - puzz.Divider)
        //        {
        //            left = GetLeftNeighbor(i);
        //            if (left >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[left]) != "null")
        //                {
        //                    countFriend++;
        //                    leftFriend = true;
        //                }
        //            }
        //            down = GetBottomNeighbor(i);
        //            if (down >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[down]) != "null")
        //                {
        //                    countFriend++;
        //                    downFriend = true;
        //                }
        //            }
        //            MessageBox.Show("Place-" + posted[i].Place + " Picture-" + posted[i].Index + " countNeighbor-" + countNeighbor + " countFriends-" + countFriend);
        //            if (countFriend != countNeighbor)
        //            {
        //                if (!leftFriend)
        //                    RemoveFromField(left);
        //                if (!downFriend)
        //                    RemoveFromField(down);
        //            }
        //        }

        //        if (posted[i].Place == puzz.Divider - 1)
        //        {
        //            up = GetTopNeighbor(i);
        //            if (up >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[up]) != "null")
        //                {
        //                    countFriend++;
        //                    upFriend = true;
        //                }
        //            }
        //            right = GetRightNeighbor(i);
        //            if (right >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[right]) != "null")
        //                {
        //                    countFriend++;
        //                    rightFriend = true;
        //                }
        //            }
        //            MessageBox.Show("Place-" + posted[i].Place + " Picture-" + posted[i].Index + " countNeighbor-" + countNeighbor + " countFriends-" + countFriend);
        //            if (countFriend != countNeighbor)
        //            {
        //                if (!upFriend)
        //                    RemoveFromField(up);
        //                if (!rightFriend)
        //                    RemoveFromField(right);
        //            }
        //        }

        //        if (posted[i].Place == puzz.Pieces.Count - 1)
        //        {
        //            up = GetTopNeighbor(i);
        //            if (up >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[up]) != "null")
        //                {
        //                    countFriend++;
        //                    upFriend = true;
        //                }
        //            }
        //            left = GetLeftNeighbor(i);
        //            if (left >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[left]) != "null")
        //                {
        //                    countFriend++;
        //                    leftFriend = true;
        //                }
        //            }
        //            MessageBox.Show("Place-" + posted[i].Place + " Picture-" + posted[i].Index + " countNeighbor-" + countNeighbor + " countFriends-" + countFriend);
        //            if (countFriend != countNeighbor)
        //            {
        //                if (!upFriend)
        //                    RemoveFromField(up);
        //                if (!leftFriend)
        //                    RemoveFromField(left);
        //            }
        //        }
        //        //if current puzzl position is on one of four sides
        //        if (posted[i].Place < puzz.Divider && posted[i].Place > 0)
        //        {
        //            up = GetTopNeighbor(i);
        //            if (up >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[up]) != "null")
        //                {
        //                    countFriend++;
        //                    upFriend = true;
        //                }
        //            }
        //            right = GetRightNeighbor(i);
        //            if (right >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[right]) != "null")
        //                {
        //                    countFriend++;
        //                    rightFriend = true;
        //                }
        //            }
        //            down = GetBottomNeighbor(i);
        //            if (down >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[down]) != "null")
        //                {
        //                    countFriend++;
        //                    downFriend = true;
        //                }
        //            }
        //            MessageBox.Show("Place-" + posted[i].Place + " Picture-" + posted[i].Index + " countNeighbor-" + countNeighbor + " countFriends-" + countFriend);
        //            if (countFriend != countNeighbor)
        //            {
        //                if (!upFriend)
        //                    RemoveFromField(up);
        //                if (!rightFriend)
        //                    RemoveFromField(right);
        //                if (!downFriend)
        //                    RemoveFromField(down);
        //            }

        //        }
        //        if (posted[i].Place % puzz.Divider == 0 && posted[i].Place > 0 && posted[i].Place < puzz.Pieces.Count - puzz.Divider)
        //        {
        //            right = GetRightNeighbor(i);
        //            if (right >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[right]) != "null")
        //                {
        //                    countFriend++;
        //                    rightFriend = true;
        //                }
        //            }
        //            down = GetBottomNeighbor(i);
        //            if (down >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[down]) != "null")
        //                {
        //                    countFriend++;
        //                    downFriend = true;
        //                }
        //            }
        //            left = GetLeftNeighbor(i);
        //            if (left >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[left]) != "null")
        //                {
        //                    countFriend++;
        //                    leftFriend = true;
        //                }
        //            }
        //            MessageBox.Show("Place-" + posted[i].Place + " Picture-" + posted[i].Index + " countNeighbor-" + countNeighbor + " countFriends-" + countFriend);
        //            if (countFriend != countNeighbor)
        //            {
        //                if (!leftFriend)
        //                    RemoveFromField(left);
        //                if (!rightFriend)
        //                    RemoveFromField(right);
        //                if (!downFriend)
        //                    RemoveFromField(down);
        //            }
        //        }
        //        if (posted[i].Place + 1 % puzz.Divider == 0 && posted[i].Place != puzz.Divider - 1 && posted[i].Place != puzz.Pieces.Count - 1)
        //        {
        //            up = GetTopNeighbor(i);
        //            if (up >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[up]) != "null")
        //                {
        //                    countFriend++;
        //                    upFriend = true;
        //                }
        //            }
        //            left = GetLeftNeighbor(i);
        //            if (left >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[left]) != "null")
        //                {
        //                    countFriend++;
        //                    leftFriend = true;
        //                }
        //            }
        //            right = GetRightNeighbor(i);
        //            if (right >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[right]) != "null")
        //                {
        //                    countFriend++;
        //                    rightFriend = true;
        //                }
        //            }
        //            MessageBox.Show("Place-" + posted[i].Place + " Picture-" + posted[i].Index + " countNeighbor-" + countNeighbor + " countFriends-" + countFriend);
        //            if (countFriend != countNeighbor)
        //            {
        //                if (!upFriend)
        //                    RemoveFromField(up);
        //                if (!rightFriend)
        //                    RemoveFromField(right);
        //                if (!leftFriend)
        //                    RemoveFromField(left);
        //            }
        //        }
        //        if (posted[i].Place > puzz.Pieces.Count - puzz.Divider - 1 && posted[i].Place != puzz.Pieces.Count - 1 && posted[i].Place < puzz.Pieces.Count - puzz.Divider)
        //        {
        //            up = GetTopNeighbor(i);
        //            if (up >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[up]) != "null")
        //                {
        //                    countFriend++;
        //                    upFriend = true;
        //                }
        //            }
        //            down = GetBottomNeighbor(i);
        //            if (down >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[down]) != "null")
        //                {
        //                    countFriend++;
        //                    downFriend = true;
        //                }
        //            }
        //            left = GetLeftNeighbor(i);
        //            if (left >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[left]) != "null")
        //                {
        //                    countFriend++;
        //                    leftFriend = true;
        //                }
        //            }
        //            MessageBox.Show("Place-" + posted[i].Place + " Picture-" + posted[i].Index + " countNeighbor-" + countNeighbor + " countFriends-" + countFriend);
        //            if (countFriend != countNeighbor)
        //            {
        //                if (!upFriend)
        //                    RemoveFromField(up);
        //                if (!leftFriend)
        //                    RemoveFromField(left);
        //                if (!downFriend)
        //                    RemoveFromField(down);
        //            }
        //        }
        //        //if current position is anywhere on a field
        //        if (posted[i].Place > puzz.Divider && posted[i].Place % puzz.Divider != 0 && posted[i].Place < puzz.Pieces.Count - puzz.Divider && posted[i].Place + 1 % puzz.Divider != 0)
        //        {
        //            up = GetTopNeighbor(i);
        //            if (up >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[up]) != "null")
        //                {
        //                    countFriend++;
        //                    upFriend = true;
        //                }
        //            }

        //            down = GetBottomNeighbor(i);
        //            if (down >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[down]) != "null")
        //                {
        //                    countFriend++;
        //                    downFriend = true;
        //                }
        //            }
        //            left = GetLeftNeighbor(i);
        //            if (left >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[left]) != "null")
        //                {
        //                    countFriend++;
        //                    leftFriend = true;
        //                }
        //            }
        //            right = GetRightNeighbor(i);
        //            if (right >= 0)
        //            {
        //                countNeighbor++;
        //                if (IsNeighbor(puzz.Pieces[posted[i].Index], puzz.Pieces[right]) != "null")
        //                {
        //                    countFriend++;
        //                    rightFriend = true;
        //                }
        //            }
        //            MessageBox.Show("Place-" + posted[i].Place + " Picture-" + posted[i].Index + " countNeighbor-" + countNeighbor + " countFriends-" + countFriend);
        //            if (countFriend != countNeighbor)
        //            {
        //                if (upFriend == false)
        //                    RemoveFromField(up);
        //                if (!rightFriend)
        //                    RemoveFromField(right);
        //                if (!downFriend)
        //                    RemoveFromField(down);
        //                if (!leftFriend)
        //                    RemoveFromField(left);
        //            }
        //        }

        //    }
        //}

        //private int GetLeftNeighbor(int i)
        //{
        //    if (can.Children[posted[i].Place - puzz.Divider].Opacity == 1)
        //    {
        //        return posted.Where(x => x.Place == posted[i].Place - puzz.Divider).First().Index; 
        //    }
        //    return -1;
        //}

        //private int GetRightNeighbor(int i)
        //{
        //    if (can.Children[posted[i].Place + puzz.Divider].Opacity == 1)
        //    {
        //        return posted.Where(x => x.Place == posted[i].Place + puzz.Divider).First().Index;
        //    }
        //    return -1;
        //}

        //private int GetBottomNeighbor(int i)
        //{
        //    if (can.Children[posted[i].Place +1].Opacity == 1)
        //    {
        //        return posted.Where(x => x.Place == posted[i].Place +1).First().Index;
        //    }
        //    return -1;
        //}

        //private int GetTopNeighbor(int i)
        //{
        //    if (can.Children[posted[i].Place - 1].Opacity == 1)
        //    {
        //        return posted.Where(x => x.Place == posted[i].Place - 1).First().Index;
        //    }
        //    return -1;
        //}

        //

        //

    }
}

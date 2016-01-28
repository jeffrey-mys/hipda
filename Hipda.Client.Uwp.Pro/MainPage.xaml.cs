﻿using Hipda.Client.Uwp.Pro.Controls;
using Hipda.Client.Uwp.Pro.Models;
using Hipda.Client.Uwp.Pro.Services;
using Hipda.Client.Uwp.Pro.ViewModels;
using Hipda.Client.Uwp.Pro.Views;
using System;
using System.IO;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Hipda.Client.Uwp.Pro
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SettingsModel _settings;
        SettingsDependencyObject _mySettings;
        MainPageViewModel _mainPageViewModel;
        ThreadHistoryListViewViewModel _threadHistoryListViewViewModel;

        public Frame AppFrame { get { return this.MainFrame; } }

        void ElementAdapter()
        {
            PromptAllBorder.Visibility = ActualWidth >= 1000 ? Visibility.Collapsed : Visibility.Visible;
            BottomButtonPanel.Orientation = MainSplitView.IsPaneOpen ? Orientation.Horizontal : Orientation.Vertical;
        }

        /// <summary>
        /// 读取并启用设置
        /// </summary>
        void InitSettings()
        {
            _settings = SettingsService.Read();
            if (_settings.ThemeType == -1)
            {
                _settings.ThemeType = App.Current.RequestedTheme == ApplicationTheme.Light ? 0 : 1;
                SettingsService.Save();
            }

            _mySettings = ((SettingsDependencyObject)App.Current.Resources["MySettings"]);
            _mySettings.ThemeType = _settings.ThemeType;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            switch (_mySettings.ThemeType)
            {
                case 0:
                    this.RequestedTheme = ElementTheme.Light;
                    titleBar.BackgroundColor = null;
                    titleBar.InactiveBackgroundColor = null;
                    titleBar.ForegroundColor = null;
                    titleBar.ButtonBackgroundColor = null;
                    titleBar.ButtonInactiveBackgroundColor = null;
                    titleBar.ButtonForegroundColor = null;
                    titleBar.ButtonHoverBackgroundColor = null;

                    _mySettings.PictureOpacity = 1;
                    break;
                case 1:
                    this.RequestedTheme = ElementTheme.Dark;
                    Color c = Colors.Black;
                    titleBar.BackgroundColor = c;
                    titleBar.InactiveBackgroundColor = c;
                    titleBar.ForegroundColor = Colors.Silver;
                    titleBar.ButtonBackgroundColor = c;
                    titleBar.ButtonInactiveBackgroundColor = c;
                    titleBar.ButtonForegroundColor = Colors.Silver;
                    titleBar.ButtonHoverBackgroundColor = Colors.DimGray;

                    _mySettings.PictureOpacity = _settings.PictureOpacity;
                    break;
            }

            _mySettings.FontSize1 = _settings.FontSize1;
            _mySettings.FontSize2 = _settings.FontSize2;
            _mySettings.LineHeight = _settings.LineHeight;
            _mySettings.PictureOpacityBak = _settings.PictureOpacity;
            _mySettings.CanShowTopThread = _settings.CanShowTopThread;
            _mySettings.BlockUsers = _settings.BlockUsers;
            _mySettings.BlockThreads = _settings.BlockThreads;
        }

        public MainPage()
        {
            this.InitializeComponent();
            InitSettings();

            _mainPageViewModel = MainPageViewModel.GetInstance();
            DataContext = _mainPageViewModel;

            _threadHistoryListViewViewModel = new ThreadHistoryListViewViewModel();
            RightSideWrap.DataContext = _threadHistoryListViewViewModel;

            this.SizeChanged += (s, e) =>
            {
                ElementAdapter();
            };

            this.Loaded += (s, e) =>
            {
                ElementAdapter();
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string param = e.Parameter.ToString();
            AppFrame.Navigate(typeof(ThreadAndReplyPage), param);

            _mainPageViewModel.SelectedNavButton = _mainPageViewModel.NavButtons.FirstOrDefault(b => b.TypeValue.Equals(param));
        }

        public async void ShowTipBar(string tipContent)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                TipTextBlock.Text = Uri.UnescapeDataString(tipContent);
                ShowTipBarAnimation.Begin();
            });
        }

        private void MainSplitViewToggle_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
            ElementAdapter();
        }

        private void MainSplitView_PaneClosed(SplitView sender, object args)
        {
            SearchPanel.Visibility = Visibility.Collapsed;
            ElementAdapter();
        }

        void ShowLeftSwipePanel()
        {
            FindName("MaskGrid");
            FindName("LeftSwipePanel");
            FindName("RightSwipePanel");

            if (MainSplitView.DisplayMode == SplitViewDisplayMode.Overlay || MainSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                // 以免 pane 挡住 left swipe panel
                MainSplitView.IsPaneOpen = false;
                ElementAdapter();
            }
            
            MaskGrid.Visibility = Visibility.Visible;
            OpenMaskAnimation.Begin();
            OpenLeftSwipePanelAnimation.Begin();
            CloseRightSwipePanelAnimation.Begin();
        }

        void HideLeftSwipePanel()
        {
            CloseMaskAnimation.Begin();
            CloseLeftSwipePanelAnimation.Begin();
            MaskGrid.Visibility = Visibility.Collapsed;

            var selectedItem = (NavButtonItemModel)TopNavButtonListBox.SelectedItem;
            if (selectedItem != null && selectedItem.TypeValue.Equals("more"))
            {
                TopNavButtonListBox.SelectedItem = null;
            }
        }

        void ShowRightSwipePanel()
        {
            FindName("MaskGrid");
            FindName("LeftSwipePanel");
            FindName("RightSwipePanel");

            if (MainSplitView.DisplayMode == SplitViewDisplayMode.Overlay || MainSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                // 以免 pane 挡住 left swipe panel
                MainSplitView.IsPaneOpen = false;
                ElementAdapter();
            }

            MaskGrid.Visibility = Visibility.Visible;

            OpenMaskAnimation.Begin();
            OpenRightSwipePanelAnimation.Begin();

            CloseLeftSwipePanelAnimation.Begin();
            TopNavButtonListBox.SelectedItem = null;
        }

        void HideRightSwipePanel()
        {
            CloseMaskAnimation.Begin();

            if (AdaptiveStates.CurrentState == Min1200 || AdaptiveStates.CurrentState == Min1400 || AdaptiveStates.CurrentState == Min1600)
            {
                // 宽视图（已显示历史记录）下，直接隐藏，无过渡
                CloseRightSwipePanelAnimation2.Begin();
            }
            else
            {
                CloseRightSwipePanelAnimation.Begin();
            }
            

            MaskGrid.Visibility = Visibility.Collapsed;

            // 保存设置
            SettingsService.Save();
        }

        private void TopNavButtonListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
            {
                return;
            }

            var data = e.AddedItems[0] as NavButtonItemModel;
            if (data.TypeValue.Equals("more"))
            {
                ShowLeftSwipePanel();
            }
            else
            {
                AppFrame.Navigate(typeof(ThreadAndReplyPage), data.TypeValue);
            }
        }

        private void TopNavButtonListBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // 如果事件源是来自“更多版块”按钮，则阻止事件继续向上冒泡并停止继续执行
            var item = Common.FindParent<ListBoxItem>(e.OriginalSource as FrameworkElement);
            if (item != null)
            {
                var data = item.DataContext as NavButtonItemModel;
                if (data.TypeValue.Equals("more"))
                {
                    e.Handled = true;
                    return;
                }
            }

            HideLeftSwipePanel();
            HideRightSwipePanel();
        }

        private void MaskGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HideLeftSwipePanel();
            HideRightSwipePanel();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchPanel.Visibility == Visibility.Collapsed)
            {
                MainSplitView.IsPaneOpen = true;
                ElementAdapter();

                SearchPanel.Visibility = Visibility.Visible;
                KeywordTextBox.Focus(FocusState.Programmatic);
            }
            else
            {
                SearchPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void KeywordTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SearchDefaultSubmit();
            }
        }

        private void HistoryRecordButton_Click(object sender, RoutedEventArgs e)
        {
            FindName("RightSwipePanel");
            RightSwipeContentControl.DataContext = _threadHistoryListViewViewModel;
            RightSwipeContentControl.ContentTemplate = Resources["HistoryRecordContentControl"] as DataTemplate;

            ShowRightSwipePanel();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            FindName("RightSwipePanel");
            RightSwipeContentControl.ContentTemplate = Resources["SettingsContentControl"] as DataTemplate;

            ShowRightSwipePanel();
        }

        private void SearchDefaultSubmit()
        {
            string paramFormat = "search={0},{1},{2},{3},1";

            string searchKeyword = Uri.EscapeUriString(KeywordTextBox.Text.Trim().Replace(",", " "));
            string searchAuthor = Uri.EscapeUriString(AuthorTextBox.Text.Trim().Replace(",", " "));
            int searchType = SearchTypeComboBox.SelectedIndex;
            int searchTimeSpan = SearchTimeSpanComboBox.SelectedIndex;

            string param = string.Format(paramFormat, searchKeyword, searchAuthor, searchType, searchTimeSpan);
            AppFrame.Navigate(typeof(ThreadAndReplyPage), param);
        }

        private void SearchButton1_Click(object sender, RoutedEventArgs e)
        {
            SearchDefaultSubmit();
        }

        private void SearchButton2_Click(object sender, RoutedEventArgs e)
        {
            string paramFormat = "search={0},{1},{2},{3},2";

            string searchKeyword = Uri.EscapeUriString(KeywordTextBox.Text.Trim().Replace(",", " "));
            string searchAuthor = Uri.EscapeUriString(AuthorTextBox.Text.Trim().Replace(",", " "));
            int searchType = SearchTypeComboBox.SelectedIndex;
            int searchTimeSpan = SearchTimeSpanComboBox.SelectedIndex;

            string param = string.Format(paramFormat, searchKeyword, searchAuthor, searchType, searchTimeSpan);
            AppFrame.Navigate(typeof(ThreadAndReplyPage), param);
        }

        private void NoticeButton_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.Navigate(typeof(ThreadAndReplyPage), "item=notice");
        }

        private void ReadListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var data = e.ClickedItem as ThreadItemModelBase;
            //OpenReplyPageByThreadId(data.ThreadId);

            //LeftListView.SelectedItem = null;
        }

        void SendToast(string toastXml)
        {
            toastXml = Common.ReplaceHexadecimalSymbols(toastXml);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(toastXml);

            // 创建通知实例
            var notification = new ToastNotification(xmlDoc);

            // 显示通知
            var tn = ToastNotificationManager.CreateToastNotifier();
            tn.Show(notification);
        }

        #region 头像上下文菜单
        //private async void openThreadInNewView_Tapped(object sender, TappedRoutedEventArgs e)
        //{
        //    var uri = new Uri("hipda:tid=" + MainPage.PopupThreadId);
        //    await Launcher.LaunchUriAsync(uri, new LauncherOptions { TreatAsUntrusted = false });
        //}

        private void OpenUserInfoDialogMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenUserInfoDialog();
        }

        private void OpenUserMessageDialogMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenUserMessageDialog();
        }

        private void BlockUserMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (PopupUserId > 0 && !string.IsNullOrEmpty(PopupUsername))
            {
                var mySettings = (SettingsDependencyObject)App.Current.Resources["MySettings"];
                mySettings.BlockUsers.Add(new BlockUser { UserId = PopupUserId, Username = PopupUsername, ForumId = PopupForumId, ForumName = PopupForumName });

                string _xml = "<toast>" +
                    "<visual>" +
                        "<binding template='ToastGeneric'>" +
                            $"<text>已将用户 {PopupUsername} 加入屏蔽名单</text>" +
                            $"<text>刷新后生效</text>" +
                        "</binding>" +
                    "</visual>" +
                    "</toast>";

                SendToast(_xml);
            }
        }

        private void BlockThreadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (PopupThreadId > 0 && !string.IsNullOrEmpty(PopupThreadTitle))
            {
                var mySettings = (SettingsDependencyObject)App.Current.Resources["MySettings"];
                mySettings.BlockThreads.Add(new BlockThread { UserId = PopupUserId, Username = PopupUsername, ThreadId = PopupThreadId, ThreadTitle = PopupThreadTitle, ForumId = PopupForumId, ForumName = PopupForumName });

                string _xml = "<toast>" +
                    "<visual>" +
                        "<binding template='ToastGeneric'>" +
                            $"<text>已将主题 《{PopupThreadTitle}》 加入屏蔽列表</text>" +
                            $"<text>刷新后生效</text>" +
                        "</binding>" +
                    "</visual>" +
                    "</toast>";

                SendToast(_xml);
            }
        }
        #endregion

        #region 坛友资料及短消息之弹窗
        public static int PopupUserId { get; set; }
        public static string PopupUsername { get; set; }
        public static int PopupThreadId { get; set; }
        public static string PopupThreadTitle { get; set; }
        public static int PopupForumId { get; set; }
        public static string PopupForumName { get; set; }

        bool _isDialogShown = false;

        public async void OpenUserInfoDialog()
        {
            if (PopupUserId == 0)
            {
                return;
            }

            FindName("UserDialog");
            UserDialog.DataContext = new ContentDialogForUserInfoViewModel(PopupUserId);
            UserDialog.Title = string.Format("查看 {0} 的详细资料", PopupUsername);
            UserDialog.ContentTemplate = this.Resources["UserInfoDialogContentTemplate"] as DataTemplate;

            if (_isDialogShown == false)
            {
                _isDialogShown = true;
                await UserDialog.ShowAsync();
            }
        }

        public async void OpenUserMessageDialog()
        {
            if (PopupUserId == 0)
            {
                return;
            }

            FindName("UserDialog");
            UserDialog.DataContext = new ContentDialogForUserMessageViewModel(PopupUserId);
            UserDialog.Title = string.Format("与 {0} 聊天", PopupUsername);
            UserDialog.ContentTemplate = this.Resources["UserMessageDialogContentTemplate"] as DataTemplate;

            if (_isDialogShown == false)
            {
                _isDialogShown = true;
                await UserDialog.ShowAsync();
            }
        }

        private async void UserMessageBox_Submit(object sender, EventArgs e)
        {
            UserMessageBox umb = sender as UserMessageBox;
            TextBox tb = umb.FindName("UserMessageTextBox") as TextBox;
            var msg = tb.Text.Trim();
            var vm = umb.DataContext as ContentDialogForUserMessageViewModel;
            bool isOk = await vm.PostUserMessage(msg, umb.UserId);
            tb.Text = string.Empty;

            // 发送完成后跳到列表底部
            var listView = Common.FindParent<Grid>(umb).Children[0] as ListView;
            if (listView.Items.Count > 0)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    listView.ScrollIntoView(listView.Items.Last());
                });
            }
        }

        public async void OpenPostDetailDialog(int postId, int threadId)
        {
            FindName("UserDialog");

            var vm = new ContentDialogForPostDetailViewModel(postId, threadId);
            if (vm == null)
            {
                return;
            }

            UserDialog.DataContext = vm;
            UserDialog.Title = "查看引用楼";
            UserDialog.ContentTemplate = this.Resources["PostDetailDialogContentTemplate"] as DataTemplate;

            if (_isDialogShown == false)
            {
                _isDialogShown = true;
                await UserDialog.ShowAsync();
            }
        }

        public async void OpenUserMessageListDialog()
        {
            FindName("UserDialog");

            var vm = new ContentDialogForUserMessageHubViewModel();
            if (vm == null)
            {
                return;
            }

            UserDialog.DataContext = vm;
            UserDialog.Title = "短消息";
            UserDialog.ContentTemplate = this.Resources["UserMessageListDialogContentTemplate"] as DataTemplate;

            if (_isDialogShown == false)
            {
                _isDialogShown = true;
                await UserDialog.ShowAsync();
            }
        }

        private void openUserInfoDialogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUserInfoDialog();
        }

        private void openUserMessageListButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUserMessageListDialog();
        }

        private void openUserMessageDialogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUserMessageDialog();
        }

        private void userMessageListListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (MyListViewForUserMessageList)sender;
            listView.SelectedUserMessageListItems = listView.SelectedItems;

            if (listView.SelectionMode == ListViewSelectionMode.Single && e.AddedItems.Count == 1)
            {
                var data = e.AddedItems[0] as UserMessageListItemModel;
                if (data == null)
                {
                    return;
                }

                PopupUserId = data.UserId;
                PopupUsername = data.Username;
                OpenUserMessageDialog();
            }
        }

        public void CloseUserDialog()
        {
            if (UserDialog != null)
            {
                _isDialogShown = false;
                UserDialog.Hide();
                UserDialog.DataContext = null;
            }

        }

        private void userDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            CloseUserDialog();
            ElementAdapter();
        }

        private void UserDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            CloseUserDialog();
        }


        #endregion

        private async void OpenImageFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("hipda", CreationCollisionOption.OpenIfExists);
            await Launcher.LaunchFolderAsync(folder);
        }
    }
}

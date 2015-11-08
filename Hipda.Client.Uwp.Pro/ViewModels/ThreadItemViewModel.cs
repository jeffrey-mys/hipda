﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hipda.Client.Uwp.Pro.Models;
using Windows.UI.Xaml.Data;
using Hipda.Client.Uwp.Pro.Commands;
using Hipda.Client.Uwp.Pro.Services;
using Windows.UI.Xaml.Controls;
using System.Threading;
using Windows.UI;
using Windows.UI.Xaml;

namespace Hipda.Client.Uwp.Pro.ViewModels
{
    public class ThreadItemViewModel : ThreadItemViewModelBase
    {
        private int _threadId { get; set; }
        private int _threadAuthorUserId { get; set; }
        private ListView _replyListView { get; set; }
        private Action _beforeLoad { get; set; }
        private Action<int> _afterLoad { get; set; }
        private Action<int> _linkClickEvent { get; set; }
        private DataService _ds { get; set; }

        private Style _statusColorStyle;

        public Style StatusColorStyle
        {
            get { return _statusColorStyle; }
            set
            {
                _statusColorStyle = value;
                this.RaisePropertyChanged("StatusColorStyle");
            }
        }

        public DelegateCommand RefreshReplyCommand { get; set; }

        public ThreadItemModel ThreadItem { get; set; }

        

        private ICollectionView _replyItemCollection;

        public ICollectionView ReplyItemCollection
        {
            get { return _replyItemCollection; }
            set
            {
                _replyItemCollection = value;
                this.RaisePropertyChanged("ReplyItemCollection");
            }
        }

        public List<FaceItem> FaceData
        {
            get
            {
                return FaceService.FaceData;
            }
        }

        private void LoadData(int pageNo)
        {
            var cv = _ds.GetViewForReplyPage(pageNo, _threadId, _threadAuthorUserId, _beforeLoad, _afterLoad, _linkClickEvent);
            if (cv != null)
            {
                ReplyItemCollection = cv;
            }
        }

        public ThreadItemViewModel(ThreadItemModel threadItem)
        {
            ThreadDataType = ThreadDataType.Default;
            _threadId = threadItem.ThreadId;
            _threadAuthorUserId = threadItem.AuthorUserId;
            ThreadItem = threadItem;
        }

        public ThreadItemViewModel(int pageNo, int threadId, int threadAuthorUserId, ListView replyListView, Action beforeLoad, Action<int> afterLoad, Action<int> linkClickEvent)
        {
            ThreadDataType = ThreadDataType.Default;
            _threadId = threadId;
            _threadAuthorUserId = threadAuthorUserId;
            _replyListView = replyListView;
            _beforeLoad = beforeLoad;
            _afterLoad = afterLoad;
            _linkClickEvent = linkClickEvent;
            _ds = new DataService();

            ThreadItem = _ds.GetThreadItem(threadId);

            RefreshReplyCommand = new DelegateCommand();
            RefreshReplyCommand.ExecuteAction = (p) =>
            {
                _ds.ClearReplyData(threadId);
                LoadData(1);
            };

            var cv = _ds.GetViewForReplyPage(pageNo, threadId, threadAuthorUserId, _beforeLoad, _afterLoad, _linkClickEvent);
            if (cv != null)
            {
                ReplyItemCollection = cv;
            }
        }

        public void SelectThreadItem(ListView replyListView, Action beforeLoad, Action<int> afterLoad, Action<int> linkClickEvent)
        {
            _replyListView = replyListView;
            _beforeLoad = beforeLoad;
            _afterLoad = afterLoad;
            _linkClickEvent = linkClickEvent;
            _ds = new DataService();

            RefreshReplyCommand = new DelegateCommand();
            RefreshReplyCommand.ExecuteAction = (p) => {
                _ds.ClearReplyData(_threadId);
                LoadData(1);
            };

            LoadData(1);
        }

        public void SetRead()
        {
            StatusColorStyle = (Style)App.Current.Resources["ReadColorStyle"];
        }

        public void RefreshReplyDataFromPrevPage()
        {
            // 先获取当前数据中已存在的最小页码
            int minPageNo = _ds.GetReplyMinPageNoInLoadedData(_threadId);
            int startPageNo = minPageNo > 1 ? minPageNo - 1 : 1;

            _ds.ClearReplyData(_threadId);
            LoadData(startPageNo);
        }
    }
}

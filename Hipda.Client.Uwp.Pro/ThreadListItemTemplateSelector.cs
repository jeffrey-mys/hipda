﻿using Hipda.Client.Uwp.Pro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipda.Client.Uwp.Pro
{
    public class ThreadListItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate MyThreadsTemplate { get; set; }
        public DataTemplate MyPostsTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            ThreadItemViewModelBase b = (ThreadItemViewModelBase)item;
            switch (b.ThreadDataType)
            {
                case ThreadDataType.MyThreads:
                    return MyThreadsTemplate;
                case ThreadDataType.MyPosts:
                    return MyPostsTemplate;
                default:
                    return NormalTemplate;
            }
        }
    }
}
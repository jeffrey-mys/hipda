﻿using Hipda.Client.Uwp.Pro.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Hipda.Client.Uwp.Pro.Models
{
    public class ReplyItemModel
    {
        public ReplyItemModel(int index, int floorNo, int postId, int pageNo, int threadId, string threadTitle, int threadAuthorUserId, int authorUserId, string authorUsername, string textContent, string htmlContent, string xamlConent, string authorCreateTime, int imageCount)
        {
            this.Index = index;
            this.FloorNo = floorNo;
            this.PostId = postId;
            this.PageNo = pageNo;
            this.ThreadId = threadId;
            this.ThreadTitle = threadTitle;
            this.ThreadAuthorUserId = threadAuthorUserId;
            this.AuthorUserId = authorUserId;
            this.AuthorUsername = authorUsername;
            this.TextStr = textContent;
            this.HtmlStr = htmlContent;
            this.XamlStr = xamlConent;
            this.AuthorCreateTime = authorCreateTime;
            this.ImageCount = imageCount;
        }

        public int Index { get; private set; }
        public int FloorNo { get; private set; }
        public int PostId { get; private set; }
        public int PageNo { get; private set; }
        public int ThreadId { get; private set; }
        public string ThreadTitle { get; private set; }
        public int ThreadAuthorUserId { get; private set; }
        public string AuthorUsername { get; private set; }

        public int AuthorUserId { get; private set; }

        public string TextStr { get; private set; }

        public string HtmlStr { get; private set; }

        public string XamlStr { get; set; }

        public string AuthorCreateTime { get; private set; }

        public int ImageCount { get; set; }

        public bool HasThreadTitle
        {
            get
            {
                return FloorNo == 1;
            }
        }

        public bool IsTopicStarter
        {
            get
            {
                return ThreadAuthorUserId == AuthorUserId;
            }
        }

        public override string ToString()
        {
            return this.HtmlStr;
        }

        public string FloorNoStr
        {
            get
            {
                return string.Format("{0}#", FloorNo);
            }
        }

        public object XamlContent
        {
            get
            {
                try
                {
                    return XamlReader.Load(XamlStr) as FrameworkElement;
                }
                catch
                {
                    Common.PostEmailForConvertReplyContent(ThreadId, FloorNo);

                    string text = Regex.Replace(TextStr, @"[^a-zA-Z\d\u4e00-\u9fa5]", " ");
                    XamlStr = string.Format("<RichTextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph>{0}</Paragraph></RichTextBlock>", text);
                    return XamlReader.Load(XamlStr);
                }
            }
        }

        public ImageBrush AvatarImageBrush
        {
            get
            {
                int uid = AuthorUserId;
                var s = new int[10];
                for (int i = 0; i < s.Length - 1; ++i)
                {
                    s[i] = uid % 10;
                    uid = (uid - s[i]) / 10;
                }
                var imgUri = new Uri("http://www.hi-pda.com/forum/uc_server/data/avatar/" + s[8] + s[7] + s[6] + "/" + s[5] + s[4] + "/" + s[3] + s[2] + "/" + s[1] + s[0] + "_avatar_middle.jpg");

                BitmapImage bi = new BitmapImage();
                bi.UriSource = imgUri;
                bi.DecodePixelWidth = 40;
                ImageBrush ib = new ImageBrush();
                ib.Stretch = Stretch.UniformToFill;
                ib.ImageSource = bi;
                ib.ImageFailed += (s2, e2) => { return; };
                return ib;
            }
        }
    }
}

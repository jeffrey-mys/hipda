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
        public ReplyItemModel(int index, int index2, int floorNo, int postId, int pageNo, int forumId, string forumName, int threadId, string threadTitle, int threadAuthorUserId, int authorUserId, string authorUsername, string textContent, string xamlConent, string authorCreateTime, int imageCount, bool isHighLight)
        {
            this.Index = index;
            this.Index2 = index;
            this.FloorNo = floorNo;
            this.PostId = postId;
            this.PageNo = pageNo;
            this.ForumId = forumId;
            this.ForumName = forumName;
            this.ThreadId = threadId;
            this.ThreadTitle = threadTitle;
            this.ThreadAuthorUserId = threadAuthorUserId;
            this.AuthorUserId = authorUserId;
            this.AuthorUsername = authorUsername;
            this.TextStr = textContent;
            this.XamlStr = xamlConent;
            this.AuthorCreateTime = authorCreateTime;
            this.ImageCount = imageCount;
            this.IsHighLight = isHighLight;
            this.IsLast = false;
        }

        public int Index { get; private set; }
        public int Index2 { get; private set; }
        public int FloorNo { get; private set; }
        public int PostId { get; private set; }
        public int PageNo { get; private set; }
        public int ForumId { get; private set; }
        public string ForumName { get; private set; }
        public int ThreadId { get; private set; }

        string _threadTitle;
        public string ThreadTitle
        {
            get
            {
                return Common.ReplaceEmojiLabel(_threadTitle);
            }
            set
            {
                _threadTitle = value;
            }
        }

        public int ThreadAuthorUserId { get; set; }
        public string AuthorUsername { get; private set; }

        public int AuthorUserId { get; private set; }

        public string TextStr { get; private set; }

        public string XamlStr { get; set; }

        public string AuthorCreateTime { get; private set; }

        public int ImageCount { get; set; }

        public bool IsHighLight { get; private set; }

        public bool IsLast { get; set; }

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

        public bool IsMine
        {
            get
            {
                return AuthorUserId == AccountService.UserId;
            }
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
                if (FloorNo == -1)
                {
                    // “---完---”项不作处理
                    return null;
                }

                try
                {
                    return XamlReader.Load(Common.ReplaceEmojiLabel(XamlStr));
                }
                catch
                {
                    string errorDetails = string.Format("http://www.hi-pda.com/forum/viewthread.php?tid={0} 楼层{1}内容解析出错。\r\n{2}", ThreadId, FloorNo, XamlStr);
                    Common.PostErrorEmailToDeveloper("回复内容解析出现异常", errorDetails);

                    string text = Regex.Replace(TextStr, @"[^a-zA-Z\d\u4e00-\u9fa5]", " ");
                    XamlStr = string.Format("<RichTextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph>{0}</Paragraph></RichTextBlock>", text);
                    return XamlReader.Load(XamlStr);
                }
            }
        }

        public Uri AvatarUri
        {
            get
            {
                return Common.GetSmallAvatarUriByUserId(AuthorUserId);
            }
        }
    }
}

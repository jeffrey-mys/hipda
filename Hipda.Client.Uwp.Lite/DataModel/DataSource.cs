﻿using Hipda.Client.Uwp.Lite;
using Hipda.Http;
using HipdaUwpLite.Common;
using HipdaUwpLite.Settings;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HipdaUwpLite.Data
{
    #region 实体类
    public class ContentForEdit
    {
        public ContentForEdit(string subject, string content, string postId, string threadId)
        {
            this.Subject = subject;
            this.Content = content;
            this.PostId = postId;
            this.ThreadId = threadId;
        }

        public string Subject { get; private set; }

        public string Content { get; private set; }

        public string PostId { get; private set; }

        public string ThreadId { get; private set; }
    }

    public class Reply
    {
        public Reply(int index, int floor, string postId, int pageNo, string threadId, string threadTitle, string ownerId, string ownerName, string textContent, string htmlContent, string xamlConent, int imageCount, string createTime)
        {
            this.Index = index;
            this.Floor = floor;
            this.PostId = postId;
            this.PageNo = pageNo;
            this.ThreadId = threadId;
            this.ThreadTitle = threadTitle;
            this.OwnerId = ownerId;
            this.OwnerName = ownerName;
            this.TextContent = textContent;
            this.HtmlContent = htmlContent;
            this.XamlContent = xamlConent;
            this.ImageCount = imageCount;
            this.CreateTime = createTime;
        }

        public int Index { get; private set; }
        public int Floor { get; private set; }
        public string PostId { get; private set; }
        public int PageNo { get; private set; }
        public string ThreadId { get; private set; }
        public string ThreadTitle { get; private set; }
        public string OwnerName { get; private set; }

        public string OwnerId { get; private set; }

        public string TextContent { get; private set; }

        public string HtmlContent { get; private set; }

        public string XamlContent { get; set; }

        public int ImageCount { get; set; }

        public string CreateTime { get; private set; }

        public bool HasThreadTitle
        {
            get 
            {
                return Floor == 1;
            }
        }

        public override string ToString()
        {
            return this.HtmlContent;
        }
        public string AvatarUrl
        {
            get
            {
                int uid = Convert.ToInt32(OwnerId);
                var s = new int[10];
                for (int i = 0; i < s.Length - 1; ++i)
                {
                    s[i] = uid % 10;
                    uid = (uid - s[i]) / 10;
                }
                return "http://www.hi-pda.com/forum/uc_server/data/avatar/" + s[8] + s[7] + s[6] + "/" + s[5] + s[4] + "/" + s[3] + s[2] + "/" + s[1] + s[0] + "_avatar_middle.jpg";
            }
        }

        public string FloorNumStr
        {
            get
            {
                return string.Format("{0}#", Floor);
            }
        }
    }

    public class ReplyData
    {
        public ReplyData(string threadId, string threadTitle)
        {
            this.ThreadId = threadId;
            this.ThreadTitle = threadTitle;
            this.Replies = new ObservableCollection<Reply>();
        }
        public string ThreadId { get; private set; }
        public string ThreadTitle { get; private set; }
        public IList<Reply> Replies { get; private set; }
    }

    public class Thread
    {
        public Thread(int index, int pageNo, string forumId, string id, string title, int attachType, string replyNum, string viewNum, string ownerName, string ownerId, string createTime, string lastPostAuthorName, string lastPostTime)
        {
            this.Index = index;
            this.PageNo = pageNo;
            this.ForumId = forumId;
            this.Id = id;
            this.Title = title;
            this.AttachType = attachType;
            this.ReplyNum = replyNum;
            this.ViewNum = viewNum;
            this.OwnerName = ownerName;
            this.OwnerId = ownerId;
            this.CreateTime = createTime;
            this.LastPostAuthorName = lastPostAuthorName;
            this.LastPostTime = lastPostTime;
        }

        public int Index { get; private set; }
        public int PageNo { get; private set; }
        public string ForumId { get; private set; }
        public string Id { get; private set; }
        public string Title { get; private set; }
        public int AttachType { get; private set; }
        public string ReplyNum { get; private set; }
        public string ViewNum { get; private set; }
        public string OwnerName { get; private set; }
        public string OwnerId { get; private set; }
        public string CreateTime { get; private set; }
        public string LastPostAuthorName { get; private set; }
        public string LastPostTime { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }

        public string AvatarUrl
        {
            get
            {
                if (string.IsNullOrEmpty(OwnerId))
                {
                    return string.Empty;
                }

                int uid = Convert.ToInt32(OwnerId);
                var s = new int[10];
                for (int i = 0; i < s.Length - 1; ++i)
                {
                    s[i] = uid % 10;
                    uid = (uid - s[i]) / 10;
                }
                return "http://www.hi-pda.com/forum/uc_server/data/avatar/" + s[8] + s[7] + s[6] + "/" + s[5] + s[4] + "/" + s[3] + s[2] + "/" + s[1] + s[0] + "_avatar_middle.jpg";
            }
        }

        public string Numbers
        {
            get
            {
                return string.Format("({0}/{1})", ReplyNum, ViewNum);
            }
        }

        public string LastPostInfo
        {
            get
            {
                return string.Format("{0} {1}", LastPostAuthorName, LastPostTime);
            }
        }
    }

    public class ThreadData
    {
        public ThreadData(string forumId, string forumName)
        {
            this.ForumId = forumId;
            this.ForumName = forumName;
            this.Threads = new ObservableCollection<Thread>();
        }
        public string ForumId { get; private set; }

        public string ForumName { get; private set; }

        public IList<Thread> Threads { get; private set; }
    }

    public class Forum
    {
        public Forum(string id, string name, string alias, string todayCount, string info)
        {
            this.Id = id;
            this.Name = name;
            this.Alias = alias;
            this.TodayQuantity = todayCount;
            this.Info = info;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Alias { get; private set; }
        public string Info { get; private set; }
        public string TodayQuantity { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
 
    }

    public class ForumGroup
    {
        public ForumGroup(string name)
        {
            this.Name = name;
            this.Forums = new ObservableCollection<Forum>();
        }

        public string Name { get; private set; }
        public ObservableCollection<Forum> Forums { get; private set; }
    }
    #endregion

    public sealed class DataSource
    {
        public static string LoginMessage = string.Empty;

        public static int ThreadPageSize { get { return 75; } }

        public static int SearchPageSize { get { return 50; } }

        public static int ReplyPageSize { get { return 50; } }

        /// <summary>
        /// 每个楼层默认显示的图片数据
        /// 用于节省流量
        /// </summary>
        public static int MaxImageCount { get { return ImageCountSettings.ImageCountSetting; } }

        public static string ThreadListPageOrderBy = string.Empty;
        public static int RelayListPageOrderType = 2;

        public static string MessageTail { get { return "[img=16,16]http://www.hi-pda.com/forum/attachments/day_140621/1406211752793e731a4fec8f7b.png[/img]"; } }

        private static DataSource _dataSource = new DataSource();

        private static HttpHandle httpClient = HttpHandle.GetInstance();

        /// <summary>
        /// 用于发布信息的 formhash 值
        /// </summary>
        private string _formHash = string.Empty;
        public static string FormHash
        {
            get { return _dataSource._formHash; }
        }

        /// <summary>
        /// 用户ID，用于上传图片
        /// </summary>
        private string _userId = string.Empty;
        public static string UserId
        {
            get { return _dataSource._userId; }
        }

        /// <summary>
        /// 用于上载图片所需的 hash 值
        /// </summary>
        private string _hash = string.Empty;
        public static string Hash
        {
            get { return _dataSource._hash; }
        }

        private ObservableCollection<ForumGroup> _forumGroups = new ObservableCollection<ForumGroup>();
        public ObservableCollection<ForumGroup> ForumGroups
        {
            get { return this._forumGroups; }
        }

        private ObservableCollection<ForumGroup> _fullForumGroups = new ObservableCollection<ForumGroup>();
        public ObservableCollection<ForumGroup> FullForumGroups
        {
            get { return this._fullForumGroups; }
        }

        private ObservableCollection<ThreadData> _threadList = new ObservableCollection<ThreadData>();
        public ObservableCollection<ThreadData> ThreadList
        {
            get { return this._threadList; }
        }

        private List<ReplyData> _replyList = new List<ReplyData>();
        public List<ReplyData> ReplyList
        {
            get { return this._replyList; }
        }

        private ContentForEdit _contentForEdit = null;
        public static ContentForEdit ContentForEdit
        {
            get { return _dataSource._contentForEdit; }
        }

        #region 读取论坛所有版块数据
        public static async Task<IEnumerable<ForumGroup>> GetForumGroupsAsync()
        {
            await _dataSource.LoadForumGroupDataAsync();

            return _dataSource.ForumGroups;
        }

        // 读取所以版区列表数据
        public async Task LoadForumGroupDataAsync()
        {
            if (this._forumGroups.Count > 0)
            {
                this.ForumGroups.Clear();
            }

            // 读取数据
            string url = "http://www.hi-pda.com/forum/index.php?_=" + DateTime.Now.Ticks.ToString("x");
            var cts = new CancellationTokenSource();
            string htmlContent = await httpClient.GetAsync(url, cts);

            // 实例化 HtmlAgilityPack.HtmlDocument 对象
            HtmlDocument doc = new HtmlDocument();

            // 载入HTML
            doc.LoadHtml(htmlContent);
            var data = doc.DocumentNode;

            var content = data.Descendants().FirstOrDefault(n => n.GetAttributeValue("class", "").Equals("content"));
            if (content == null)
            {
                return;
            }

            var mainBoxes = content.Descendants().Where(n => n.GetAttributeValue("class", "").Equals("mainbox list") && !n.GetAttributeValue("id", "").Equals("online"));
            if (mainBoxes == null)
            {
                return;
            }

            foreach (var mainBox in mainBoxes)
            {
                string forumGroupName = mainBox.ChildNodes[3].ChildNodes[0].InnerText;
                ForumGroup forumGroup = new ForumGroup(forumGroupName);

                var tbodies = mainBox.ChildNodes[5] // table
                    .Descendants().Where(n => n.GetAttributeValue("id", "").StartsWith("forum"));

                if (tbodies == null)
                {
                    return;
                }

                foreach (var tbody in tbodies)
                {
                    var divLeft = tbody
                        .ChildNodes[1] // tr
                        .ChildNodes[1] // th
                        .ChildNodes[1]; // div.left

                    var h2 = divLeft.ChildNodes[1]; // h2
                    var p = divLeft.ChildNodes[3]; // p

                    var a = h2.ChildNodes[0];
                    string forumId = a.Attributes[0].Value.Substring("forumdisplay.php?fid=".Length);
                    if (forumId.Contains("&"))
                    {
                        forumId = forumId.Split('&')[0];
                    }

                    string forumName = a.InnerText;
                    string forumAlias = forumName;
                    switch (forumId)
                    {
                        case "6":
                            forumAlias = "BS版";
                            break;
                        case "7":
                            forumAlias = "G版";
                            break;
                        case "14":
                            forumAlias = "Win版";
                            break;
                        case "2":
                            forumAlias = "地板";
                            break;
                        case "59":
                            forumAlias = "E版";
                            break;
                        default:
                            forumAlias = string.Format("{0}版", forumAlias.Substring(0, 1));
                            break;
                    }

                    string forumTodayQuantity = string.Empty;
                    if (h2.ChildNodes.Count == 2)
                    {
                        var em = h2.ChildNodes[1];
                        forumTodayQuantity = em.InnerText;
                    }

                    string forumInfo = p.InnerText;

                    forumGroup.Forums.Add(new Forum(forumId, forumName, forumAlias, forumTodayQuantity, forumInfo));
                }

                this.ForumGroups.Add(forumGroup);
            }
        }

        public static async Task<IEnumerable<ForumGroup>> GetFullForumGroupsAsync()
        {
            await _dataSource.LoadFullForumGroupDataAsync();

            return _dataSource.FullForumGroups;
        }

        // 读取所以版区列表数据
        public async Task LoadFullForumGroupDataAsync()
        {
            if (this._fullForumGroups.Count > 0)
            {
                this.FullForumGroups.Clear();
            }

            // 读取数据
            string url = "http://www.hi-pda.com/forum/search.php";
            var cts = new CancellationTokenSource();
            string htmlContent = await httpClient.GetAsync(url, cts);

            // 实例化 HtmlAgilityPack.HtmlDocument 对象
            HtmlDocument doc = new HtmlDocument();

            // 载入HTML
            doc.LoadHtml(htmlContent);
            var data = doc.DocumentNode;

            var content = data.Descendants().FirstOrDefault(n => n.GetAttributeValue("id", "").Equals("srchfid"));
            if (content == null)
            {
                return;
            }

            var groups = content.Descendants().Where(n => n.Name.Equals("optgroup"));
            if (groups == null)
            {
                return;
            }

            foreach (var group in groups)
            {
                string forumGroupName = group.Attributes[0].Value.Replace("--", string.Empty);
                ForumGroup forumGroup = new ForumGroup(forumGroupName);

                var groupItems = group.Descendants().Where(n => n.Name.Equals("option"));
                if (groupItems == null)
                {
                    return;
                }

                foreach (var item in groupItems)
                {
                    string forumId = item.Attributes[0].Value;
                    string forumName = item.NextSibling.InnerText;
                    forumName = forumName.Replace("&nbsp;", string.Empty);
                    forumName = forumName.Trim();
                    string forumAlias = forumName;
                    switch (forumId)
                    {
                        case "6":
                            forumAlias = "BS版";
                            break;
                        case "7":
                            forumAlias = "G版";
                            break;
                        case "14":
                            forumAlias = "Win版";
                            break;
                        case "2":
                            forumAlias = "地板";
                            break;
                        case "59":
                            forumAlias = "E版";
                            break;
                        default:
                            forumAlias = string.Format("{0}版", forumAlias.Substring(0, 1));
                            break;
                    }

                    forumGroup.Forums.Add(new Forum(forumId, forumName, forumAlias, string.Empty, string.Empty));
                }

                this.FullForumGroups.Add(forumGroup);
            }
        }
        #endregion

        #region 读取指定版块下的所有贴子
        public static ThreadData GetThread(string forumId, string forumName)
        {
            var count = _dataSource.ThreadList.Count(t => t.ForumId.Equals(forumId));
            if (count == 0)
            {
                _dataSource.ThreadList.Add(new ThreadData(forumId, forumName));
            }

            return _dataSource.ThreadList.Single(t => t.ForumId.Equals(forumId));
        }

        public static async Task RefreshThreadList(string forumId)
        {
            var threadData = _dataSource.ThreadList.FirstOrDefault(t => t.ForumId.Equals(forumId));
            if (threadData != null)
            {
                threadData.Threads.Clear();
                await _dataSource.LoadThreadsDataAsync(forumId, 1);
            }
        }

        /// <summary>
        /// 版内搜索
        /// </summary>
        /// <param name="keywords">关键字</param>
        /// <param name="searchType">搜索类型，如按标题、按作者进行搜索</param>
        /// <param name="forumId">版块ID</param>
        /// <returns>是否有搜索到数据</returns>
        public static async Task<bool> SearchThreadList(string keywords, int searchType, string forumId)
        {
            var threadData = _dataSource.ThreadList.FirstOrDefault(t => t.ForumId.Equals(forumId));
            if (threadData != null)
            {
                threadData.Threads.Clear();
                await _dataSource.SearchThreadsDataAsync(keywords, searchType, forumId, 1);

                return _dataSource.ThreadList.Single(f => f.ForumId.Equals(forumId)).Threads.Count > 0;
            }

            return false;
        }

        public static async Task<int> GetLoadThreadsCountAsync(string forumId, int pageNo, Action showProgressBar, Action hideProgressBar)
        {
            showProgressBar();
            await _dataSource.LoadThreadsDataAsync(forumId, pageNo);
            hideProgressBar();

            return _dataSource.ThreadList.Single(f => f.ForumId.Equals(forumId)).Threads.Count;
        }

        public static async Task<int> SearchLoadThreadsCountAsync(string keywords, int searchType, string forumId, int pageNo, Action showProgressBar, Action hideProgressBar)
        {
            showProgressBar();
            await _dataSource.SearchThreadsDataAsync(keywords, searchType, forumId, pageNo);
            hideProgressBar();

            return _dataSource.ThreadList.Single(f => f.ForumId.Equals(forumId)).Threads.Count;
        }

        /// <summary>
        /// 用于增量加载来控制要显示哪几项
        /// </summary>
        /// <param name="forumId"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Thread GetThreadByIndex(string forumId, int index)
        {
            return _dataSource.ThreadList.Single(f => f.ForumId.Equals(forumId)).Threads[index];
        }

        private async Task LoadThreadsDataAsync(string forumId, int pageNo)
        {
            // 载入过的页面不再载入
            var forum = _dataSource.ThreadList.FirstOrDefault(t => t.ForumId.Equals(forumId));
            if (forum == null)
            {
                return;
            }

            int threadCount = forum.Threads.Count(t => t.PageNo == pageNo);
            if (threadCount > 1)
            {
                return;
            }

            // 读取数据
            string url = string.Format("http://www.hi-pda.com/forum/forumdisplay.php?fid={0}&orderby={1}&page={2}&_={3}", forumId, ThreadListPageOrderBy, pageNo, DateTime.Now.Ticks.ToString("x"));
            var cts = new CancellationTokenSource();
            string htmlContent = await httpClient.GetAsync(url, cts);

            // 实例化 HtmlAgilityPack.HtmlDocument 对象
            HtmlDocument doc = new HtmlDocument();

            // 载入HTML
            doc.LoadHtml(htmlContent);

            var dataTable = doc.DocumentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("class", "").Equals("datatable"));
            if (dataTable == null)
            {
                return;
            }

            // 如果置顶贴数过多，只取非置顶贴的话，第一页数据项过少，会导致不会自动触发加载下一页数据
            var tbodies = dataTable.Descendants().Where(n => n.GetAttributeValue("id", "").StartsWith("normalthread_"));
            if (tbodies == null)
            {
                return;
            }

            int i = 0;
            foreach (var item in tbodies)
            {
                var tr = item.ChildNodes[1];
                var th = tr.ChildNodes[5];
                var span = th.Descendants().Single(n => n.GetAttributeValue("id", "").StartsWith("thread_"));
                var a = span.ChildNodes[0];
                var tdAuthor = tr.ChildNodes[7];
                var tdNums = tr.ChildNodes[9];
                var tdLastPost = tr.ChildNodes[11];

                string id = span.Attributes[0].Value.Substring("thread_".Length);
                string title = a.InnerText;

                int attachType = -1;
                var attachIconNode = th.Descendants().FirstOrDefault(n => n.GetAttributeValue("class", "").Equals("attach"));
                if (attachIconNode != null)
                {
                    string attachString = attachIconNode.Attributes[1].Value;
                    if (attachString.Equals("图片附件"))
                    {
                        attachType = 1;
                    }

                    if (attachString.Equals("附件"))
                    {
                        attachType = 2;
                    }
                }

                var authorName = string.Empty;
                var authorId = string.Empty;
                var authorNameNode = tdAuthor.ChildNodes[1]; // cite 此节点有出“匿名”的可能
                var authorNameLink = authorNameNode.Descendants().FirstOrDefault(n => n.Name.Equals("a"));
                if (authorNameLink == null)
                {
                    authorName = authorNameNode.InnerText.Trim();
                }
                else
                {
                    authorName = authorNameLink.InnerText;
                    authorId = authorNameLink.Attributes[0].Value.Substring("space.php?uid=".Length);
                    if (authorId.Contains("&"))
                    {
                        authorId = authorId.Split('&')[0];
                    }
                }

                var authorCreateTime = tdAuthor.ChildNodes[3].InnerText;

                string[] nums = tdNums.InnerText.Split('/');
                var replyNum = nums[0].Trim();
                var viewNum = nums[1].Trim();

                string lastPostAuthorName = "匿名";
                string lastPostTime = string.Empty;
                string[] lastPostInfo = tdLastPost.InnerText.Trim().Replace("\n", "@").Split('@');
                if (lastPostInfo.Length == 2)
                {
                    lastPostAuthorName = lastPostInfo[0];
                    lastPostTime = lastPostInfo[1]
                        .Replace(string.Format("{0}-{1}-{2} ", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), string.Empty)
                        .Replace(string.Format("{0}-", DateTime.Now.Year), string.Empty);
                }

                Thread thread = new Thread(i, pageNo, forumId, id, title, attachType, replyNum, viewNum, authorName, authorId, authorCreateTime, lastPostAuthorName, lastPostTime);
                forum.Threads.Add(thread);

                i++;
            }
        }

        private async Task SearchThreadsDataAsync(string keywords, int searchType, string forumId, int pageNo)
        {
            // 载入过的页面不再载入
            var forum = _dataSource.ThreadList.FirstOrDefault(t => t.ForumId.Equals(forumId));
            if (forum == null)
            {
                return;
            }

            // 如果页面已存在，并且已载满数据，则不重新从网站拉取数据，以便节省流量， 
            // 但最后一页（如果数据少于一页，那么第一页就是最后一页）由于随时可能会有新回复，所以对于最后一页的处理方式是先清除再重新加载
            int threadCount = forum.Threads.Count(t => t.PageNo == pageNo);
            if (threadCount > 0)
            {
                if (threadCount >= SearchPageSize) // 满页的不再加载，以便节省流量
                {
                    return;
                }

                // 再判断未满页的
                // 第一页或最后一页的回复数量不足一页，表示此页随时可能有新回复，故删除
                var lastPageData = forum.Threads.Where(t => t.PageNo == pageNo).ToList();
                foreach (var item in lastPageData)
                {
                    forum.Threads.Remove(forum.Threads.Single(t => t.Id == item.Id));
                }
            }

            // 读取数据
            string url = "http://www.hi-pda.com/forum/search.php?srchtype=title&srchtxt={0}&searchsubmit=%CB%D1%CB%F7&st=on&srchuname=&srchfilter=all&srchfrom=0&before=&orderby={2}&ascdesc=desc&srchfid%5B0%5D={1}&page={3}&_={4}";
            if (searchType == 2) // 按作者
            {
                url = "http://www.hi-pda.com/forum/search.php?srchtype=title&srchtxt=&searchsubmit=%CB%D1%CB%F7&st=on&srchuname={0}&srchfilter=all&srchfrom=0&before=&orderby={2}&ascdesc=desc&srchfid%5B0%5D={1}&page={3}&_={4}";
            }
            url = string.Format(url, httpClient.GetEncoding(keywords), "all", ThreadListPageOrderBy, pageNo, DateTime.Now.Ticks.ToString("x"));
            var cts = new CancellationTokenSource();
            string htmlContent = await httpClient.GetAsync(url, cts);

            // 实例化 HtmlAgilityPack.HtmlDocument 对象
            HtmlDocument doc = new HtmlDocument();

            // 载入HTML
            doc.LoadHtml(htmlContent);

            #region 先判断页码是否已超过最大页码，以免造成重复加载
            if (pageNo > 1)
            {
                var forumControlNode = doc.DocumentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("class", "").Equals("pages_btns s_clear"));
                // 找不到分页条，说明只有一页
                if (forumControlNode == null)
                {
                    return;
                }
            }
            #endregion

            var dataTable = doc.DocumentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("class", "").Equals("datatable"));
            if (dataTable == null)
            {
                return;
            }

            // 如果置顶贴数过多，只取非置顶贴的话，第一页数据项过少，会导致不会自动触发加载下一页数据
            var tbodies = dataTable.ChildNodes;
            if (tbodies == null)
            {
                return;
            }

            int i = 0;
            foreach (var item in tbodies)
            {
                if (item.Name != "tbody")
                {
                    continue;
                }

                if (item.InnerText.Equals("对不起，没有找到匹配结果。"))
                {
                    return;
                }

                var tr = item.ChildNodes[1];
                var th = tr.ChildNodes[5];
                var a = th.ChildNodes[3];
                var tdAuthor = tr.ChildNodes[9];
                var tdNums = tr.ChildNodes[11];
                var tdLastPost = tr.ChildNodes[13];

                string id = a.Attributes[0].Value.Substring("viewthread.php?tid=".Length);
                if (id.Contains("&"))
                {
                    id = id.Split('&')[0];
                }

                string title = a.InnerHtml;

                // 替换搜索关键字，用于高亮关键字
                MatchCollection matchsForSearchKeywords = new Regex(@"<em style=""color:red;"">([^>#]*)</em>").Matches(title);
                if (matchsForSearchKeywords != null && matchsForSearchKeywords.Count > 0)
                {
                    for (int j = 0; j < matchsForSearchKeywords.Count; j++)
                    {
                        var m = matchsForSearchKeywords[j];

                        string placeHolder = m.Groups[0].Value; // 要被替换的元素
                        string k = m.Groups[1].Value;

                        string linkXaml = string.Format(@"  “{0}”  ", k);
                        title = title.Replace(placeHolder, linkXaml);
                    }
                }

                var authorName = string.Empty;
                var authorId = string.Empty;
                var authorNameNode = tdAuthor.ChildNodes[1]; // cite 此节点有出“匿名”的可能
                var authorNameLink = authorNameNode.Descendants().FirstOrDefault(n => n.Name.Equals("a"));
                if (authorNameLink == null)
                {
                    authorName = authorNameNode.InnerText.Trim();
                }
                else
                {
                    authorName = authorNameLink.InnerText;
                    authorId = authorNameLink.Attributes[0].Value.Substring("space.php?uid=".Length);
                    if (authorId.Contains("&"))
                    {
                        authorId = authorId.Split('&')[0];
                    }
                }

                var authorCreateTime = tdAuthor.ChildNodes[3].InnerText;

                string[] nums = tdNums.InnerText.Split('/');
                var replyNum = nums[0].Trim();
                var viewNum = nums[1].Trim();

                string lastPostAuthorName = "匿名";
                string lastPostTime = string.Empty;
                string[] lastPostInfo = tdLastPost.InnerText.Trim().Replace("\n", "@").Split('@');
                if (lastPostInfo.Length == 2)
                {
                    lastPostAuthorName = lastPostInfo[0];
                    lastPostTime = lastPostInfo[1]
                        .Replace(string.Format("{0}-{1}-{2} ", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), string.Empty)
                        .Replace(string.Format("{0}-", DateTime.Now.Year), string.Empty);
                }
                
                Thread thread = new Thread(i, pageNo, forumId, id, title, -1, replyNum, viewNum, authorName, authorId, authorCreateTime, lastPostAuthorName, lastPostTime);
                forum.Threads.Add(thread);

                i++;
            }
        }
        #endregion

        #region 读取指定贴子下所有回复
        public static Reply ReconvertReplyXamlContent(int replyFloor, string threadId)
        {
            var reply = _dataSource
                .ReplyList.Single(r => r.ThreadId.Equals(threadId))
                .Replies.Single(r => r.Floor == replyFloor);

            int imageCount = 0;
            reply.XamlContent = HtmlHelper.HtmlToXaml(reply.HtmlContent, DataSource.MaxImageCount, ref imageCount);
            reply.ImageCount = imageCount;

            return reply;
        }

        public static ReplyData GetReply(string threadId, string threadName)
        {
            var count = _dataSource.ReplyList.Count(t => t.ThreadId.Equals(threadId));
            if (count == 0)
            {
                _dataSource.ReplyList.Add(new ReplyData(threadId, threadName));
            }

            return _dataSource.ReplyList.Single(t => t.ThreadId.Equals(threadId));
        }

        public static async Task RefreshReplyList(string threadId)
        {
            var replyData = _dataSource.ReplyList.FirstOrDefault(r => r.ThreadId.Equals(threadId));
            if (replyData != null)
            {
                replyData.Replies.Clear();
                await _dataSource.LoadRepliesDataAsync(threadId, 1);
            }
        }

        public static async Task<int> GetLoadRepliesCountAsync(string threadId, int pageNo, Action showProgressBar, Action hideProgressBar)
        {
            showProgressBar();
            await _dataSource.LoadRepliesDataAsync(threadId, pageNo);
            hideProgressBar();

            return _dataSource.ReplyList.First(t => t.ThreadId == threadId).Replies.Count;
        }

        /// <summary>
        /// 用于增量加载来控制要显示哪几项
        /// </summary>
        /// <param name="forumId"></param>
        /// <param name="threadId"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Reply GetReplyByIndex(string threadId, int index)
        {
            return _dataSource.ReplyList.Single(r => r.ThreadId.Equals(threadId)).Replies[index];
        }

        // 读取指定贴子的回复列表数据
        private async Task LoadRepliesDataAsync(string threadId, int pageNo)
        {
            // 移除过旧的数量，以释放内存空间
            //if (_dataSource.ReplyList.Count > 7)
            //{
            //    _dataSource.ReplyList[0] = null;
            //    _dataSource.ReplyList.RemoveAt(0);
            //}

            #region 如果数据已存在，则不读取，以便节省流量
            var thread = _dataSource.ReplyList.FirstOrDefault(t => t.ThreadId.Equals(threadId));
            if (thread == null)
            {
                return;
            }

            // 载入过的页面不再载入
            //// 现改为，只要当前页数据存在就不重新加载，让用户自己点击最底下的刷新按钮来刷新
            //// 此举可提高响应速度
            //int count = thread.Replies.Count(o => o.PageNo == pageNo);
            //if (count > 0)
            //{
            //    return;
            //}

            // 如果页面已存在，并且已载满数据，则不重新从网站拉取数据，以便节省流量， 
            // 但最后一页（如果数据少于一页，那么第一页就是最后一页）由于随时可能会有新回复，所以对于最后一页的处理方式是先清除再重新加载
            int count = thread.Replies.Count(o => o.PageNo == pageNo);
            if (count > 0)
            {
                if (count >= ReplyPageSize) // 满页的不再加载，以便节省流量
                {
                    return;
                }

                // 再判断未满页的
                // 第一页或最后一页的回复数量不足一页，表示此页随时可能有新回复，故删除
                var lastPageData = thread.Replies.Where(r => r.PageNo == pageNo).ToList();
                foreach (var item in lastPageData)
                {
                    thread.Replies.Remove(thread.Replies.Single(r => r.Floor == item.Floor));
                }
            }
            #endregion

            // 读取数据
            string url = string.Format("http://www.hi-pda.com/forum/viewthread.php?tid={0}&page={1}&ordertype={2}&_={3}", threadId, pageNo, RelayListPageOrderType, DateTime.Now.Ticks.ToString("x"));
            var cts = new CancellationTokenSource();
            string htmlStr = await httpClient.GetAsync(url, cts);

            // 实例化 HtmlAgilityPack.HtmlDocument 对象
            HtmlDocument doc = new HtmlDocument();

            // 载入HTML
            doc.LoadHtml(htmlStr);

            #region 先判断页码是否已超过最大页码，以免造成重复加载
            if (pageNo > 1)
            {
                var forumControlNode = doc.DocumentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("class", "").Equals("forumcontrol s_clear"));
                var pagesNode = forumControlNode.ChildNodes[1] // table
                    .ChildNodes[1] // tr
                    .ChildNodes[3] // td
                    .Descendants().FirstOrDefault(n => n.GetAttributeValue("class", "").Equals("pages"));
                if (pagesNode == null) // 没有超过两页
                {
                    return;
                }
                else
                {
                    var actualCurrentPageNode = pagesNode.Descendants().FirstOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == "strong");
                    if (actualCurrentPageNode != null)
                    {
                        int currentPage = Convert.ToInt32(actualCurrentPageNode.InnerText);
                        if (pageNo > currentPage)
                        {
                            return;
                        }
                    }
                }
            }
            #endregion

            var data = doc.DocumentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("id", "").Equals("postlist")).ChildNodes;
            if (data == null)
            {
                return;
            }

            EnumLayoutMode layoutMode = (EnumLayoutMode)LayoutModeSettings.LayoutModeSetting;

            int i = thread.Replies.Count;
            foreach (var item in data)
            {
                var mainTable = item.Descendants().FirstOrDefault(n => n.Name.Equals("table") && n.GetAttributeValue("summary", "").StartsWith("pid"));
                var postAuthorNode = mainTable // table
                        .ChildNodes[1] // tr
                        .ChildNodes[1]; // td.postauthor

                var postContentNode = mainTable // table
                        .ChildNodes[1] // tr
                        .ChildNodes[3]; // td.postcontent

                string authorId = string.Empty;
                string ownerName = string.Empty;
                var authorNode = postAuthorNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("class", "").Equals("postinfo"));
                if (authorNode != null)
                {
                    authorNode = authorNode.ChildNodes[1]; // a
                    authorId = authorNode.Attributes[1].Value.Substring("space.php?uid=".Length);
                    if (authorId.Contains("&"))
                    {
                        authorId = authorId.Split('&')[0];
                    }
                    ownerName = authorNode.InnerText;
                }

                var floorPostInfoNode = postContentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("class", "").StartsWith("postinfo")); // div
                var floorLinkNode = floorPostInfoNode.ChildNodes[1].ChildNodes[0]; // a
                string postId = floorLinkNode.Attributes["id"].Value.Replace("postnum", string.Empty);
                var floorNumNode = floorLinkNode.ChildNodes[0]; // em
                int floor = Convert.ToInt32(floorNumNode.InnerText);
                string threadTitle = string.Empty;
                if (floor == 1)
                {
                    var threadTitleNode = postContentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("id", "").Equals("threadtitle"));
                    if (threadTitleNode != null)
                    {
                        threadTitle = threadTitleNode.InnerText.Trim();
                    }
                }

                string postTime = string.Empty;
                var postTimeNode = postContentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("id", "").StartsWith("authorposton")); // em
                if (postTimeNode != null)
                {
                    postTime = postTimeNode.InnerText
                        .Replace("发表于 ", string.Empty)
                        .Replace(string.Format("{0}-{1}-{2} ", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), string.Empty)
                        .Replace(string.Format("{0}-", DateTime.Now.Year), string.Empty);
                }

                string textContent = string.Empty;
                string htmlContent = string.Empty;
                string xamlContent = string.Empty;
                int imageCount = 0;
                var contentNode = postContentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("class", "").Equals("t_msgfontfix"));
                if (contentNode != null)
                {
                    // 用于回复引用
                    textContent = contentNode.InnerText.Trim();
                    textContent = new Regex("\r\n").Replace(textContent, "↵");
                    textContent = new Regex("\r").Replace(textContent, "↵");
                    textContent = new Regex("\n").Replace(textContent, "↵");
                    textContent = new Regex(@"↵{1,}").Replace(textContent, "\r\n");
                    textContent = textContent.Replace("&nbsp;", "  ");

                    // 用于显示原始内容
                    htmlContent = contentNode.InnerHtml.Trim();

                    // 只在非纯文本模式下才转换，以提升在纯文本模式下的性能
                    if (layoutMode != EnumLayoutMode.Plain)
                    {
                        xamlContent = HtmlHelper.HtmlToXaml(htmlContent, MaxImageCount, ref imageCount);
                    }
                }
                else
                {
                    xamlContent = @"<RichTextBlock xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""><Paragraph>{0}</Paragraph></RichTextBlock>";
                    xamlContent = string.Format(xamlContent, @"作者被禁止或删除&#160;内容自动屏蔽");
                }

                Reply reply = new Reply(i, floor, postId, pageNo, threadId, threadTitle, authorId, ownerName, textContent, htmlContent, xamlContent, imageCount, postTime);
                thread.Replies.Add(reply);

                i++;
            }
        }
        #endregion

        #region 获取 formhash 用于提交数据，用于切换账号时调用
        public static async Task GetHashAndUserId()
        {
            await _dataSource.LoadHashAndUserId();
        }

        private async Task LoadHashAndUserId()
        {
            string url = "http://www.hi-pda.com/forum/post.php?action=newthread&fid=2&_=" + DateTime.Now.Ticks.ToString("x");
            var cts = new CancellationTokenSource();
            string htmlContent = await httpClient.GetAsync(url, cts);

            // 实例化 HtmlAgilityPack.HtmlDocument 对象
            HtmlDocument doc = new HtmlDocument();

            // 载入HTML
            doc.LoadHtml(htmlContent);

            // 读取发布文字信息所需要的 hash 值
            var postNode = doc.DocumentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("class", "").Equals("content editorcontent"));
            if (postNode != null)
            {
                var formHashInputNode = postNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("name", "").Equals("formhash"));
                if (formHashInputNode != null)
                {
                    this._formHash = formHashInputNode.Attributes[3].Value.ToString();
                }
            }

            // 读取 上载图片所需的 uid 和 hash 值
            var imgAttachNode = doc.DocumentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("id", "").Equals("imgattachbtnhidden"));
            if (imgAttachNode != null)
            {
                var userIdNode = imgAttachNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("name", "").Equals("uid"));
                if (userIdNode != null)
                {
                    this._userId = userIdNode.Attributes[2].Value;
                }

                var hashNode = imgAttachNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("name", "").Equals("hash"));
                if (hashNode != null)
                {
                    this._hash = hashNode.Attributes[2].Value;
                }
            }
        }
        #endregion

        #region 读取指定贴子标题、内容及回复数据，用于修改贴子或回复
        public static async Task GetContentForEdit(string threadId, string postId)
        {
            await _dataSource.LoadContentForEdit(threadId, postId);
        }

        private async Task LoadContentForEdit(string threadId, string postId)
        {
            string url = string.Format("http://www.hi-pda.com/forum/post.php?action=edit&tid={0}&pid={1}&page=1&_=" + DateTime.Now.Ticks.ToString("x"), threadId, postId);
            var cts = new CancellationTokenSource();
            string htmlContent = await httpClient.GetAsync(url, cts);

            // 实例化 HtmlAgilityPack.HtmlDocument 对象
            HtmlDocument doc = new HtmlDocument();

            // 载入HTML
            doc.LoadHtml(htmlContent);

            // 标题
            string subject = string.Empty;
            var subjectInput = doc.DocumentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("prompt", "").Equals("post_subject"));
            if (subjectInput != null)
            {
                subject = subjectInput.Attributes["value"].Value;
            }

            // 内容
            string content = string.Empty;
            var contentTextArea = doc.DocumentNode.Descendants().FirstOrDefault(n => n.GetAttributeValue("prompt", "").Equals("post_message"));
            if (contentTextArea != null)
            {
                content = contentTextArea.InnerText.Replace(MessageTail, string.Empty).TrimEnd();
            }

            this._contentForEdit = new ContentForEdit(subject, content, postId, threadId);
        }
        #endregion
    }
}
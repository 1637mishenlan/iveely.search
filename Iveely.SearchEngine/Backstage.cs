﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Iveely.CloudComputing.Client;
using Iveely.Framework.Algorithm;
using Iveely.Framework.Algorithm.AI;
using Iveely.Framework.Algorithm.AI.Library;
using Iveely.Framework.Network;
using Iveely.Framework.Network.Synchronous;
using Iveely.Framework.Text;
using Iveely.Framework.Text.Segment;
using System.Threading;


namespace Iveely.SearchEngine
{
    /// <summary>
    /// 搜索引擎
    /// </summary>
    public class Backstage : Application
    {
        /// <summary>
        /// 网页信息
        /// </summary>
        public class Page
        {
            /// <summary>
            /// 采集日期
            /// </summary>
            public string Date { get; set; }

            /// <summary>
            /// 页面链接
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// 页面标题
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// 页面正文
            /// </summary>
            public string Content { get; set; }


            public override string ToString()
            {
                return string.Format("{0}\t{1}\t{2}\t{3}", Date, Url, Title, Content.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty));
            }
        }

        /// <summary>
        /// 爬虫爬行队列
        /// </summary>
        public List<string> Urls = new List<string>();

        /// <summary>
        /// 索引片段向量
        /// </summary>
        public static InvertFragment Fragment;

        /// <summary>
        /// 爬虫爬行的跟站点
        /// </summary>
        private string Host = "www.cnblogs.com";

        /// <summary>
        /// 索引文件
        /// </summary>
        private string indexFile;

        /// <summary>
        /// 搜索端口
        /// </summary>
        private int searchPort = 9000;

        /// <summary>
        /// 主程序入口
        /// </summary>
        /// <param name="args"></param>
        public override void Run(object[] args)
        {
            //1. 初始化
            Init(args);
            Fragment = new InvertFragment(GetRootFolder());
            Urls.Add("http://www.cnblogs.com");
            indexFile = GetRootFolder() + "\\InvertFragment.global";

            Thread searchThread = new Thread(StartSearcher);
            searchThread.Start();

            //2. 循环数据采集
            while (Urls.Count > 0)
            {
                List<Page> pages = new List<Page>();

                //2.1 爬虫开始运行
                Crawler(ref pages);

                //2.2 索引器开始运行
                if (pages != null && pages.Count > 0)
                {
                    Indexer(ref pages);
                }
            }
        }

        /// <summary>
        /// 爬虫主程序
        /// </summary>
        /// <param name="pages">搜集的网页信息</param>
        public void Crawler(ref List<Page> pages)
        {
            //1. 遍历url集合
            HashSet<string> newUrls = new HashSet<string>();
            using (var database = Database.Open(GetRootFolder() + "\\Iveely.Search.Data.part"))
            {
                for (int i = 0; i < Urls.Count; i++)
                {
                    try
                    {
                        //1.1 获取标题，网页正文，子链接集
                        //WriteToConsole("Processing " + Urls[i]);
                        string title = string.Empty;
                        string content = string.Empty;
                        List<string> childrenLink = null;
                        GetHtml(Urls[i], ref title, ref content, ref childrenLink);

                        //1.2 过滤子链接集
                        foreach (string link in childrenLink)
                        {
                            if (!newUrls.Contains(link) && (new Uri(link)).Host == Host)
                            {
                                newUrls.Add(link);
                            }
                        }

                        //1.3 记录数据
                        if (title != string.Empty)
                        {
                            Page page = new Page();
                            page.Url = Urls[i];
                            page.Title = title;
                            page.Date = DateTime.Now.ToShortDateString();
                            page.Content = content;
                            pages.Add(page);
                            database.Store(page);
                        }
                    }
                    catch (Exception exception)
                    {
                        WriteToConsole(exception.ToString());
                    }
                }
            }

            //3. 新的url标识为未爬行过，并存放于缓存中
            SetListIntoCache(newUrls.ToArray(), false);

            //4. 重新获取新的url
            Urls.Clear();
            Urls.AddRange(GetKeysByValueFromCache(false, 10, true));
        }

        /// <summary>
        /// 索引程序入口
        /// </summary>
        /// <param name="pages">网页信息集合</param>
        public void Indexer(ref List<Page> pages)
        {
            //自动分析网页表达的含义
            WriteToConsole(string.Format("开始自动分析网页表达的含义，共{0}条记录。", pages.Count));
            List<Template.Question> questions = new List<Template.Question>();
            const string delimiter = ".?。！\t？…●|\r\n])!";
            foreach (Page page in pages)
            {
                //List<Template.Question> titleResult = Bot.GetInstance(GetRootFolder()).BuildQuestion(page.Title, page.Url);
                //if (titleResult != null && titleResult.Count > 0)
                //{
                //    questions.AddRange(titleResult);
                //}
                string[] sentences = page.Content.Split(delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string sentence in sentences)
                {
                    if (sentence.Length >= 5)
                    {
                        List<Template.Question> result = Bot.GetInstance(GetRootFolder()).BuildQuestion(sentence, page.Url, page.Title);
                        if (result != null && result.Count > 0)
                        {
                            questions.AddRange(result);
                        }
                    }
                }
            }
            pages.Clear();

            //对表达的语义建议索引
            WriteToConsole(string.Format("对表达的语义建议索引，共{0}条记录。", questions.Count));

            if (File.Exists(indexFile))
            {
                Fragment = Serializer.DeserializeFromFile<InvertFragment>(indexFile);
            }

            using (var database = Database.Open(GetRootFolder() + "\\Iveely.Search.Question"))
            {
                if (questions.Any())
                {
                    for (int i = 0; i < questions.Count; i++)
                    {
                        int id = questions[i].Answer.GetHashCode();
                        Fragment.AddDocument(id, NGram.GetGram(questions[i].Description));
                        questions[i].Id = id;
                        database.Store(questions[i]);
                    }
                }
            }
            questions.Clear();

            Serializer.SerializeToFile(Fragment, indexFile);
        }

        public void StartSearcher()
        {
            if (File.Exists(indexFile))
            {
                Fragment = Serializer.DeserializeFromFile<InvertFragment>(indexFile);
            }
            int servicePort = int.Parse(GetRootFolder()) % 100;

            try
            {
                searchPort += servicePort;
                Server server = new Server(Dns.GetHostName(), searchPort, processQuery);
                WriteToConsole(searchPort + " is start search service.");
                server.Listen();
            }
            catch (Exception exception)
            {
                WriteToConsole("Start Server Error." + exception);
            }
        }


        public byte[] processQuery(byte[] bytes)
        {
            string currentQueryIndex = GetGlobalCache<string>("CurrentQueryIndex");
            if (!string.IsNullOrEmpty(currentQueryIndex))
            {
                string query = GetGlobalCache<string>(currentQueryIndex);
                WriteToConsole("Get Query:" + query);
                string[] keywords = NGram.GetGram(query, NGram.Type.BiGram);
                List<string> docs = Fragment.FindCommonDocumentByKeys(keywords);
                List<Template.Question> result = new List<Template.Question>();
                using (var database = Database.Open(GetRootFolder() + "\\Iveely.Search.Question"))
                {
                    var wordsQuey = database.Query<Template.Question>();
                    int returnCount = 5;
                    foreach (string doc in docs)
                    {
                        if (returnCount == 0)
                            break;
                        returnCount--;
                        wordsQuey.Descend("Id").Constrain(int.Parse(doc)).Equal();
                        var questions = wordsQuey.Execute<Template.Question>();
                        if (questions != null && questions.Count > 0)
                        {
                            result.Add(questions.GetFirst());
                        }
                    }
                }
                string inputResultKey = Dns.GetHostName() + "," + searchPort + query;
                WriteToConsole("Result write into cache key=" + inputResultKey + ", count=" + result.Count);
                //if (result.Count > 10)
                //{
                //    result.RemoveRange(10, result.Count - 10);
                //}
                string content = string.Join("\r\n", result);
                foreach (string keyword in keywords)
                {
                    content = content.Replace(keyword, "<strong>" + keyword + "</strong>");
                }
                SetGlobalCache(inputResultKey, content);
            }
            return Serializer.SerializeToBytes(true);
        }
    }
}

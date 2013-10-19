﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Iveely.CloudComputting.Client;

namespace Iveely.SearchEngine
{
    /// <summary>
    /// 搜索引擎
    /// </summary>
    public class Program : Application
    {
        /// <summary>
        /// 爬虫爬行队列
        /// </summary>
        public List<string> Urls = new List<string>();

        public override void Run(object[] args)
        {
            //1. 初始化
            Init(args);
            Urls.Add("http://www.baidu.com");

            //2. 搜索服务开始运行
            Thread searchThread = new Thread(Searcher);
            searchThread.Start();

            //3. 循环数据采集
            while (Urls.Count > 0)
            {
                //3.1 爬虫开始运行
                Crawler();

                //3.2 索引器开始运行
                Indexer();
            }

        }

        public void Crawler()
        {
            //1. 遍历url集合
            StringBuilder data = new StringBuilder();
            HashSet<string> newUrls = new HashSet<string>();
            for (int i = 0; i < Urls.Count; i++)
            {
                try
                {
                    //1.1 获取标题，网页正文，子链接集
                    WriteToConsole("Processing " + Urls[i]);
                    string title = string.Empty;
                    string content = string.Empty;
                    List<string> childrenLink = null;
                    GetHtml(Urls[i], ref title, ref content, ref childrenLink);

                    //1.2 过滤子链接集
                    foreach (string link in childrenLink)
                    {
                        if (!newUrls.Contains(link))
                        {
                            newUrls.Add(link);
                        }
                    }

                    //1.3 记录数据
                    if (title != string.Empty)
                    {
                        data.Append(Urls[i]);
                        data.Append("\t");
                        data.Append(title);
                        data.Append("\t");
                        data.Append(content + "\n");
                    }

                }
                catch (Exception exception)
                {
                    WriteToConsole(exception.ToString());
                }
            }

            //2. 存储数据
            DateTime dateTime = DateTime.UtcNow;
            string fileName = string.Format("{0}{1}{2}{3}", dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour);
            WriteText(data.ToString(), fileName, false);

            //3. 新的url标识为未爬行过，并存放于缓存中
            SetListIntoCache(newUrls.ToArray(), false);

            //4. 重新获取新的url
            Urls.Clear();
            Urls.AddRange(GetKeysByValueFromCache(false, 10, true));
        }

        public void Indexer()
        {

        }

        public void Searcher()
        {

        }
    }
}

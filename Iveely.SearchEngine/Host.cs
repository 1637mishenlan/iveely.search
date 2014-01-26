﻿/*==========================================
 *创建人：刘凡平
 *邮  箱：liufanping@iveely.com
 *世界上最快乐的事情，莫过于为理想而奋斗！
 *版  本：0.1.0
 *Iveely=I void everything,except love you!
 *========================================*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Iveely.CloudComputing.Client;
using Iveely.Framework.Algorithm.AI;
using Iveely.Framework.Algorithm.AI.Library;
using Iveely.Framework.DataStructure;
using Iveely.Framework.Network;
using Iveely.Framework.Network.Synchronous;
using Iveely.Framework.Text;
using Iveely.Framework.Text.Segment;

namespace Iveely.SearchEngine
{
    public class Host : Application
    {
        private Library library = new Library();

        public static void Main(string[] args)
        {
            Framework.NLP.QuestionChecker checker = Framework.NLP.QuestionChecker.GetInstance();


            Iveely.Framework.Text.Segment.IctclasSegment semantic =
            Iveely.Framework.Text.Segment.IctclasSegment.GetInstance();
            string[] allLines = File.ReadAllLines("Init\\question.txt", Encoding.UTF8);
            StringBuilder builder = new StringBuilder();
            int fi = 0;
            int ti = 0;
            foreach (string line in allLines)
            {
                bool isQ = checker.IsQuestion(line);
                if (isQ)
                    ti++;
                else
                {
                    fi++;
                }
                builder.AppendLine(line+" "+isQ);
            }

            File.WriteAllText("question_checker.txt", builder.ToString(), Encoding.UTF8);
            Console.WriteLine(ti*1.0/(fi+ti));

            //Console.WriteLine(checker.IsQuestion("你好吗?"));
            //Console.WriteLine(checker.IsQuestion("今天是周一"));
            //Console.WriteLine(checker.IsQuestion("今天是周一吗?"));
            //Console.WriteLine(checker.IsQuestion("别哭了，好吗?"));
            //Console.WriteLine(checker.IsQuestion("别哭了，好吗"));

            //Iveely.Framework.Text.Segment.IctclasSegment semantic =
            //    Iveely.Framework.Text.Segment.IctclasSegment.GetInstance();
            //string[] allLines = File.ReadAllLines("Init\\question.txt", Encoding.UTF8);
            //StringBuilder builder = new StringBuilder();
            //foreach (string line in allLines)
            //{
            //    builder.AppendLine(semantic.SplitToSemantic(line));
            //}

            //File.WriteAllText("question_result.txt",builder.ToString(),Encoding.UTF8);
          //  Iveely.Framework.NLP.Semantic semantic = Iveely.Framework.NLP.Semantic.GetInstance();

            //时间疑问句


            //Console.WriteLine(semantic.TextSimilarity("你是班长", "你的确是班长"));

            //string[] allLines = File.ReadAllLines("语义.txt", Encoding.UTF8);
            //StringBuilder builder = new StringBuilder();
            //foreach (string line in allLines)
            //{
            //    string[] context = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            //    semantic.GetSimilarContext(context[2]);
            //    Console.WriteLine();
            //    semantic.GetSimilarContext(context[3]);
            //    builder.AppendLine(line + "    " + semantic.TextSimilarity(context[2], context[3]));
            //    Console.WriteLine();
            //}
            //File.WriteAllText("语义-分词-义原-结果.txt", builder.ToString(), Encoding.UTF8);



            Console.WriteLine("end");
            //QuestionGetter getter = new QuestionGetter();
            //getter.Run(new object[] { 8001, 8001, 8001, 8001, 8001, 8001 });

            //if (args.Length > 0)
            //{
            //    Host host = new Host();
            //    host.Run(null);
            //}
            //else
            //{
            //    Backstage backstage = new Backstage();
            //    backstage.Run(new object[] { 8001, 8001, 8001, 8001, 8001, 8001 });
            //}
            Console.ReadKey();
        }

        public override void Run(object[] args)
        {
            while (true)
            {
                Console.Write("Text Query Words:");
                string query = Console.ReadLine();
                Console.WriteLine(library.TextQuery(query));

                Console.Write("Relative Query Word:");
                query = Console.ReadLine();
                Console.WriteLine(library.RelativeQuery(query));
            }
        }
    }
}

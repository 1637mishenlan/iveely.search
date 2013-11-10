﻿/*==========================================
 *创建人：刘凡平
 *邮  箱：liufanping@iveely.com
 *世界上最快乐的事情，莫过于为理想而奋斗！
 *版  本：0.1.0
 *Iveely=I void everything,except love you!
 *========================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iveely.CloudComputing.Merger
{
    public class CombineList : Operate
    {
        private const string OperateType = "combine_list";

        private string flag;

        public CombineList(string appTimeStamp, string appName)
            : base(appTimeStamp, appName)
        {
            this.AppName = appName;
            this.AppTimeStamp = appTimeStamp;
            flag = OperateType + "_" + appTimeStamp + "_" + appName;
        }

        public override T Compute<T>(T val)
        {
            lock (Table)
            {
                if (Table[flag] == null)
                {
                    Table.Add(flag, val);
                    CountTable.Add(flag, 1);
                }
                else
                {
                    List<object> list = (List<object>)Table[flag];
                    list.AddRange((List<object>)(object)val);

                    Table[flag] = list;
                    int count = int.Parse(CountTable[flag].ToString());
                    CountTable[flag] = count + 1;
                }
            }
            if (Waite(flag))
            {
                T t = (T)Convert.ChangeType(Table[flag], typeof(T));
                return t;
            }
            throw new TimeoutException();
        }
    }
}

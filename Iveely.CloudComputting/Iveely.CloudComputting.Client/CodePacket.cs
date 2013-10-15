﻿/*==========================================
 *创建人：刘凡平
 *邮  箱：liufanping@iveely.com
 *电  话：13896622743
 *版  本：0.1.0
 *Iveely=I void everything,except love you!
 *========================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iveely.Framework.Network;

[Serializable]
public class CodePacket : Packet
{
    public string ClassName
    {
        get;
        private set;
    }

    public string AppName
    {
        get;
        private set;
    }

    public string TimeStamp
    {
        get;
        private set;
    }

    /// <summary>
    /// 信息返回IP
    /// </summary>
    public string ReturnIp
    {
        get;
        private set;
    }

    /// <summary>
    /// 信息返回接收端口
    /// </summary>
    public int Port
    {
        get;
        private set;
    }

    public CodePacket(byte[] codeBytes, string className, string appName, string timeStamp)
    {
        this.Data = codeBytes;
        this.ClassName = className;
        this.AppName = appName;
        this.TimeStamp = timeStamp;
    }

    public void SetReturnAddress(string ip, int port)
    {
        this.ReturnIp = ip;
        this.Port = port;
    }
}


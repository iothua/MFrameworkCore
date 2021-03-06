﻿using System;
using System.Threading;
using Google.Protobuf;
using Loxodon.Framework.Execution;
using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.NetWorks
{
    public class UnityControl : MonoBehaviour
    {
        private ControlTerminal controlTerminal;
        private EventPool serverEventPool;

        private ThreadScheduledExecutor scheduledExecutor;
        private Thread thread;

        private void Awake()
        {
            controlTerminal = new ControlTerminal();
            serverEventPool = new EventPool(MessageDistributionControl.Instance);
            serverEventPool.GetEvent<ExperimentRequestEvent>().AddReceiveEvent(OnExpReq);
            serverEventPool.GetEvent<ExperimentReceiptEvent>().AddReceiveEvent(OnExpRec);
            serverEventPool.GetEvent<BreakConnectEvent>().AddReceiveEvent(OnExitReq);
            controlTerminal.Start("127.0.0.1", 8888);

            thread = new Thread(OnUpdate);
            thread.IsBackground = true;
            thread.Start();
        }

//        public UnityControl()
//        {
//            controlTerminal = new ControlTerminal();
//            serverEventPool=new EventPool(MessageDistributionControl.Instance);
//            serverEventPool.GetEvent<ExperimentRequestEvent>().AddReceiveEvent(OnExpReq);
//            serverEventPool.GetEvent<ExperimentReceiptEvent>().AddReceiveEvent(OnExpRec);
//            serverEventPool.GetEvent<BreakConnectEvent>().AddReceiveEvent(OnExitReq);
//            controlTerminal.Start("127.0.0.1",8888);
            
//            thread = new Thread(OnUpdate);
//            thread.IsBackground = true;
//            thread.Start();
            
//            /*
            
//            scheduledExecutor = new ThreadScheduledExecutor();
//            scheduledExecutor.Start();

//            scheduledExecutor.ScheduleAtFixedRate(() => {
                
//                Debug.Log("Hello 你在执行吗？");
                
//                if (!isBreak)
//                {
//                    controlTerminal.Update();
//                }
//            }, 0, 10);
            
//*/
//            //Debug.Log("1");
//        }

        private void OnExitReq(int sender,IMessage proto)
        {
            var info = proto as ConnectInfo;
            controlTerminal.Broadcast(serverEventPool.GetEvent<BreakConnectEvent>().GetProtobuf(proto),sender);
        }

        bool isBreak = false;
        /// <summary>
        /// 从控制端收到打开实验请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proto"></param>
        private void OnExpReq(int sender,IMessage proto)
        {
            var info = proto as ExperimentInfo;
            Debug.Log("收到请求"+info.PrefabPath);
            
            controlTerminal.Broadcast(serverEventPool.GetEvent<ExperimentRequestEvent>().GetProtobuf(proto),sender);
        }
        /// <summary>
        /// 从客户端收到实验打开回执
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proto"></param>
        private void OnExpRec(int sender,IMessage proto)
        {
            var info = proto as ExperimentInfo;

            controlTerminal.Broadcast(serverEventPool.GetEvent<ExperimentReceiptEvent>().GetProtobuf(proto),sender);
        }

        
        
        private void OnUpdate()
        {
            while (!isBreak)
            {
                controlTerminal.Update();
                Thread.Sleep(10);
            }
        }

        public void OnDestroy()
        {
            isBreak=true;

            if (thread != null)
            {
                thread.Join();
            }

            if (scheduledExecutor != null)
                scheduledExecutor.Stop();
            
            if (controlTerminal != null)
                controlTerminal.Close();
        }
    }
}

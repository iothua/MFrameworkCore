﻿using System;
using Google.Protobuf;
using UnityEngine;

namespace MagiCloud.NetWorks
{
    public class ClientManager :ServerNetManager
    {
        public event Action<int> EventExperimentStatus;

        public ClientManager() : base()
        {
            eventPool.GetEvent<ExperimentRequestEvent>().AddReceiveEvent(OnExperimentRequest);

            eventPool.GetEvent<BreakConnectEvent>().AddReceiveEvent((sender,proto) =>
            {
                Application.Quit();
            });

            eventPool.GetEvent<SystemSettingRequestEvent>().AddReceiveEvent(OnSystemSetting);
            connection.Connect("127.0.0.1",8888);

            IsConnect = true;
        }

        /// <summary>
        /// 接收到实验请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proto">ExperimentInfo</param>
        protected virtual void OnExperimentRequest(int sender,IMessage proto)
        {

        }

        protected void OnSystemSetting(int sender,IMessage proto)
        {
            SystemSettingInfo systemSettingInfo = proto as SystemSettingInfo;
            MSystemSetting.SetSystemData(systemSettingInfo);

            MSystemSetting.OnInitialize();

            //发送设置成功
            OnSendData<SystemSettingReceiptEvent,SystemSettingReceipt>(new SystemSettingReceipt() { Status = 1 });

        }

        /// <summary>
        /// 返回
        /// </summary>
        public void OnBack()
        {
            currentExperiment.ExperimentStatus = 2;
            OnSendData<ExperimentReceiptEvent, ExperimentInfo>(currentExperiment);
        }

        public void OnExit()
        {
            currentExperiment.ExperimentStatus = 204;
            OnSendData<ExperimentReceiptEvent, ExperimentInfo>(currentExperiment);
        }

        public void OnLoadComplete()
        {
            currentExperiment.ExperimentStatus = 1;

            OnSendData<ExperimentReceiptEvent, ExperimentInfo>(currentExperiment);
        }



        public void OnExperimentError(string msg)
        {
            currentExperiment.ExperimentStatus = 255;

            currentExperiment.Name = currentExperiment.Name + ":" + msg;
            eventPool.GetEvent<ExperimentReceiptEvent>().Send(connection, currentExperiment);

        }

        public void OnExperimentReset()
        {
            currentExperiment.ExperimentStatus = 3;
            eventPool.GetEvent<ExperimentReceiptEvent>().Send(connection, currentExperiment);

            SendExperimentStatus(currentExperiment.ExperimentStatus);

            currentExperiment = null;
        }

        public override void OnDestroy()
        {
            OnExit();
            base.OnDestroy();
        }

        public void SendExperimentStatus(int status)
        {
            if (EventExperimentStatus != null)
                EventExperimentStatus(status);
        }
    }
}

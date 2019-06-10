﻿using MagiCloud.Common;
using MagiCloud.Core.Events;
using MagiCloud.RotateAndZoomTool;

namespace MagiCloud.Operate.OperateFSM
{
    /// <summary>
    /// 缩放状态
    /// </summary>
    public class Zoom :OperateStateBase
    {
        internal override void OnEnter(IFsm<OperateSystem> fSM)
        {
            base.OnEnter(fSM);
            switch (Platform)
            {
                case Core.OperatePlatform.Kinect:
                    RotateAndZoomManager.ZoomSpeed=RotateAndZoomManager.Speed_CameraZoom;
                    break;
                case Core.OperatePlatform.Mouse:
                    if (IsTwoTouch)
                    {
#if UNITY_ANDROID
                         RotateAndZoomManager.ZoomSpeed=RotateAndZoomManager.Speed_CameraZoom;
#elif UNITY_STANDALONE_WIN
                        RotateAndZoomManager.ZoomSpeed=RotateAndZoomManager.Speed_CameraZoom*5;
#endif
                    }
                    else
                        RotateAndZoomManager.ZoomSpeed=RotateAndZoomManager.Speed_CameraZoom*5;
                    break;
                default:
                    break;
            }
            MSwitchManager.CurrentMode=OperateModeType.Zoom;
        }
        internal override void OnUpdate(IFsm<OperateSystem> fSM)
        {
            base.OnUpdate(fSM);
            if (!IsTwoTouch)
            {
                if ((LeftIdle||RightIdle))
                    ChangeState(fSM,typeof(Idle));
            }
        }
        internal override void OnLeave(IFsm<OperateSystem> fSM,bool v)
        {
            base.OnLeave(fSM,v);
            EventCameraZoom.SendListener(0);
        }
    }

}

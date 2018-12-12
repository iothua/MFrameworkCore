﻿using Chemistry.Data;
using DG.Tweening;
using MagiCloud.Equipments;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chemistry.Equipments.Actions
{
    public class EA_SpoonTrajectoryContent
    {
        /// <summary>
        /// 药匙取药动画合集
        /// </summary>
        public EA_SpoonTrajectoryContent(EquipmentBase equipmentBase, I_ET_S_SpoonTake i_ET_S_SpoonTake, Action<I_ET_S_SpoonTake> onCompleteAction)
        {
            switch (i_ET_S_SpoonTake.InteractionEquipment)
            {
                case DropperInteractionType.细口瓶:
                    break;
                case DropperInteractionType.锥形瓶:
                    break;
                case DropperInteractionType.集气瓶:
                    break;
                case DropperInteractionType.烧杯:
                    break;
                case DropperInteractionType.试管:
                    break;
                case DropperInteractionType.蒸发皿:
                    break;
                case DropperInteractionType.量筒:
                    break;
                case DropperInteractionType.玻璃杯:
                    break;
                case DropperInteractionType.培养皿:
                    break;
                case DropperInteractionType.广口瓶:
                    equipmentBase.transform.DOLocalMoveY(equipmentBase.transform.localPosition.y - i_ET_S_SpoonTake.Height, 0.5f).OnComplete(() => onCompleteAction.Invoke(i_ET_S_SpoonTake));
                    break;
                default:
                    equipmentBase.transform.DOLocalMoveY(equipmentBase.transform.localPosition.y - i_ET_S_SpoonTake.Height, 0.5f).OnComplete(() => onCompleteAction.Invoke(i_ET_S_SpoonTake));
                    break;
            }
        }
        /// <summary>
        /// 药匙放药动画合集
        /// </summary>
        public EA_SpoonTrajectoryContent(EquipmentBase equipmentBase, I_ET_S_SpoonPut i_ET_S_SpoonPut, Action<I_ET_S_SpoonPut> onCompleteAction)
        {
            switch (i_ET_S_SpoonPut.InteractionEquipment)
            {
                case DropperInteractionType.细口瓶:
                    break;
                case DropperInteractionType.锥形瓶:
                    break;
                case DropperInteractionType.集气瓶:
                    break;
                case DropperInteractionType.烧杯:
                    equipmentBase.transform.DOLocalMoveY(equipmentBase.transform.localPosition.y - i_ET_S_SpoonPut.Height, 0.5f).OnComplete(() => onCompleteAction.Invoke(i_ET_S_SpoonPut));
                    break;
                case DropperInteractionType.试管:
                    break;
                case DropperInteractionType.蒸发皿:
                    break;
                case DropperInteractionType.量筒:
                    break;
                case DropperInteractionType.玻璃杯:
                    break;
                case DropperInteractionType.培养皿:
                    break;
                case DropperInteractionType.广口瓶:
                    break;
                default:
                    equipmentBase.transform.DOLocalMoveY(equipmentBase.transform.localPosition.y - i_ET_S_SpoonPut.Height, 0.5f).OnComplete(() => onCompleteAction.Invoke(i_ET_S_SpoonPut));
                    break;
            }
        }
    } 
}
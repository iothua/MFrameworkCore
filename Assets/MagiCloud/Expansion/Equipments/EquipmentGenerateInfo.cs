﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace MagiCloud.Equipments
{


    [ExecuteInEditMode]
    public class EquipmentGenerateInfo :MonoBehaviour
    {
        [Header("仪器基本信息")]
        //   [LabelText("仪器名称(EquipmentName)")]
        public string EquipmentName = "仪器名称"; //仪器名称

        [Header("命名空间(Namespaces)")]
        public string Namespaces = "MagiCloud.Equipments"; //命名空间

        [Header("脚本名称(scriptName)")]
        public string scriptName = "EquipmentBase";

        [Space(10)]
        [Header("碰撞体数据(ColliderDat)")]
        //碰撞体数据
        public ColliderData colliderData; //碰撞体数据
        [Header("坐标信息(TransformData)")]
        public TransformData transformData;


        [Header("模型数据")]
        public List<EquipmentModelData> modelDatas;
        [Header("特效数据")]
        public List<EquipmentModelData> effectDatas;

        [Header("子仪器")]
        public List<EquipmentGenerateInfo> childs;

        // [ButtonGroup]
        //[Button("创建")]
        public void OnCreate()
        {
            //添加脚本，修改名称
            gameObject.name = EquipmentName;
            transform.SetTransform(transformData);

            var equipment = transform.AddEquipmentScript<EquipmentBase>(Namespaces,scriptName);
            if (equipment != null)
            {
                equipment.FeaturesObject.SetCollider(colliderData.Center.Vector,colliderData.Size.Vector);
            }

            Transform modelNode = equipment == null ? transform : equipment.ModelNode;

            if (equipment != null)
                modelNode.DestroyImmediateChildObject();

            for (int i = 0; i < modelDatas.Count; i++)
            {
                if (modelDatas[i].resourcesItem == null) continue;
                modelDatas[i].CreateModel(modelNode);
            }

            Transform effectNode = equipment == null ? transform : equipment.EffectNode;

            if (equipment != null)
                effectNode.DestroyImmediateChildObject();

            for (int i = 0; i < effectDatas.Count; i++)
            {
                if (effectDatas[i].resourcesItem == null) continue;
                effectDatas[i].CreateModel(effectNode);
            }

            if (equipment != null)
                equipment.OnInitializeEquipment_Editor(EquipmentName);
        }

        //[ButtonGroup]
        //[Button("获取物体数据")]
        public void GetObjectData()
        {
            var equipment = gameObject.GetComponent<EquipmentBase>();
            if (equipment == null) return;

            EquipmentName = name;

            Type type = equipment.GetType();
            Namespaces = type.Namespace;
            scriptName = type.Name;

            colliderData = new ColliderData(equipment.FeaturesObject.Collider);
            transformData = new TransformData(transform);

            //获取到根目录下的





            for (int i = 0; i < modelDatas.Count; i++)
            {
                modelDatas[i].geneterItem.Assignment();
                modelDatas[i].resourcesItem.Assignment(modelDatas[i].geneterItem.modelObject.transform);
            }

            for (int i = 0; i < effectDatas.Count; i++)
            {
                effectDatas[i].geneterItem.Assignment();
                effectDatas[i].resourcesItem.Assignment(effectDatas[i].geneterItem.modelObject.transform);
            }

            childs = gameObject.GetComponentsInChildren<EquipmentGenerateInfo>().Where(arg => !arg.Equals(this)).ToList();
        }

        //[ButtonGroup]
        //[Button("设置物体数据")]
        public void SetObjectData()
        {
            var equipment = gameObject.GetComponent<EquipmentBase>();
            if (equipment == null) return;

            equipment.FeaturesObject.SetCollider(colliderData.Center.Vector,colliderData.Size.Vector);
            transform.SetTransform(transformData);

            for (int i = 0; i < modelDatas.Count; i++)
            {
                modelDatas[i].geneterItem.SetTransform();
            }

            for (int i = 0; i < effectDatas.Count; i++)
            {
                effectDatas[i].geneterItem.SetTransform();
            }
        }
    }
}



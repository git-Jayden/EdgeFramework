﻿/****************************************************
	文件：CustomScrollPage.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 15:39   	
	Features： * 自定义滚动页
               * 先暂时支持从左到右的横向滚动页
*****************************************************/
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EdgeFramework.UGUIEx
{
    public class CustomScrollPage : CustomScrollBase
    {
        public RectTransform Content;
        public int Count = 0;
        public int Index = 0;
        public bool Horizontal = true;
        public float Offset = 100f;
        public Vector2 PageSize;
        public float SmoothTime = 0.05f;
        private float mSmoothSpeed = 0;
        private bool mNeedMove = false;
        private Vector2 mBeginPostion;
        //=================================================
        /// </summary>
        /// 翻页成功后的回调函数
        /// </summary>
        public Action<int> OnPageAction { get; set; }

        /// <summary>
        /// 开始滑动
        /// </summary>
        public override void OnBeginDrag(PointerEventData eventData)
        {
            mNeedMove = false;
            mSmoothSpeed = 0;
            mBeginPostion = Content.localPosition;
        }

        /// <summary>
        /// 滑动
        /// </summary>
        public override void OnDrag(PointerEventData eventData)
        {
            var delta = eventData.position - eventData.pressPosition;
            if (Horizontal)
            {
                Content.localPosition = new Vector3(mBeginPostion.x + delta.x, Content.localPosition.y, 0);
            }
            else
            {
                // todo
            }
        }

        /// <summary>
        /// 拖动结束后判断是否翻页
        /// </summary>
        public override void OnEndDrag(PointerEventData eventData)
        {
            var delta = eventData.position - eventData.pressPosition;
            if (Horizontal)
            {
                if (delta.x > Offset && Index > 0)
                {
                    Index--;
                    if (OnPageAction != null) OnPageAction(Index);
                }
                else if (delta.x < -Offset && Index < Count - 1)
                {
                    Index++;
                    if (OnPageAction != null) OnPageAction(Index);
                }
            }
            else
            {
                // todo
            }
            mNeedMove = true;
        }

        /// <summary>
        /// 回到正确的页面上
        /// </summary>
        private void Update()
        {
            if (!mNeedMove) return;
            var targetPosX = -PageSize.x * Index - PageSize.x * 0.5f;
            var curPosX = Content.localPosition.x;
            if (Mathf.Abs(targetPosX - curPosX) > 1)
            {
                var x = Mathf.SmoothDamp(curPosX, targetPosX, ref mSmoothSpeed, SmoothTime);
                Content.localPosition = new Vector3(x, Content.localPosition.y, 0);
            }
            else
            {
                mNeedMove = false;
                Content.localPosition = new Vector3(targetPosX, Content.localPosition.y, 0);
            }
        }
    }
}

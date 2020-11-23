using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EdgeFramework.Util;

namespace EdgeFramework
{

    public partial class MonoBehaviourSimplify:MonoBehaviour
    {
        /// <summary>
        /// 执行协程
        /// </summary>
        /// <param name="seconds">等待的秒数</param>
        /// <param name="onFinished">执行的函数Action</param>
        public void Delay(float seconds, Action onFinished)
        {
            StartCoroutine(DelayCoroutine(seconds, onFinished));
        }
        private IEnumerator DelayCoroutine(float seconds, Action onFinished)
        {
            yield return new WaitForSeconds(seconds);
            onFinished();
            
        }
    }
    public abstract partial class MonoBehaviourSimplify
    {
        List<MsgRecord> mMsgRecorder = new List<MsgRecord>();
        class MsgRecord
        {
            private MsgRecord() { }
            static Stack<MsgRecord> mMsgRecordPool = new Stack<MsgRecord>();
            public static MsgRecord Allocate(string msgName, Action<object> onMsgReceived)
            {
                var retRecord = mMsgRecordPool.Count > 0 ? mMsgRecordPool.Pop(): new MsgRecord();
                retRecord.Name = msgName;
                retRecord.OnMsgReceived = onMsgReceived;
                return retRecord;
            }
            public void Recycle()
            {
                Name = null;
                OnMsgReceived = null;
                mMsgRecordPool.Push(this);
            }
            public string Name;
            public Action<object> OnMsgReceived;
        }
        public void RegisterMsg(string msgName, Action<object> onMsgReceived)
        {
            MsgDispatcher.Register(msgName, onMsgReceived);
            mMsgRecorder.Add(MsgRecord.Allocate(msgName, onMsgReceived));
        }
        public void SendMsg(string msgName, object data)
        {
            MsgDispatcher.Send(msgName, data);
        }
        public void UnRegisterMsg(string msgName)
        {
            var SelectedRecords = mMsgRecorder.FindAll(record => record.Name == msgName);
            SelectedRecords.ForEach(record =>
            {
                MsgDispatcher.UnRegister(record.Name, record.OnMsgReceived);
                mMsgRecorder.Remove(record);

                record.Recycle();

            });
            SelectedRecords.Clear();
        }
        public void UnRegisterMsg(string msgName, Action<object> onMsgReceived)
        {
            var selectedRecords = mMsgRecorder.FindAll(record => record.Name == msgName && record.OnMsgReceived == onMsgReceived);

            selectedRecords.ForEach(record =>
            {
                MsgDispatcher.UnRegister(record.Name, record.OnMsgReceived);
                mMsgRecorder.Remove(record);

                record.Recycle();
            });


            selectedRecords.Clear();
        }
        private void OnDestroy()
        {
            OnBeforeDestroy();
            foreach (var msgRecord in mMsgRecorder)
            {
                MsgDispatcher.UnRegister(msgRecord.Name, msgRecord.OnMsgReceived);
                msgRecord.Recycle();
            }
            mMsgRecorder.Clear();
        }
        protected abstract void OnBeforeDestroy();
    }

}


using UnityEngine;

namespace Ls_Mobile.Example
{
    public class ARCDocument 
    {
        class Light
        {
            public void Open()
            {
                Log.I("灯打开了");
            }

            public void Close()
            {
                Log.I("灯关闭了");
            }
        }
        class Room : SimpleRC
        {
            private Light mLight = new Light();

            public void EnterPeople()
            {
                if (RefCount == 0)
                {
                    mLight.Open();
                }

                Retain();

                Log.I("一个人走进房间,房间里当前有{0}个人", RefCount);
            }

            public void LeavePeople()
            {
                // 当前还没走出，所以输出的时候先减1
                Log.I("一个人走出房间,房间里当前有{0}个人", RefCount - 1);

                // 这里才真正的走出了
                Release();
            }

            protected override void OnZeroRef()
            {
                mLight.Close();
            }
        }
        public class SimpleRCExample : MonoBehaviour
        {

            // Use this for initialization
            void Start()
            {
                var room = new Room();
                room.EnterPeople();
                room.EnterPeople();
                room.EnterPeople();

                room.LeavePeople();
                room.LeavePeople();
                room.LeavePeople();

                room.EnterPeople();
            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}

using EdgeFramework;
using UnityEngine;

public class SimpleRCTests
    {
        class Light
        {
            public bool Opening { get; private set; }
            public void Open()
            {
                Opening = true;
            }

            public void Close()
            {
                Opening = false;
            }
        }

        class Room : SimpleRC
        {
            public readonly Light Light = new Light();

            public void EnterPeople()
            {
                if (RefCount == 0)
                {
                    Light.Open();
                }

                Retain();

            }

            public void LeavePeople()
            {
                // 这里才真正的走出了
                Release();
            }

            protected override void OnZeroRef()
            {
                Light.Close();
            }
        }


        public void SimpleRC_Test()
        {
            var room = new Room();
        Debug.Log(room.RefCount);
        Debug.Log(room.Light.Opening);
   

            room.EnterPeople();

        Debug.Log(room.RefCount);
        Debug.Log(room.Light.Opening);

        room.EnterPeople();
            room.EnterPeople();

          
        Debug.Log(room.RefCount);
   
        room.LeavePeople();
            room.LeavePeople();
            room.LeavePeople();

        Debug.Log(room.RefCount);
        Debug.Log(room.Light.Opening);

        room.EnterPeople();

        Debug.Log(room.RefCount);
    }
    }

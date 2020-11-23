using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EdgeFramework.Example

{
    public class JsonPathProtobuf : MonoBehaviour
    {
        private void Start()
        {
            var tempProto = new ProtoBufTest
            {
                ID = 1,
                Msg = "Hello"
            };

            var tempJson = new JsonTest { Age = 18 };

            this.Sequence()
                .Until(() => { return Input.GetKeyDown(KeyCode.P); })
                .Event(() => { string path = "Assets/TestJosn".CreateDirIfNotExists(); path += "/testPro.proto"; tempProto.SaveProtoBuff(path); })
                .Begin();

            this.Sequence()
                .Until(() => { return Input.GetKeyDown(KeyCode.O); })
                .Event(() =>
                {
                    ProtoBufTest tempLoadBuf = SerializeHelper.LoadProtoBuff<ProtoBufTest>(Application.dataPath + "/TestJosn/testPro.proto");
                    Debug.Log(tempLoadBuf.ID);
                })
                .Begin();

            this.Sequence()
                .Until(() => { return Input.GetKeyDown(KeyCode.A); })
                .Event(() => { string path = "Assets/TestJosn".CreateDirIfNotExists(); path += "/TestJson.json"; tempJson.SaveJson(path); })
                .Begin();

            this.Sequence()
                .Until(() => { return Input.GetKeyDown(KeyCode.S); })
                .Event(() =>
                {
                    JsonTest tempLoadJson = SerializeHelper.LoadJson<JsonTest>(Application.dataPath + "/TestJosn/TestJson.json");
                    Debug.Log(tempLoadJson.Age);
                })
                .Begin();
        }

    }

    [ProtoBuf.ProtoContract]
    public class ProtoBufTest
    {
        [ProtoBuf.ProtoMember(1)]
        public int ID = 0;

        [ProtoBuf.ProtoMember(2)]
        public string Msg = "Hello";
    }

    [System.Serializable]
    public class JsonTest
    {
        private string mName;
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        private int mAge;
        public int Age
        {
            get { return mAge; }
            set { mAge = value; }
        }
    }
}
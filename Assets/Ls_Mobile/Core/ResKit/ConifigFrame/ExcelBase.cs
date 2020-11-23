using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EdgeFramework
{
    [System.Serializable]
    public class ExcelBase
    {
#if UNITY_EDITOR
        public virtual void Construction() { }

#endif
        public virtual void Init() { }
    }
}
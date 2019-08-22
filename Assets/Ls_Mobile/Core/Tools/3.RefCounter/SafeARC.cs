using com.ls_mobile.tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ls_Mobile
{
    public class SafeARC : IRefCounter
    {
        public int RefCount
        {
            get { return mOwners.Count; }
        }

        public HashSet<object> Owners
        {
            get { return mOwners; }
        }

        readonly HashSet<object> mOwners = new HashSet<object>();

        public void Retain(object refOwner)
        {
            if (!Owners.Add(refOwner))
            {
                Log.E("ObjectIsAlreadyRetainedByOwnerException");
            }
        }

        public void Release(object refOwner)
        {
            if (!Owners.Remove(refOwner))
            {
                Log.E("ObjectIsNotRetainedByOwnerExceptionWithHint");
            }
        }

    }
}
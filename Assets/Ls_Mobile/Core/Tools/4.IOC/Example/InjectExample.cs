using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ls_Mobile.Example
{
    public class InjectExample : MonoBehaviour
    {
        [Inject] public A AObj;

        // Use this for initialization
        void Start()
        {
            var container = new LFrameworkContainer();
            container.RegisterInstance(new A());
            container.Inject(this);

            container.Resolve<A>().HelloWorld();
        }

        public class A
        {
            public void HelloWorld()
            {
                "This is A obj".LogInfo();
            }
        }
    }
}
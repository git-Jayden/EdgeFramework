using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    public  class TestCLRBinding
    {
        public static void RunTest()
        {
            for (int i = 0; i < 100000; i++)
            {
                CRLBindingTestClass.DoSomeTest(i,i);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace myGitTest
{
    class testClass
    {
        int x = 30;
        int clone = 300;
        public testClass()
        {
            Console.WriteLine("main branch");
            clone = 300;
        }
        public testClass(int cl)
        {
            clone = cl;
        }

    }
}

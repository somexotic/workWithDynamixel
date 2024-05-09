using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace workWithDynamixel
{
    internal class AR_DRIVER : PeripheryBase
    {
        public AR_DRIVER(int id)
        {
            gotId = id;
        }

        public override void createArduinoFile()
        {
            throw new NotImplementedException();
        }

        public override void testDevice()
        {
            return;
        }

        public override bool needThreadPause()
        {
            return true;
        }
    }
}

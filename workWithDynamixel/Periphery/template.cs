using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace workWithDynamixel
{
    internal class AR_TEMPLATE : PeripheryBase
    {
        public AR_TEMPLATE(int id)
        {
            gotId = id;
        }

        public override void createArduinoFile()
        {
            throw new NotImplementedException();
        }

        public override void testDevice()
        {
            throw new NotImplementedException();
        }

        public override bool needThreadPause()
        {
            throw new NotImplementedException();
        }
    }
}

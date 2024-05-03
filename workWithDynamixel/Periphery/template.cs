using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace workWithDynamixel.Periphery
{
    internal class AR_TEMPLATE : PeripheryBase
    {
        public AR_TEMPLATE(int id)
        {
            gotId = id;
        }

        public override void getRegistersById(int id, Form1 form, CancellationToken token, ManualResetEvent pause)
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace workWithDynamixel
{
    internal class AR_BALL : PeripheryBase
    {
        public AR_BALL(int id)
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
            return false;
        }
    }
}

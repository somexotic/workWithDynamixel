using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace workWithDynamixel
{
    internal class Servo : PeripheryBase
    {
        public Servo(int id)
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

        public override void setTorque(bool status)
        {
            if(status)
            {
                dyn.writeReg(gotId, 64, 1, 1);
            }
            else
            {
                dyn.writeReg(gotId, 64, 0, 1);
            }
        }

        public override void setLED(bool status)
        {
            if (status)
            {
                dyn.writeReg(gotId, 65, 1, 1);
            }
            else
            {
                dyn.writeReg(gotId, 65, 0, 1);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workWithDynamixel
{
    internal class AR_LED : PeripheryBase
    {
        public AR_LED(int id)
        {
            gotId = id;
        }

        public override void createArduinoFile()
        {
            throw new NotImplementedException();
        }

        public override void testDevice()
        {
            dyn.writeReg(gotId, 26, 255, 1);
            MessageBox.Show("Светодиод должен загореться");
            Thread.Sleep(200);
            dyn.writeReg(gotId, 26, 0, 1);
        }

        public override bool needThreadPause()
        {
            return true;
        }
    }
}

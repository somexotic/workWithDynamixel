using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using dynamixel_sdk;

namespace workWithDynamixel
{
    internal class baseDynamixel
    {
        public string com_port = "";
        public int gotBaudrate = 0;
        public void saver()
        {
            int port_num = dynamixel.portHandler("COM6");
            dynamixel.packetHandler();
            dynamixel.openPort(port_num);
            dynamixel.setBaudRate(port_num, 57600);
            dynamixel.closePort(port_num);
        }

        public async Task<List<int>> findDynamixel(string comPort, int baudrate)
        {
            if (baudrate < 1) return null;
            List<int> list = new List<int>();
            await Task.Run(() =>
            {
                int port_num = dynamixel.portHandler(comPort);
                dynamixel.packetHandler();
                try
                {
                    dynamixel.openPort(port_num);
                    dynamixel.setBaudRate(port_num, baudrate);
                    for (byte i = 1; i <= 100; i++)
                    {
                        int value = dynamixel.read1ByteTxRx(port_num, 1, i, 3);
                        if (value != 0)
                        {
                            list.Add(value);
                        }
                    }
                    if(list.Count > 0)
                    {
                        com_port = comPort;
                        gotBaudrate = baudrate;

                    }
                    dynamixel.closePort(port_num);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            return list;
        }

        public async Task<Dictionary<int,int>> getReg(int id, List<int> regId, List<int> regLen)
        {
            Dictionary<int, int> list = new Dictionary<int,int>();
            await Task.Run(() =>
            {
                int port_num = dynamixel.portHandler(com_port);
                dynamixel.packetHandler();
                try
                {
                    dynamixel.openPort(port_num);
                    dynamixel.setBaudRate(port_num, gotBaudrate);
                    for(int i = 1; i < regId.Count; i++)
                    {
                        switch (regLen[i])
                        {
                            case 1:
                                list.Add(regId[id], dynamixel.read1ByteTxRx(port_num, 1, (byte)id, (ushort)regId[i]));
                                break;
                            case 2:
                                list.Add(regId[id], dynamixel.read2ByteTxRx(port_num, 1, (byte)id, (ushort)regId[i]));
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            return list;
        }
    }
}

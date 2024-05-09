using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using dynamixel_sdk;

namespace workWithDynamixel
{
    internal class baseDynamixel
    {
        public List<int> listOfIds = new List<int>();
        public List<string> listOfNames = new List<string>();
        protected Dictionary<int, int> regData = new Dictionary<int, int>();
        protected dataStruct gotStruct = null;
        protected Form1 gotForm;

        public baseDynamixel(Form1 form)
        {
            gotForm = form;
        }
        public baseDynamixel () { }

        public async Task findDynamixel(string comPort, int baudrate, int len)
        {
            if(listOfIds.Count > 0)
            {
                dynamixel.closePort(storage.openedPort);
            }
            listOfNames.Clear();
            listOfIds.Clear();
            if (baudrate < 1) return;
            await Task.Run(() =>
            {
                try
                {
                    int port_num = dynamixel.portHandler(comPort);
                    dynamixel.packetHandler();
                    dynamixel.clearPort(port_num);
                    dynamixel.openPort(port_num);
                    dynamixel.setBaudRate(port_num, baudrate);
                    for (byte i = 1; i <= len; i++)
                    {
                        int value = dynamixel.read1ByteTxRx(port_num, 1, i, 3);
                        if (value != 0)
                        {
                            listOfIds.Add(value);
                            int baseId = dynamixel.read1ByteTxRx(port_num, 1, i, 0);
                            listOfNames.Add(Enum.GetName(typeof(BaseReg), baseId));
                        }
                    }
                    if(listOfIds.Count > 0)
                    {
                        storage.openedPort = port_num;
                        storage.com_port = comPort;
                        storage.gotBaudrate = baudrate;
                    }
                    else
                    {
                        dynamixel.closePort(port_num);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        public async Task resetIds()
        {
            await Task.Run(() =>
            {
                for (byte i = 1; i < 255; i++)
                {
                    int baseId = dynamixel.read1ByteTxRx(storage.openedPort, 1, i, 0);
                    if (baseId != 0)
                    { 
                        dynamixel.write1ByteTxRx(storage.openedPort, 1, i, 3, (byte)baseId);
                    }
                }
                MessageBox.Show("Выполнено");
            });
        }

        public Dictionary<int, int> getReg(object data)
        {
            gotStruct = (dataStruct)data;
            regData.Clear();
            try
            {
                for (int i = 0; i < gotStruct.regToRead.Count; i++)
                {
                    switch (gotStruct.lengOfReg[i])
                    {
                        case 1:
                            regData.Add(gotStruct.regToRead[i], dynamixel.read1ByteTxRx(storage.openedPort, 1, (byte)storage.lastId, (ushort)gotStruct.regToRead[i]));
                            break;
                        case 2:
                            regData.Add(gotStruct.regToRead[i], dynamixel.read2ByteTxRx(storage.openedPort, 1, (byte)storage.lastId, (ushort)gotStruct.regToRead[i]));
                            break;
                    }
                    
                }
                return regData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return regData;
        }

        public void writeReg(int id, int reg, int value, int byteSize)
        {
            try
            {
                switch(byteSize)
                {
                    case 1:
                        dynamixel.write1ByteTxRx(storage.openedPort, 1, (byte)id, (ushort)reg, (byte)value);
                        break;
                    case 2:
                        dynamixel.write2ByteTxRx(storage.openedPort, 1, (byte)id, (ushort)reg, (ushort)value);
                        break;
                }
            }
            catch
            {
                return;
            }
        }
    }
}
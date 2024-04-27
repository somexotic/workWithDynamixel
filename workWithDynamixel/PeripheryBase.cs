using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace workWithDynamixel
{
    //цепочка обязанностей
    internal abstract class PeripheryBase
    {
        protected baseDynamixel dyn = new baseDynamixel();
        public Dictionary<int, int> registers = new Dictionary<int, int>();
        public abstract void getRegistersById(int id);
        //public abstract Dictionary<int, int> getRegistersByType(string type);

        protected List<int> regToRead = new List<int>();
        protected List<int> lengOfReg = new List<int>();
        protected void getRegData(string type)
        {
            for(int i = 1; i < 23; i++)
            {
                regToRead.Add(i);
                lengOfReg.Add(1);
            }
            switch (type)
            {
                case "AR_BUTTON":
                    for(int i = 24; i < 50; i++)
                    {
                        regToRead.Add(i);
                        lengOfReg.Add(1);
                    }
                    break;
                case "AR_TEMP":
                    for(int i = 24; i < 50; i++)
                    {
                        switch (i)
                        {
                            case 26:
                                regToRead.Add(i);
                                lengOfReg.Add(2);
                                i++;
                                break;
                            default:
                                regToRead.Add(i);
                                lengOfReg.Add(1);
                                break;
                        }
                        
                    }
                    break;
            }
        }
    }

    internal class AR_LED : PeripheryBase
    {

        public override void getRegistersById(int id)
        {
            //return registers;
        }
    }

    internal class AR_BUTTON : PeripheryBase
    {

        public async override void getRegistersById(int id)
        {
            getRegData("AR_BUTTON");
            registers =  await dyn.getReg(id, regToRead, lengOfReg);
        }
    }
}

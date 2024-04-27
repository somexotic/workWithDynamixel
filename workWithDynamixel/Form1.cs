using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace workWithDynamixel
{
    public partial class Form1 : Form
    {
        baseDynamixel dyn = new baseDynamixel();
        Dictionary<int, baseDynamixel> dynMap = new Dictionary<int, baseDynamixel>();

        public Form1()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            foreach(string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            comboBox1.SelectedIndex = 0;

            // super -> label3.Text = Enum.GetName(typeof(BaseReg), 3);

            /*dataGridView1.ColumnCount = 1;
            dataGridView1.Columns[0].Name = "test";
            dataGridView1.Columns[0].Width = 340;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Rows.Add("Регистр: 27 qew qweqw qwe ");
            DataGridViewButtonColumn cell = new DataGridViewButtonColumn();
            cell.HeaderText = "Button Column";
            cell.Text = "Click Me";
            cell.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(cell);*/
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (baud.Text == "") return;
                button1.Enabled = false;
                List<int> list = await dyn.findDynamixel(comboBox1.SelectedItem.ToString(), Int32.Parse(baud.Text));
                
                for(int i = 0; i < list.Count; i++)
                {
                    string str = "id: " + list[i].ToString();
                    listBox1.Items.Add(str);
                }
                button1.Enabled = true;
            }
            catch (Exception ex)
            {
                button1.Enabled = true;
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = listBox1.SelectedItem.ToString();
            string str = selected[4].ToString();
            if(selected.Length > 6) {
                str += selected[5].ToString();
                if(selected.Length == 7)
                {
                    str += selected[6].ToString();
                }
            }
            int gotId = Int32.Parse(str);

            if(!dynMap.ContainsKey(gotId))
            {
                //dynMap.Add(gotId,)
            }
        }
    }
}

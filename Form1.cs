using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace UpperComputer2
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            string[] baud = { "300", "1200", "2400", "4800", "9600", "19200", "38400", "57600" };
            comboBox2.Items.AddRange(baud);     //添加波特率列表
            string[] sps = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(sps);


            //设置默认值
            comboBox1.Text = "COM3";
            comboBox2.Text = "9600";
            comboBox3.Text = "8";
            comboBox4.Text = "None";
            comboBox5.Text = "1";

           
        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Focus();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            //trycatcj处理串口打开过程中的异常
            try
            {
                //将可能产生异常的代码放置在try块中
                //根据当前串口属性来判断是否打开
                if (serialPort1.IsOpen)
                {
                    //串口已经处于打开状态
                    serialPort1.Close();    //关闭串口
                    button1.Text = "打开串口";
                    button1.BackColor = Color.ForestGreen;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox4.Enabled = true;
                    comboBox5.Enabled = true;
                    ReceptTb.Text = "";  //清空接收区
                    SendTb.Text = "";     //清空发送区
                }
                else
                {
                    //串口已经处于关闭状态，则设置好串口属性后打开
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.DataBits = Convert.ToInt16(comboBox3.Text);

                    if (comboBox4.Text.Equals("None"))
                        serialPort1.Parity = System.IO.Ports.Parity.None;
                    else if (comboBox4.Text.Equals("Odd"))
                        serialPort1.Parity = System.IO.Ports.Parity.Odd;
                    else if (comboBox4.Text.Equals("Even"))
                        serialPort1.Parity = System.IO.Ports.Parity.Even;
                    else if (comboBox4.Text.Equals("Mark"))
                        serialPort1.Parity = System.IO.Ports.Parity.Mark;
                    else if (comboBox4.Text.Equals("Space"))
                        serialPort1.Parity = System.IO.Ports.Parity.Space;

                    if (comboBox5.Text.Equals("1"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.One;
                    else if (comboBox5.Text.Equals("1.5"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.OnePointFive;
                    else if (comboBox5.Text.Equals("2"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.Two;

                    serialPort1.Open();     //打开串口
                    button1.Text = "关闭串口";
                    button1.BackColor = Color.Firebrick;
         
                    this.Activate();
                }
            }
            catch (Exception ex)
            {
                //捕获可能发生的异常并进行处理

                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }
        }



        private void SendBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(serialPort1.IsOpen);
                //首先判断串口是否开启
                if (serialPort1.IsOpen)
                {
                    //串口处于开启状态，将发送区文本发送
                    serialPort1.Write(SendTb.Text);
                    SendTb.Text = "";
                }
            }
            catch (Exception ex)
            {
                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }
        }

   
        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                //因为要访问UI资源，所以需要使用invoke方式同步ui
                this.Invoke((EventHandler)(delegate
                {
                    //String str = serialPort1.ReadExisting();
                    //string[] strArray = str.Split(',');

                    //Console.Write(str);
                    //ReceptTb.AppendText(str);
                    int byteNumber = serialPort1.BytesToRead;

                    Thread.Sleep(20);


                    //延时等待数据接收完毕。
                    while ((byteNumber < serialPort1.BytesToRead) && (serialPort1.BytesToRead < 77))
                    {
                        byteNumber = serialPort1.BytesToRead;
                        Thread.Sleep(20);

                    }
                    int n = serialPort1.BytesToRead; //记录下缓冲区的字节个数 
                    byte[] buf = new byte[n]; //声明一个临时数组存储当前来的串口数据 
                    serialPort1.Read(buf, 0, n); //读取缓冲数据到buf中，同时将这串数据从缓冲区移除 
                    string str = System.Text.Encoding.Default.GetString(buf);
                    string[] strArray = str.Split(',');
                    

                    int count = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if(count == 0)
                        {
                            textBox1.Text = strArray[i];
                        }else if(count == 1)
                        {
                            textBox2.Text = strArray[i];
                        }else if(count == 2)
                        {
                            textBox3.Text = strArray[i];
                        }
                        count++;
                        if(count == 3)
                        {
                            count = 0;
                        }

                        
                        //textBox2.Text = strArray[i + 1];
                        //textBox3.Text = strArray[i + 2];
                        //Console.WriteLine(strArray[i]);
                    }



                    //StringBuilder sb = new StringBuilder();
                    //for (int i = 0; i < n; i++)
                    //{
                    //    string s;
                    //    if (buf[i] < 16)
                    //        s = "0" + Convert.ToString(buf[i], 16).ToUpper() + " ";
                    //    else
                    //        s = Convert.ToString(buf[i], 16).ToUpper() + " ";


                    //    sb.Append(s);
                    //}

                    //string str = sb.ToString();
                    //float Num = float.Parse(str);


                    Console.Write(str);

                }
                   )
                );

            }
            catch (Exception ex)
            {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show(ex.Message);

            }
        }
        //按下上下左右进行反应
        //左箭头     37   
        //向上箭头    38
        //右箭头     39
        //向下箭头    40



        private void Button2_KeyDown(object sender, KeyEventArgs e)
        {
            //重点在于要重新聚焦
            String key = e.KeyValue.ToString();
            String signal = "";
            Console.WriteLine(key);
            switch (key)
            {
               // case "65":
                case "37":
                    signal = "l";
                    //按下左
                    break;
               // case "68":
                case "39":
                    signal = "r";
                    //按下小键盘1以后
                    break;
               // case "87":
                case "38":
                    signal = "f";
                    //按下向上键以后
                    break;
               // case "83":
                case "40":
                    signal = "b";
                    //按下向下键以后
                    break;
            }

            Console.WriteLine(signal);




            try
            {

                //首先判断串口是否开启
                if (serialPort1.IsOpen)
                {
                    //串口处于开启状态，发送
                    serialPort1.Write(signal);
                    SendTb.Text = "";
                }
            }
            catch (Exception ex)
            {
                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }

        }
        //Ascii转字符串
        public static string Ascii2Str(byte[] buf)
        {
            return System.Text.Encoding.ASCII.GetString(buf);
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //重点在于要重新聚焦
            String key = e.KeyValue.ToString();
            String signal = "";
     
            switch (key)
            {
                //case "65":
                case "37": signal = "l";
                    //按下左
                    break;
               // case "68":
                case "39": signal = "r";
                    //按下小键盘1以后
                    break;
               // case "87":
                case "38": signal = "f";
                    //按下向上键以后
                    break;
                //case "83":
                case "40": signal = "b";
                    //按下向下键以后
                    break;
            }

            Console.WriteLine(signal);




            try
            {

                //首先判断串口是否开启
                if (serialPort1.IsOpen)
                {
                    //串口处于开启状态，发送
                    serialPort1.Write(signal);
                }
            }
            catch (Exception ex)
            {
                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }
        }

       
    }
}

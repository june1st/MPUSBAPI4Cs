using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using PVOID = System.IntPtr;
using DWORD = System.UInt32;

namespace test
{
    public partial class Form1 : Form
    {

        [System.Runtime.InteropServices.DllImportAttribute("mpusbapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _MPUSBGetDLLVersion();

        [System.Runtime.InteropServices.DllImportAttribute("mpusbapi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern UInt32 _MPUSBGetDeviceCount(String pVID_PID);


        [System.Runtime.InteropServices.DllImportAttribute("mpusbapi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int _MPUSBOpen(
        UInt32 instance,
        String pVID_PID,
        String pEP,
        UInt32 dwDir,
        UInt32 dwReserved);


        [System.Runtime.InteropServices.DllImportAttribute("mpusbapi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int _MPUSBRead(
        int handle,
        byte[] pData, //String pData,

        UInt32 dwLen,
        out UInt32 pLength,
        UInt32 dwMilliseconds);


        [System.Runtime.InteropServices.DllImportAttribute("mpusbapi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int _MPUSBWrite(
        int handle,
        byte[] pData, //String pData,

        UInt32 dwLen,
        out UInt32 pLength,
        UInt32 dwMilliseconds);


        [System.Runtime.InteropServices.DllImportAttribute("mpusbapi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int _MPUSBReadInt(
        int handle,
        String pData,
        UInt32 dwLen,
        String pLength,
        UInt32 dwMilliseconds);

        
        [System.Runtime.InteropServices.DllImportAttribute("mpusbapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool _MPUSBClose(int handle);

        
        ////////////////////////////////////////////////////////////////////////////////////////////
        
        public const Byte MP_WRITE = 0;
        public const Byte MP_READ = 1;


        internal static readonly string INVALID_HANDLE_VALUE = "";//= new IntPtr(-1);

        static int myOutPipe;// = INVALID_HANDLE_VALUE;
        static int myInPipe;// = INVALID_HANDLE_VALUE;
        const string out_pipe = "\\MCHP_EP1"; // Define End Points
        const string in_pipe = "\\MCHP_EP1";
        public const string vid_pid_norm = "vid_04D8&pid_0053";

        public Form1()
        {
            InitializeComponent();

            richTextBox1.Font = new Font("MSゴシック", 10, FontStyle.Bold);

            DWORD selection = 0; // Selects the device to connect to, in this example it is assumed you will only have one device per vid_pid connected.


            button1.Text = _MPUSBGetDLLVersion().ToString();
                        
            button1.Text = _MPUSBGetDeviceCount(vid_pid_norm).ToString();

            myOutPipe = _MPUSBOpen(selection, vid_pid_norm, out_pipe, 0, 0);
            myInPipe = _MPUSBOpen(selection, vid_pid_norm, in_pipe, 1, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] test = new byte[4];
            test[0] = 0;
            test[1] = 0;
            test[2] = 0;
            test[3] = 0;
            DWORD len = (DWORD)test.Length; 
            _MPUSBWrite(myOutPipe,test, 4, out len, 10);
        }

        byte[] getData(int n) {
            string text = "";
            byte[] temp = new byte[n];
            //string temp = "";
            DWORD plength = 0;
            int res = _MPUSBRead(myInPipe, temp, (uint)n, out plength, 100);
            if (res != -1)
            {
                if (plength == 512)
                {
                    text = BitConverter.ToString(temp);
                    //button1.Text = plength.ToString();
                }
            }
            else
            {
                text = "NG";
            }

            return temp;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            byte[] d1 = getData(64);
            byte[] d2 = getData(512).Take(64).ToArray();

            var data = d2 ;

            richTextBox1.Text += BitConverter.ToString(data).Replace("-",", ") + "\n";

            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.Focus();
            richTextBox1.ScrollToCaret();

            if (richTextBox1.Lines.Length == 50) richTextBox1.Text = "";

        }
    }
}

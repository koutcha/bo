using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication7
{
    static class Program
    {


        private const double waitTime = 1000.0f / 60.0f;

        [STAThread]
        static void Main()
        {
            double targetTime;
            Form1 mainForm = new Form1();
            mainForm.Show();

            targetTime = (double)System.Environment.TickCount;
            targetTime += waitTime;
            while (mainForm.Created)
            {
                if ((double)System.Environment.TickCount >= targetTime)
                {
                    //メインの処理
                    mainForm.RenderFps();
                    targetTime += waitTime;
                }
                System.Threading.Thread.Sleep(1);
                Application.DoEvents();
            }
        }
    }
}


﻿using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Demo
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetTopWindow(IntPtr hWnd);

        private Page page;
        private bool started;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("about:blank");
            while (!IsWindow(GetTopWindow(GetTopWindow(GetTopWindow(webBrowser1.Handle)))))
            {
                Application.DoEvents();
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            started = true;
            txtRequests.Clear();
            page = new Page(GetTopWindow(GetTopWindow(GetTopWindow(webBrowser1.Handle))), this);
            webBrowser1.Navigate(txtAddress.Text);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.ReadyState == WebBrowserReadyState.Complete && !e.Url.Equals("about:blank"))
            {
                System.Timers.Timer timer = new System.Timers.Timer(5000);
                timer.AutoReset = false;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
                timer.Start();
            }
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!started)
            {
                return;
            }

            started = false;

            txtRequests.Invoke((MethodInvoker)delegate()
            {
                foreach (Entry entry in page.AllEntries)
                {
                    txtRequests.AppendText(entry.ToString());
                }
            });
        }
    }
}

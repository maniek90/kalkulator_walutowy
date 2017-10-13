using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace Projekt
{
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
            StreamReader dane = new StreamReader("opis.txt", Encoding.Default);
            textBox1.Text = dane.ReadToEnd();
            StreamReader opis = new StreamReader("opis2.txt", Encoding.Default);
            textBox2.Text = opis.ReadToEnd();                       
        }

        

        
    }
}

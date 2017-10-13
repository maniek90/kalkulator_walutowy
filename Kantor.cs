using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Projekt
{
    public partial class Kantor : Form
    {
        public Kantor()
        {
            InitializeComponent();
        }

        DataSet dsNotowanie = new DataSet("pozycja");
        XmlDocument notowania = new XmlDocument();

        public void Kantor_Load(object sender, EventArgs e)
        {

            if (File.Exists(Application.StartupPath + "\\notowanie.xml"))
            {
                dsNotowanie.ReadXml(Application.StartupPath + "\\notowanie.xml");
                dataGridView1.DataSource = dsNotowanie;
                dataGridView1.DataMember = "pozycja";

                XmlTextReader reader = new XmlTextReader(Application.StartupPath + "\\notowanie.xml");
                XmlNodeType type;
                while (reader.Read())
                {
                    type = reader.NodeType;
                    if (type == XmlNodeType.Element)
                    {
                        if (reader.Name == "data_publikacji")
                        {
                            reader.Read();
                            textBox1.Text = reader.Value;
                        }
                    }
                }
                walutaCombo();
            }
        }

        public string actual_file;
        private void Aktualizuj_Click(object sender, EventArgs e)
        {
            try
            {
                WebClient nazwa = new WebClient();
                nazwa.DownloadFile("http://www.nbp.pl/Kursy/xml/dir.txt", "dir.txt");

                StreamReader objReader = new StreamReader("dir.txt");
                String sLine = "";
                DataTable notowania = new DataTable();

                notowania.Columns.Add("name");

                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                    {
                        if (sLine.StartsWith("a")) //NBP posiada kilka rodzai standardu zapisu, rozróżnia się je po pierwszej literze pliku
                        {
                            DataRow dr = notowania.NewRow();
                            dr["name"] = sLine;
                            notowania.Rows.Add(dr);
                        }
                    }
                }
                actual_file = notowania.Rows[notowania.Rows.Count - 1]["name"].ToString();
                objReader.Close();

                WebClient notowanie = new WebClient();
                notowanie.DownloadFile("http://www.nbp.pl/kursy/xml/" + actual_file + ".xml", "notowanie.xml");

                dsNotowanie.ReadXml(Application.StartupPath + "\\notowanie.xml");
                dataGridView1.DataSource = dsNotowanie;
                dataGridView1.DataMember = "pozycja";
                XmlTextReader reader = new XmlTextReader(Application.StartupPath + "\\notowanie.xml");
                XmlNodeType type;
                while (reader.Read())
                {
                    type = reader.NodeType;
                    if (type == XmlNodeType.Element)
                    {
                        if (reader.Name == "data_publikacji")
                        {
                            reader.Read();
                            textBox1.Text = reader.Value;
                        }
                    }
                }
                MessageBox.Show("Notowania zaktualizowane", "Komunikat",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                walutaCombo();
            }
            catch (Exception)
            {
                MessageBox.Show("Połączenie z serwerem nie powiodło się!", "Błąd",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Zapisz_Click_1(object sender, EventArgs e)
        {
            dsNotowanie.WriteXml(Application.StartupPath + "\\notowanie_" + textBox1.Text + ".xml");
        }

        private void walutaCombo()
        {
            if (File.Exists(Application.StartupPath + "\\notowanie.xml") == false)
            {
                notowania.Load("http://www.nbp.pl/kursy/xml/" + actual_file + ".xml");
                XmlNodeList nodeList = notowania.SelectNodes("tabela_kursow/pozycja");

                foreach (XmlNode node in nodeList)
                    if (!comboBox1.Items.Contains(node.SelectSingleNode("nazwa_waluty").InnerText))
                        comboBox1.Items.Add(node.SelectSingleNode("nazwa_waluty").InnerText);
            }
            else
                notowania.Load(Application.StartupPath + "\\notowanie.xml");
            XmlNodeList potomek = notowania.SelectNodes("tabela_kursow/pozycja");

            foreach (XmlNode node in potomek)
                if (!comboBox1.Items.Contains(node.SelectSingleNode("nazwa_waluty").InnerText))
                    comboBox1.Items.Add(node.SelectSingleNode("nazwa_waluty").InnerText);
        }

        private void Wyczysc_Click_1(object sender, EventArgs e)
        {
            textBox2.Text = "";
            comboBox1.SelectedIndex = -1;
            textBox3.Text = "";
        }

        private void Oblicz_Click_1(object sender, EventArgs e)
        {
            double kasa, przelicznik;
            double wynik = 0;

            kasa = Convert.ToDouble(textBox2.Text);

            notowania.Load(Application.StartupPath + "\\notowanie.xml");
            XmlNodeList nodeList = notowania.SelectNodes("tabela_kursow/pozycja");

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i].SelectSingleNode("nazwa_waluty").InnerText == comboBox1.Text)
                {
                    przelicznik = Convert.ToDouble(nodeList[i].SelectSingleNode("kurs_sredni").InnerText);
                    wynik = Math.Round((kasa / przelicznik), 2);
                }
            }
            textBox3.Text = "Za " + textBox2.Text + " złotych możesz kupić " + Convert.ToString(wynik) + " " + comboBox1.Text;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime data = DateTime.Now;
            toolStripStatusLabel2.Text = data.ToString();
        }

        private void Autor_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Show();
        }
   
    }
}
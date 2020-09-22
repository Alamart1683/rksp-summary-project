using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Stuff : Form
    {
        ServiceReference1.Service1Client Svc = new ServiceReference1.Service1Client();

        public Stuff()
        {
            InitializeComponent();
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox1.SelectedItem = "5";
        }

        private void Stuff_Load(object sender, EventArgs e)
        {
            aFTDataSet.Educational_contract.Merge(Svc.GetEducational_Contracts());
            aFTDataSet.Student.Merge(Svc.GetAllStudents());
            aFTDataSet.Price.Merge(Svc.GetAllPrices());
        }

        // Спросим о том, хочет ли пользователь выйти
        private void Stuff_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Вы уверены, что намереваетесь выйти?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.No)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    return;
                }
            }
            Form auth = Application.OpenForms[0];
            auth.Show();
        }

        // Синхронизироваться с бд
        private void button1_Click(object sender, EventArgs e)
        {
            educational_contractTableAdapter1.Update(aFTDataSet.Educational_contract);
            priceTableAdapter1.Update(aFTDataSet.Price);
            studentTableAdapter1.Update(aFTDataSet.Student);
        }

        // Повысить стоимость обучения на указанный процент
        private void button2_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals(""))
            {
                int procent = 0;
                try { procent = Convert.ToInt32(comboBox1.SelectedItem); }
                catch { MessageBox.Show("Процент не может быть не числом!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (Svc.up_price(textBox1.Text, procent)) MessageBox.Show("Стоимость обучения повышена успешно!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("Триггер повышения стоимости зафиксировал превышение максимальной стоимости!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                aFTDataSet.Clear();
                object obj = new object();
                EventArgs evnt = new EventArgs();
                Stuff_Load(obj, evnt);
                textBox1.Text = "";
            }
            else MessageBox.Show("Не все поля ввода заполнены верно, необходимо заполнить поля специальности и процента!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Заплатил ли студент все семестры
        private void button3_Click(object sender, EventArgs e)
        {
            if (!textBox3.Text.Equals("") && !textBox2.Text.Equals(""))
            {
                if (Svc.good_student(textBox3.Text, textBox2.Text))
                {
                    MessageBox.Show("Данный студент находится в списке оплативших все семестры!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Данный студент не находится в списке оплативших все семестры!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                textBox3.Text = "";
                textBox2.Text = "";
            }
            else MessageBox.Show("Не все поля ввода заполнены верно, необходимо заполнить поля фамилии и группы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Узнать количество должников по указанной специальности
        private void button4_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals(""))
            {
                if (!Svc.bad_student(textBox1.Text).Equals(""))
                {
                    MessageBox.Show("По специальности с кодом " + textBox1.Text + " в базе данных имеется " + Svc.bad_student(textBox1.Text) + " должников!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox1.Text = "";
                }
                else
                {
                    MessageBox.Show("По специальности с кодом " + textBox1.Text + " в базе данных не имеется должников!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else MessageBox.Show("Не все поля ввода заполнены верно, необходимо заполнить поле кода специальности!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

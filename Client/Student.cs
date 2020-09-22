using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Client
{
    public partial class Student : Form
    {
        ServiceReference1.Service1Client Svc = new ServiceReference1.Service1Client();

        public Student()
        {
            InitializeComponent();
        }

        private void Student_FormClosing(object sender, FormClosingEventArgs e)
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

        // Вывести договоры
        private void button1_Click(object sender, System.EventArgs e)
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection("Data Source=DELAMART\\SQLEXP;Initial Catalog=accounting_for_tuition;Integrated Security=True"))
            {
                if (!textBox1.Text.Equals("") && !textBox2.Equals(""))
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand("select * from Educational_contract where surname = '" + textBox1.Text + "' and [group] = '" + textBox2.Text + "'", connection);
                    connection.Open();
                    da.Fill(ds);
                    dataGridView1.DataSource = ds.Tables[0];
                    dataGridView1.Columns[0].HeaderText = "ID договора";
                    dataGridView1.Columns[1].HeaderText = "ID студента";
                    dataGridView1.Columns[2].HeaderText = "Фамилия";
                    dataGridView1.Columns[3].HeaderText = "Группа";
                    dataGridView1.Columns[4].HeaderText = "Отделение";
                    dataGridView1.Columns[5].HeaderText = "ID стоимости";
                    dataGridView1.Columns[6].HeaderText = "Специальность";
                    dataGridView1.Columns[7].HeaderText = "Номер семестра";
                    dataGridView1.Columns[8].HeaderText = "Сумма";
                    dataGridView1.Columns[9].HeaderText = "Дата";
                    textBox1.Text = "";
                    textBox2.Text = "";
                }
                else
                {
                    MessageBox.Show("Не все поля ввода заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

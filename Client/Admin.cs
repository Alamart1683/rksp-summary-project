using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Client
{
    public partial class Admin : Form
    {
        ServiceReference1.Service1Client Svc = new ServiceReference1.Service1Client();

        public Admin()
        {
            InitializeComponent();
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            aFTDataSet.users.Merge(Svc.AdminGetUsers());
            if (!label2.Text.Contains(User.username))
                label2.Text += User.username;
            if ((!label3.Text.Contains(User.type)))
                label3.Text += User.type;
        }

        // Удалить пользователя
        private void button1_Click(object sender, EventArgs e)
        {
            user_removing(); aFTDataSet.Clear();
            object obj = new object(); EventArgs evnt = new EventArgs();
            Admin_Load(obj, evnt);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Проверка допустимой длины
            if (textBox1.Text.Length > 3 && textBox2.Text.Length > 3 || textBox3.Text.Length > 3)
            {
                // Проверка совпадения паролей
                if (textBox2.Text.Equals(textBox3.Text))
                {
                    // Проверка на то, что был выбран уровень доступа
                    if (comboBox1.SelectedItem.ToString().Length != 0)
                    {
                        // Проверка на допустимость введенных символов
                        if (Regex.IsMatch(textBox1.Text + textBox2.Text + textBox3.Text, @"^[a-zA-Z0-9]+$"))
                        {
                            user_creation();
                            object obj = new object(); EventArgs evnt = new EventArgs();
                            Admin_Load(obj, evnt);
                        }
                        else
                        {
                            MessageBox.Show("Для ввода имени и пароля допустимы только латинские буквы и цифры!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не выбран уровень доступа!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Введенные пароли не совпадают!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Длина пароля и логина должна превышать 3 символа!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод удаления пользователя из базы данных
        private void user_removing()
        {
            // Потребуем подтверждение удаления
            DialogResult dialog = MessageBox.Show("Вы уверены, что хотите удалить этого пользователя?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                String removing_user_name = "";
                String removing_user_type = "";
                // Выбираем удаляемого пользователя
                foreach (DataGridViewRow row in usersDataGridView.SelectedRows)
                {
                    removing_user_name = row.Cells[0].Value.ToString(); // Получим логин удаляемого
                    removing_user_type = row.Cells[1].Value.ToString(); // Получим тип его учетной записи
                }
                // Проверяем, допустимо ли удаление данного пользователя
                if (!User.username.Equals(removing_user_name) && !User.type.Equals(removing_user_type) && !removing_user_type.Equals("root"))
                {
                    Svc.user_removing(removing_user_name);
                    MessageBox.Show("Пользователь был удален успешно!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Недостаточно прав для удаления данного пользователя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                return;
            }
        }

        // Метод создания пользователя
        private void user_creation()
        {
            // Проверим, свободно ли имя
            if (name_free_detector())
            {
                // Попытка соединения с сервером
                try
                {
                    Svc.user_creating(textBox1.Text, textBox2.Text, comboBox1.SelectedItem.ToString());
                    MessageBox.Show("Пользователь был создан успешно!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox1.Text = ""; textBox2.Text = ""; textBox3.Text = "";
                }
                catch
                {
                    MessageBox.Show("Не удается установить соединение с сервером!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Данный логин занят!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод проверки существования пользователя
        private bool name_free_detector()
        {
            DataTableReader reader = aFTDataSet.CreateDataReader();
            // Построчно считываем пользователей
            while (reader.Read())
            {
                // Если логин найден в списке пользователей и пароль совпал с введенным
                if (textBox1.Text.Equals(reader.GetValue(1)))
                {
                    return false;
                }
            }
            return true;
        }

        // Спросим о том, хочет ли пользователь выйти
        private void Admin_FormClosing(object sender, FormClosingEventArgs e)
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
    }
}

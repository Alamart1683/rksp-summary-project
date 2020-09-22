using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Client
{
    public partial class Auth : Form
    {
        public Auth()
        {
            InitializeComponent();
        }

        private void Auth_Load(object sender, EventArgs e)
        {
            ServiceReference1.Service1Client Svc = new ServiceReference1.Service1Client();
            aFTDataSet.users.Merge(Svc.GetAllUsers());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            autentification();
        }

        
        // Метод авторизации в клиент-серверном приложении
        private void autentification()
        {
            // Проверка заполнености полей авторизации:
            if (!textBox1.Text.Equals("") && !textBox2.Text.Equals(""))
            {
                DataTableReader reader = aFTDataSet.CreateDataReader();
                while (reader.Read())
                {
                    // Если логин найден в списке пользователей и пароль совпал с введенным
                    if (textBox1.Text.Equals(reader.GetValue(1)) && textBox2.Text.Equals(reader.GetValue(2)))
                    {
                        //MessageBox.Show("Авторизация прошла успешно!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        User.username = reader.GetValue(1).ToString();
                        User.password = reader.GetValue(2).ToString();
                        User.type = reader.GetValue(3).ToString();
                        // Очистим поля ввода
                        textBox1.Text = ""; textBox2.Text = "";
                        // Создание объектов форм
                        Admin admin = new Admin();
                        Stuff stuff = new Stuff();
                        Student studentform = new Student();
                        // Теперь необходимо предоставить каждому пользователю соответствующую ему форму
                        switch (User.type)
                        {
                            // Открыть форму админа с рут правами
                            case "root":
                                admin.Show();
                                Hide();
                                break;
                            case "admin":
                                admin.Show();
                                Hide();
                                break;
                            case "stuff":
                                stuff.Show();
                                Hide();
                                break;
                            case "student":
                                studentform.Show();
                                Hide();
                                break;
                        }
                        return;
                    }
                }
                MessageBox.Show("Данная кобинация логина и пароля не опознана!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Иначе сообщить пользователю, что он нехороший человек
            else
            {
                MessageBox.Show("Не все поля ввода заполнены корректно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Спросим о том, хочет ли пользователь выйти из программы
        private void Auth_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Вы уверены, что намереваетесь выйти из программы?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.No)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
    }

    // Класс пользователя (для обмена информацией у форм)
    static class User
    {
        // Поля - данные авторизации пользователя
        public static String username { get; set; }
        public static String password { get; set; }
        public static String type { get; set; }
    }
}

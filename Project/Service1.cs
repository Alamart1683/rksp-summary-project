using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Project
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде и файле конфигурации.
    public class Service1 : IService1
    {
        public String connection_string = "Data Source=DELAMART\\SQLEXP;Initial Catalog=accounting_for_tuition;Integrated Security=True";

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        // Получение списка всех пользователей для авторизации
        public DataEntityTier.AFTDataSet.usersDataTable GetAllUsers()
        {
            DataAccessTier.AFTDataSetTableAdapters.usersTableAdapter adapter = new DataAccessTier.AFTDataSetTableAdapters.usersTableAdapter();
            return adapter.GetAllUsers();
        }

        // Получение списка пользователей админом
        public DataEntityTier.AFTDataSet.usersDataTable AdminGetUsers()
        {
            DataAccessTier.AFTDataSetTableAdapters.usersTableAdapter adapter = new DataAccessTier.AFTDataSetTableAdapters.usersTableAdapter();
            return adapter.AdminGetUsers();
        }

        // Создание пользователя
        public void user_creating(String username, String password, String type)
        {
            // Соединение с сервером
            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                // Запрос на вызов хранимой процедуры создания пользователя
                using (SqlCommand create_user = new SqlCommand("dbo.user_creating", connection))
                {
                    // Укажем, что наша команда - вызов хранимой процедуры
                    create_user.CommandType = CommandType.StoredProcedure;
                    // Создание параметров для хранимой процедуры
                    create_user.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar, 50));
                    create_user.Parameters["@username"].Value = username;
                    create_user.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar, 20));
                    create_user.Parameters["@password"].Value = password;
                    create_user.Parameters.Add(new SqlParameter("@type", SqlDbType.VarChar, 20));
                    create_user.Parameters["@type"].Value = type;
                    // Попытка соединения с сервером
                    try
                    {
                        connection.Open();
                        create_user.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch { }
                }
            }
        }

        // Удаление пользователя
        public void user_removing(String username)
        {
            // Подключение к серверу БД
            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                // Выполнить хранимую процедуру удаления пользователя на сервере 
                using (SqlCommand remove_user = new SqlCommand("dbo.user_removing", connection))
                {
                    // Укажем, что наша команда - вызов хранимой процедуры
                    remove_user.CommandType = CommandType.StoredProcedure;
                    // Добавим входной параметр
                    remove_user.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar, 50));
                    remove_user.Parameters["@username"].Value = username;
                    // Пытаемся соединиться с сервером для осуществления удаления
                    try
                    {
                        connection.Open();
                        remove_user.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch { }
                }
            }
        }

        // Получить все образовательные контакты
        public DataEntityTier.AFTDataSet.Educational_contractDataTable GetEducational_Contracts()
        {
            DataAccessTier.AFTDataSetTableAdapters.Educational_contractTableAdapter adapter = new DataAccessTier.AFTDataSetTableAdapters.Educational_contractTableAdapter();
            return adapter.GetEducationalContracts();
        }

        // Получить всех студентов
        public DataEntityTier.AFTDataSet.StudentDataTable GetAllStudents()
        {
            DataAccessTier.AFTDataSetTableAdapters.StudentTableAdapter adapter = new DataAccessTier.AFTDataSetTableAdapters.StudentTableAdapter();
            return adapter.GetAllStudents();
        }

        // Полчить все цены
        public DataEntityTier.AFTDataSet.PriceDataTable GetAllPrices()
        {
            DataAccessTier.AFTDataSetTableAdapters.PriceTableAdapter adapter = new DataAccessTier.AFTDataSetTableAdapters.PriceTableAdapter();
            return adapter.GetAllPrices();
        }

        // Поднять цену указанной специальности на указанный процент
        public bool up_price(string Speciality, int Procent)
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                try
                {
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = new SqlCommand("dbo.price_upd", connection);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.Add(new SqlParameter("@speciality", SqlDbType.VarChar, 50));
                    da.SelectCommand.Parameters["@speciality"].Value = Speciality;
                    da.SelectCommand.Parameters.Add(new SqlParameter("@procent", SqlDbType.Int, 0));
                    da.SelectCommand.Parameters["@procent"].Value = Procent;
                    da.Fill(ds);
                    connection.Close();
                    return true;
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    return false;
                }
            }
        }

        // Определить, есть ли студент в списке оплативших все семестра
        public bool good_student(string Surname, string Groupe)
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand("dbo.good_student", connection);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.Fill(ds);
                connection.Close();
                ds.Tables[0].Rows[0].Table.PrimaryKey = new DataColumn[] { ds.Tables[0].Columns[0] };
                if (ds.Tables[0].Rows.Contains(Surname))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // Определить количество должников по указанной специальности
        public string bad_student(string Speciality)
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand("dbo.bad_student", connection);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.Fill(ds);
                connection.Close();
                ds.Tables[0].Rows.ToString();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row.ItemArray[0].ToString().Equals(Speciality))
                    {
                        return row.ItemArray[1].ToString();
                    }
                }
                return "";
            }
        }
    }
}


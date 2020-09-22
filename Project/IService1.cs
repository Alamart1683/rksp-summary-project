using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Project
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IService1" в коде и файле конфигурации.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Добавьте здесь операции служб

        [OperationContract]
        DataEntityTier.AFTDataSet.usersDataTable GetAllUsers();

        [OperationContract]
        DataEntityTier.AFTDataSet.usersDataTable AdminGetUsers();

        [OperationContract]
        void user_creating(String username, String password, String type);

        [OperationContract]
        void user_removing(String username);

        [OperationContract]
        DataEntityTier.AFTDataSet.Educational_contractDataTable GetEducational_Contracts();

        [OperationContract]
        DataEntityTier.AFTDataSet.StudentDataTable GetAllStudents();

        [OperationContract]
        DataEntityTier.AFTDataSet.PriceDataTable GetAllPrices();

        [OperationContract]
        bool up_price(string Speciality, int Procent);

        [OperationContract]
        bool good_student(string Surname, string Groupe);

        [OperationContract]
        string bad_student(string Speciality);
    }

    // Используйте контракт данных, как показано на следующем примере, чтобы добавить сложные типы к сервисным операциям.
    // В проект можно добавлять XSD-файлы. После построения проекта вы можете напрямую использовать в нем определенные типы данных с пространством имен "Project.ContractType".
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}

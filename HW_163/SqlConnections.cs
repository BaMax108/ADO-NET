using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace HW_163
{
    class SqlConnections
    {
        public enum DBType : byte
        {
            LocalDB,
            OleDB
        }

        /// <summary>
        /// Список пользователей
        /// </summary>
        public ObservableCollection<SqlUser> UsersList { get; set; }

        /// <summary>
        /// Список заказов выбранного пользователя
        /// </summary>
        public ObservableCollection<SqlData> CurrentChecklist { get; set; }

        /// <summary>
        /// Список всех заказов
        /// </summary>
        public List<SqlData> DataList { get; private set; }

        /// <summary>
        /// Список доступных продуктов
        /// </summary>
        public List<SqlProduct> ProductsList { get; private set; }

        private static string Path = $@"{Directory.GetCurrentDirectory()}\db";

        private SqlConnection ConnectionLocalDB = new SqlConnection(
            $@"Data Source = (LocalDB)\MSSQLLocalDB;
            AttachDbFilename={Path}\LocalDB.mdf;
            Integrated Security = true;
            User id = Admin;
            Password = qwerty;");
        private OleDbConnection ConnectionMSAccess = new OleDbConnection(
            $@" Provider = Microsoft.ACE.OLEDB.12.0;
            Data Source = {Path}\MSAccessDB.accdb;
            Persist Security Info=False;
            User Id = Admin;
            Jet OLEDB:Database Password = qwerty;");

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public SqlConnections()
        {
            Task t = Task.Run(() => {
                OpenConnection(ConnectionLocalDB);
                OpenConnection(ConnectionMSAccess);
            });
            t.Wait();

            t = Task.Run(() => {
                OleDBReader();
                LocalDBReader();
                CreateProductsList();
            });
            t.Wait();
        }

        /// <summary>
        /// Открытие подключения к локальной БД
        /// </summary>
        /// <param name="dbType"></param>
        static void OpenConnection(SqlConnection dbType)
        {
            try
            {
                dbType.Open();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Открытие подключения БД MS Access
        /// </summary>
        /// <param name="dbType"></param>
        private void OpenConnection(OleDbConnection dbType)
        {
            try
            {
                dbType.Open();
            }
            catch (OleDbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region Select
        /// <summary>
        /// Получение данных из локальной БД
        /// </summary>
        public void LocalDBReader()
        {
            DataList = new List<SqlData>();
            SqlCommand command = new SqlCommand("SELECT * FROM CheckList", ConnectionLocalDB);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                DataList.Add(new SqlData()
                {
                    ID = Convert.ToInt32(reader.GetValue(0)),
                    Email = reader.GetValue(1).ToString(),
                    ProductCode = Convert.ToInt16(reader.GetValue(2)),
                    ProductName = reader.GetValue(3).ToString()
                });
            }
            reader.Close();
        }

        /// <summary>
        /// Получение данных из БД MS Access
        /// </summary>
        private void OleDBReader()
        {
            UsersList = new ObservableCollection<SqlUser>();
            OleDbCommand command = new OleDbCommand("SELECT * FROM Users", ConnectionMSAccess);
            OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                UsersList.Add(new SqlUser()
                {
                    ID = Convert.ToInt32(reader.GetValue(0)),
                    SecondName = reader.GetValue(1).ToString(),
                    FirstName = reader.GetValue(2).ToString(),
                    LastName = reader.GetValue(3).ToString(),
                    PhoneNumber = reader.GetValue(4).ToString(),
                    Email = reader.GetValue(5).ToString()
                });
            }
            reader.Close();
        }

        /// <summary>
        /// Получение данных из списка доступных продуктов
        /// </summary>
        private void CreateProductsList()
        {
            ProductsList = new List<SqlProduct>();
            OleDbCommand command = new OleDbCommand("SELECT * FROM Products", ConnectionMSAccess);
            OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ProductsList.Add(new SqlProduct()
                {
                    ID = Convert.ToInt32(reader.GetValue(0)),
                    ProductCode = Convert.ToInt16(reader.GetValue(1)),
                    ProductName = reader.GetValue(2).ToString()
                });
            }
            reader.Close();
        }

        /// <summary>
        /// Создание списка продуктов по адресу электронной почты, выбранного пользователя
        /// </summary>
        /// <param name="value">Email пользователя</param>
        public void CreateChecklist(string value)
        {
            CurrentChecklist = new ObservableCollection<SqlData>();
            SqlCommand command = new SqlCommand(
                    $@"SELECT * FROM CheckList
                    WHERE Email = '{value}';", ConnectionLocalDB);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                CurrentChecklist.Add(new SqlData()
                {
                    ID = Convert.ToInt32(reader.GetValue(0)),
                    Email = reader.GetValue(1).ToString(),
                    ProductCode = (short)reader.GetValue(2),
                    ProductName = reader.GetValue(3).ToString()
                });
            };
            reader.Close();
        }
        #endregion

        #region Insert
        /// <summary>
        /// Создание новой записи в выбранной таблице (LocalDB)
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="currentRecord">Исходные данные</param>
        public void AddRecord(string tableName, SqlData currentRecord)
        {
            try
            {
                SqlCommand command = new SqlCommand($@"INSERT INTO {tableName} VALUES 
({currentRecord.ID},
N'{currentRecord.Email}',
{currentRecord.ProductCode},
N'{currentRecord.ProductName}');", ConnectionLocalDB);

                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            SqlData newRecord = new SqlData() {
                ID = currentRecord.ID,
                ProductCode = currentRecord.ProductCode,
                ProductName = currentRecord.ProductName,
                Email = currentRecord.Email
            };
            DataList.Add(newRecord);
            CurrentChecklist.Add(newRecord);
        }

        /// <summary>
        /// Создание новой записи в выбранной таблице (MS Access)
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="currentRecord">Исходные данные</param>
        public void AddRecord(string tableName, SqlUser currentRecord)
        {
            try
            {
                OleDbCommand command = new OleDbCommand($@"INSERT INTO {tableName} VALUES 
({currentRecord.ID},
'{currentRecord.SecondName}',
'{currentRecord.FirstName}',
'{currentRecord.LastName}',
'{currentRecord.PhoneNumber}',
'{currentRecord.Email}');", ConnectionMSAccess);

                command.ExecuteNonQuery();
            }
            catch (OleDbException ex)
            {
                Console.WriteLine(ex.Message);
            }

            UsersList.Add(new SqlUser()
            {
                ID = currentRecord.ID,
                SecondName = currentRecord.SecondName,
                FirstName = currentRecord.FirstName,
                LastName = currentRecord.LastName,
                PhoneNumber = currentRecord.PhoneNumber,
                Email = currentRecord.Email
            });
        }
        #endregion

        #region Update
        /// <summary>
        /// Изменение данных в записи пользователя
        /// </summary>
        /// <param name="currentRecord"></param>
        public void UpdateRecord(SqlUser currentRecord)
        {
            try
            {
                OleDbCommand command = new OleDbCommand(
$@"UPDATE Users SET 
[Фамилия] = '{currentRecord.LastName}', 
[Имя] = '{currentRecord.FirstName}', 
[Отчество] = '{currentRecord.SecondName}', 
[Номер телефона] = '{currentRecord.PhoneNumber}', 
[Email] = '{currentRecord.Email}'
WHERE Email = '{currentRecord.Email}';",ConnectionMSAccess);
                command.ExecuteNonQuery();
            }
            catch (OleDbException ex)
            {
                Console.WriteLine(ex.Message);
            }
            //OleDBReader();
        }
        /// <summary>
        /// Изменение данных в строке продукта
        /// </summary>
        /// <param name="currentRecord"></param>
        public void UpdateRecord(SqlData currentRecord)
        {
            try
            {
                SqlCommand command = new SqlCommand(
$@"UPDATE CheckList SET 
[Email] = N'{currentRecord.Email}', 
[Код товара] = {currentRecord.ProductCode}, 
[Наименование товара] = N'{currentRecord.ProductName}'
WHERE ID = {currentRecord.ID};", ConnectionLocalDB);
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            //OleDBReader();
        }
        #endregion

        #region Delete

        /// <summary>
        /// Удаление записи по входным данным
        /// </summary>
        /// <param name="value">Значение, которое требуется найти</param>
        /// <param name="db">Тип базы данных</param>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="column">Колонка, по которой требуется найти запись</param>
        public void DeleteRecord(DBType db, string tableName, string column, dynamic value)
        {
            switch (db)
            {
                case DBType.OleDB:
                    try
                    {
                        OleDbCommand command = new OleDbCommand(
                            $@"DELETE FROM {tableName} WHERE {column} = '{value}';", ConnectionMSAccess);
                        command.ExecuteNonQuery();
                    }
                    catch (OleDbException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    for (int i = 0; i < UsersList.Count; i++)
                    {
                        if (UsersList[i].Email == value)
                            UsersList.Remove(UsersList[i]);
                    }
                    break;
                case DBType.LocalDB:
                    try
                    {
                        SqlCommand command = new SqlCommand(
                            $@"DELETE FROM {tableName} WHERE {column} = '{value}';", ConnectionLocalDB);
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    if (column == "ID")
                    {
                        for (int i = 0; i < DataList.Count; i++)
                        {
                            if (DataList[i].ID == value)
                                DataList.Remove(DataList[i]);
                        }
                        for (int i = 0; i < CurrentChecklist.Count; i++)
                        {
                            if (CurrentChecklist[i].ID == value)
                                CurrentChecklist.Remove(CurrentChecklist[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < DataList.Count; i++)
                        {
                            if (DataList[i].Email == value)
                            { DataList.Remove(DataList[i]); i--; }
                        }
                        CurrentChecklist.Clear();
                    }
                    break;
            }
        }
        #endregion
    }
}

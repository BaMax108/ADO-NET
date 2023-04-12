using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static HW_163.SqlConnections;

namespace HW_163
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnections sql = new SqlConnections();
        SqlUser CurrentUser;
        SqlData CurrentData;

        public MainWindow()
        {
            InitializeComponent();
            dgUsers.ItemsSource = sql.UsersList;
        }

        /// <summary>
        /// Триггер выбора ячейки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                object[] cells = ((object[])e.AddedItems).ToArray<object>();
                if (cells.Length == 0) return;

                SqlUser currentCell = (SqlUser)cells.GetValue(0);
                CurrentUser = currentCell;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            sql.CreateChecklist(CurrentUser.Email);
            dgData.ItemsSource = sql.CurrentChecklist;
        }

        /// <summary>
        /// Триггер выбора ячейки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                object[] cells = ((object[])e.AddedItems).ToArray<object>();
                if (cells.Length == 0) return;

                SqlData currentCell = (SqlData)cells.GetValue(0);
                CurrentData = currentCell;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region Insert
        /// <summary>
        /// Определение первого свободного ID
        /// </summary>
        /// <param name="db"></param>
        private void SetID(DBType db)
        {
            switch (db)
            {
                case DBType.OleDB:
                    CurrentUser.ID = 1;
                    List<SqlUser> list = (sql.UsersList.OrderBy(user => user.ID)).ToList();
                    for (int i = 0; i < sql.UsersList.Count; i++)
                    {
                        if (list[i].ID == CurrentUser.ID) CurrentUser.ID++;
                        else break;
                    }
                    break;
                case DBType.LocalDB:
                    CurrentData.ID = 1;
                    List<SqlData> dataList = (sql.DataList.OrderBy(data => data.ID)).ToList();
                    for (int i = 0; i < sql.DataList.Count; i++)
                    {
                        if (dataList[i].ID == CurrentData.ID) CurrentData.ID++;
                        else break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Добавление нового пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser = new SqlUser();
            SetID(DBType.OleDB);

            NewUser wnd = new NewUser(ref CurrentUser);
            wnd.ShowDialog();
            if (CurrentUser.Email == "" | CurrentUser.FirstName == "" | CurrentUser.LastName == "" |
                CurrentUser.SecondName == "")
            {
                CurrentUser = null;
                return;
            }
            sql.AddRecord("Users", CurrentUser);
            CurrentUser = null;
        }

        /// <summary>
        /// Добавление нового заказа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddData_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser == null) return;
            CurrentData = new SqlData();
            SetID(DBType.LocalDB);

            CurrentData.Email = CurrentUser.Email;
            NewData wnd = new NewData(ref CurrentData, sql.ProductsList);
            wnd.ShowDialog();
            if (CurrentData.ProductCode == 0)
            {
                CurrentData = null;
                return;
            }

            sql.AddRecord("CheckList", CurrentData);
            CurrentData = null;
        }
        #endregion

        #region Update
        /// <summary>
        /// Изменение личных данных выбранного пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpdateUser_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser == null) return;
            NewUser wnd = new NewUser(ref CurrentUser);
            wnd.ShowDialog();
            sql.UpdateRecord(CurrentUser);
        }

        /// <summary>
        /// Изменение существующего заказа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpdateData_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentData == null) return;
            NewData wnd = new NewData(ref CurrentData, sql.ProductsList);
            wnd.ShowDialog();
            sql.UpdateRecord(CurrentData);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Удаление выбранного пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser == null) return;

            sql.DeleteRecord(DBType.OleDB, "Users", "Email", CurrentUser.Email);
            sql.DeleteRecord(DBType.LocalDB, "CheckList", "Email", CurrentUser.Email);

            CurrentUser = null;
        }

        /// <summary>
        /// Удаление выбранного заказа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeleteData_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentData == null) return;
            sql.DeleteRecord(DBType.LocalDB, "CheckList", "ID", CurrentData.ID);
            CurrentData = null;
        }
        #endregion
    }
}

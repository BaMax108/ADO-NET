using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace HW_163
{
    /// <summary>
    /// Логика взаимодействия для NewData.xaml
    /// </summary>
    public partial class NewData : Window
    {
        private SqlData Data;

        public NewData(ref SqlData data, List<SqlProduct> ProductsList)
        {
            Data = data;
            InitializeComponent();
            cbProducts.ItemsSource = ProductsList;
            if (data != null)
            {
                cbProducts.Text = $"";
                foreach (SqlProduct s in ProductsList)
                {
                    if (data.ProductCode == s.ProductCode)
                    {
                        cbProducts.Text = s.ToString();
                        break;
                    }
                }
            }
            cbProducts.SelectionChanged += new SelectionChangedEventHandler(CbSelectionChanged);
        }

        private void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SqlProduct product = (SqlProduct)((object[])e.AddedItems).GetValue(0);
            Data.ProductCode = Convert.ToInt16(product.ProductCode);
            Data.ProductName = product.ProductName;
            this.Close();
        }
    }
}

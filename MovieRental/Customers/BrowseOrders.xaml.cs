﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovieRental.Customers
{
    /// <summary>
    /// Interaction logic for BrowseOrders.xaml
    /// </summary>
    public partial class BrowseOrders : Page
    {
        Customer customer;

        public BrowseOrders(Customer customer)
        {
            InitializeComponent();
            this.customer = customer;

            var gridView = new GridView();
            this.OrderList.View = gridView;

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "OrderID",
                DisplayMemberBinding = new Binding("OrderID")
            });

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Movie",
                DisplayMemberBinding = new Binding("Title")
            });

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Rental Date",
                DisplayMemberBinding = new Binding("Date")
            });

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Expected Return",
                DisplayMemberBinding = new Binding("Expected")
            });

            using (var context = new MovieRentalEntities())
            {
                var orders = context.Orders.Include("Movie").Where(o => o.AccountNumber == customer.AccountNumber && o.RentalDate != null).ToList();

                foreach (Order order in orders)
                {
                    OrderList.Items.Add(new OrderView { OrderID = order.OrderID, Title = order.Movie.Title,
                        Date = order.RentalDate.ToString(), Expected = order.ExpectedReturn.ToString() });
                }
            }
        }

        private void OrderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

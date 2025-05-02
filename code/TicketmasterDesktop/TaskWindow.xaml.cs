using System;
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
using System.Windows.Shapes;
using Ticketmaster.Models;

namespace TicketmasterDesktop
{
    public partial class TaskWindow : Window
    {
        private readonly Board _board;

        public TaskWindow(Board board)
        {
            InitializeComponent();
            _board = board;
            LoadBoardData();
        }

        private void LoadBoardData()
        {
            // Clear existing content
            StagesPanel.Children.Clear();

            foreach (var stage in _board.Stages.OrderBy(s => s.Position))
            {
                var stageGroup = new GroupBox
                {
                    Header = stage.StageTitle,
                    Margin = new Thickness(10),
                    Content = new ListBox
                    {
                        ItemsSource = stage.Tasks,
                        DisplayMemberPath = "TaskTitle",
                        Tag = stage // Store stage for reference
                    }
                };

                StagesPanel.Children.Add(stageGroup);
            }
        }

        private void BackToProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            var projectList = new ProjectListWindow(Session.CurrentUser.Id);
            projectList.Show();
            this.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}

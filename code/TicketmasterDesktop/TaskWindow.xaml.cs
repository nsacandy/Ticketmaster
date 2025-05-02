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
using Ticketmaster.Data;
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

            using var context = new TicketmasterContext(App.DbOptions);

            foreach (var stage in _board.Stages.OrderBy(s => s.Position))
            {
                var tasks = context.TaskItem
                    .Where(t => t.StageId == stage.StageId &&
                                (!t.AssignedTo.HasValue || t.AssignedTo == Session.CurrentUser.Id))
                    .ToList();

                var taskPanel = new StackPanel();

                foreach (var task in tasks)
                {
                    string assignedName = "Unassigned";
                    if (task.AssignedTo.HasValue)
                    {
                        var employee = context.Employee.FirstOrDefault(e => e.Id == task.AssignedTo.Value);
                        if (employee != null)
                        {
                            assignedName = employee.FirstName + " " + employee.LastName;
                        }
                    }

                    var taskRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
                    taskRow.Children.Add(new TextBlock
                    {
                        Text = $"{task.Title}: {task.Description} (Assigned to: {assignedName})",
                        Width = 400,
                        VerticalAlignment = VerticalAlignment.Center
                    });

                    var assignButton = new Button
                    {
                        Content = task.AssignedTo == Session.CurrentUser?.Id ? "Unassign" : "Assign",
                        Tag = task,
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    assignButton.Click += AssignOrUnassign_Click;
                    taskRow.Children.Add(assignButton);

                    taskPanel.Children.Add(taskRow);
                }

                if (taskPanel.Children.Count > 0)
                {
                    var stageGroup = new GroupBox
                    {
                        Header = stage.StageTitle,
                        Margin = new Thickness(10),
                        Content = taskPanel
                    };

                    StagesPanel.Children.Add(stageGroup);
                }
            }
        }

        private void AssignOrUnassign_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TaskItem task)
            {
                using var context = new TicketmasterContext(App.DbOptions);
                var dbTask = context.TaskItem.FirstOrDefault(t => t.TaskItemId == task.TaskItemId);
                if (dbTask != null)
                {
                    if (dbTask.AssignedTo == Session.CurrentUser?.Id)
                    {
                        dbTask.AssignedTo = null;
                    }
                    else
                    {
                        dbTask.AssignedTo = Session.CurrentUser?.Id;
                    }
                    context.SaveChanges();
                    LoadBoardData(); // Refresh UI
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void BackToProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            var projectList = new ProjectListWindow(Session.CurrentUser.Id);
            projectList.Show();
            this.Close();
        }
    }
}







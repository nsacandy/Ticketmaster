using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ticketmaster.Data;
using Ticketmaster.Models; // Assuming Board, Stage, Task models are here

namespace TicketmasterDesktop
{
    public partial class TaskWindow : Window
    {
        public TaskWindow()
        {
            InitializeComponent();
            this.Width = 400;
            SetupFilterComboBox();
            LoadBoardData();
        }

        private void SetupFilterComboBox()
        {
            var filterBox = new ComboBox
            {
                Name = "FilterComboBox",
                Margin = new Thickness(10),
                Width = 200
            };

            filterBox.Items.Add("All");
            filterBox.Items.Add("Assigned");
            filterBox.Items.Add("Unassigned");
            filterBox.SelectedIndex = 0;
            filterBox.SelectionChanged += (s, e) => LoadBoardData();

            HeaderPanel.Children.Add(filterBox);
        }

        private void LoadBoardData()
        {
            StagesPanel.Children.Clear();

            using var context = new TicketmasterContext(App.DbOptions);

            var filter = (HeaderPanel.Children.OfType<ComboBox>().FirstOrDefault(cb => cb.Name == "FilterComboBox")?.SelectedItem as string) ?? "All";

            var query = context.TaskItem.AsQueryable();

            if (filter == "Assigned")
                query = query.Where(t => t.AssignedTo == Session.CurrentUser.Id);
            else if (filter == "Unassigned")
                query = query.Where(t => !t.AssignedTo.HasValue);
            else
                query = query.Where(t => !t.AssignedTo.HasValue || t.AssignedTo == Session.CurrentUser.Id);

            var allTasks = query.ToList();
            var stages = context.Stage.Include(s => s.ParentBoard).ToList();
            var projects = context.Project.ToDictionary(p => p.ProjectId, p => p.ProjectName);

            var stagesByBoard = stages.GroupBy(s => s.ParentBoardId)
                .ToDictionary(g => g.Key, g => g.ToList());


            foreach (var task in allTasks)
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

                string projectTitle = "Unknown Project";
                var stage = stages.FirstOrDefault(s => s.StageId == task.StageId);
                if (stage != null && projects.TryGetValue(stage.ParentBoardId, out string title))
                {
                    projectTitle = title;
                }

                var taskRow = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(10),
                    Background = System.Windows.Media.Brushes.LightGray
                };
                taskRow.Children.Add(new TextBlock
                {
                    Text = task.Title + "\nProject: " + projectTitle + "\nDescription: " + task.Description +
                           "\nAssigned to: " + assignedName + "\nStage: " + task.Stage.StageTitle,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(5)
                });

                var assignButton = new Button
                {
                    Content = task.AssignedTo == Session.CurrentUser?.Id ? "Unassign" : "Assign",
                    Tag = task,
                    Margin = new Thickness(5, 0, 5, 10)
                };
                assignButton.Click += AssignOrUnassign_Click;
                taskRow.Children.Add(assignButton);

                if (stages.FirstOrDefault(s => s.StageId == task.StageId) is Stage currentStage &&
                    stagesByBoard.TryGetValue(currentStage.ParentBoardId, out var stageList))
                {
                    var stageDropdown = new ComboBox
                    {
                        Width = 200,
                        Margin = new Thickness(5),
                        DisplayMemberPath = "StageTitle",
                        SelectedValuePath = "StageId",
                        ItemsSource = stageList,
                        SelectedValue = task.StageId,
                        Tag = task
                    };

                    var moveButton = new Button
                    {
                        Content = "Move",
                        Margin = new Thickness(5, 0, 5, 10),
                        Tag = stageDropdown
                    };
                    moveButton.Click += MoveTask_Click;
                    taskRow.Children.Add(moveButton);
                    taskRow.Children.Add(stageDropdown);
                }

                StagesPanel.Children.Add(taskRow);
                
            }
        }

        private void MoveTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button moveButton &&
                moveButton.Tag is ComboBox dropdown &&
                dropdown.Tag is TaskItem task &&
                dropdown.SelectedValue is int newStageId)
            {
                using var context = new TicketmasterContext(App.DbOptions);
                var dbTask = context.TaskItem.FirstOrDefault(t => t.TaskItemId == task.TaskItemId);
                if (dbTask != null)
                {
                    if (dbTask.StageId == newStageId)
                    {
                        MessageBox.Show("Task is already in the selected stage.", "No Change", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    dbTask.StageId = newStageId;
                    context.SaveChanges();
                    MessageBox.Show("Task moved to new stage!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBoardData();
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
                    LoadBoardData();
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
    }
}

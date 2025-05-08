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
using Microsoft.EntityFrameworkCore;

namespace TicketmasterDesktop
{
    /// <summary>
    /// Interaction logic for ProjectListWindow.xaml
    /// </summary>
    public partial class ProjectListWindow : Window
    {
        private readonly int _userId;

        public ProjectListWindow(int id)
        {
            InitializeComponent();
            _userId = id;

            WelcomeTextBlock.Text = $"Welcome, {Session.CurrentUser.FirstName} {Session.CurrentUser.LastName}!";

            LoadProjectsForEmployee();
        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private async void LoadProjectsForEmployee()
        {
            var employee = Session.CurrentUser;
            if (employee == null)
            {
                MessageBox.Show("No employee logged in.");
                return;
            }

            int employeeId = employee.Id;

            // Get all groups from DB
            var allGroups = await App.DbContext.Groups
                .Where(g => !string.IsNullOrWhiteSpace(g.EmployeeIds) || g.ManagerId != null)
                .ToListAsync();

            // Identify groups where user is either a member or a manager
            var matchingGroupIds = allGroups
                .Where(g =>
                    (g.EmployeeIds?.Split(',').Any(id => int.TryParse(id.Trim(), out var parsedId) && parsedId == employeeId) ?? false)
                    || g.ManagerId == employeeId)
                .Select(g => g.GroupId)
                .ToList();

            // Get all projects with involved groups
            var allProjects = await App.DbContext.Project
                .Where(p => !string.IsNullOrWhiteSpace(p.InvolvedGroups))
                .ToListAsync();

            // Filter projects where any involved group matches one of the matching groups
            var matchingProjects = allProjects
                .Where(p =>
                    p.InvolvedGroups.Split(',')
                    .Any(id => int.TryParse(id.Trim(), out var parsed) && matchingGroupIds.Contains(parsed)))
                .ToList();

            ProjectsListView.ItemsSource = matchingProjects;
        }


        private void OpenProject(Project selectedProject)
        {
            if (selectedProject == null)
            {
                MessageBox.Show("Please select a project.");
                return;
            }

            // Load board for the project
            using var context = new TicketmasterContext(App.DbOptions);
            var board = context.Board
                .Include(b => b.Stages)
                .ThenInclude(s => s.Tasks)
                .FirstOrDefault(b => b.ParentProjectId == selectedProject.ProjectId);

            if (board == null)
            {
                MessageBox.Show("No board found for this project.");
                return;
            }

            var taskWindow = new TaskWindow(); // hypothetical window
            taskWindow.Show();
            this.Close();
        }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            var selectedProject = ProjectsListView.SelectedItem as Project;
            OpenProject(selectedProject);
        }

        private void ProjectsListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedProject = ProjectsListView.SelectedItem as Project;
            OpenProject(selectedProject);
        }

    }
}

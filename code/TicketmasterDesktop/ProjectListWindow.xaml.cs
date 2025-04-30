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

namespace TicketmasterDesktop
{
    /// <summary>
    /// Interaction logic for ProjectListWindow.xaml
    /// </summary>
    public partial class ProjectListWindow : Window
    {
        private readonly string _username;

        public ProjectListWindow(string username)
        {
            InitializeComponent();
            _username = username;

            LoadProjects();
        }

        private void LoadProjects()
        {
            // Example: In real life, you'd query your real database.
            // Here is fake hardcoded data:
            var fakeProjects = new List<Project>
            {
                new Project { ProjectName = "Website Redesign", Description = "Updating the company website" },
                new Project { ProjectName = "Mobile App", Description = "Building an app for customers" },
                new Project { ProjectName = "Internal Tools", Description = "Improving internal systems" }
            };

            // TODO: If real, you'd filter based on the _username.
            ProjectsListView.ItemsSource = fakeProjects;
        }
    }

    // Simple "Project" class just for demo purposes
    public class Project
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
    }
}

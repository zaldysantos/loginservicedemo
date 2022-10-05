using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net.Http;
using System.Windows;

namespace DeskApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly LoginDialog loginWindow = new LoginDialog();

        public string _authenticationCode;
        public string AuthenticationCode
        {
            get { return _authenticationCode; } 
            set { _authenticationCode = value; }
        }

        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// draw main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// main window was loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Hide(); // hide main window
            Title = typeof(MainWindow).Assembly.GetName().Name;
            
            loginWindow.Title = $"{Title} Login";
            loginWindow.Closing += Window_Closing; // handle login window closing event
            loginWindow.button.Click += LoginButton_Click; // handle login button click event
            loginWindow.textBox.Focus();
            loginWindow.ShowDialog(); // show login window
        }

        /// <summary>
        /// main window is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object? sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// login button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // serialize payload as json
            var payload = JsonSerializer.Serialize(
                new
                {
                    Username = loginWindow.textBox.Text,
                    Password = loginWindow.passwordBox.Password
                }
            );
            try // consume /api/user/login
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7058/api/user/login");
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                var result = await new HttpClient().SendAsync(request);
                result.EnsureSuccessStatusCode();
                // receive response as json
                var content = await result.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<JsonNode>(content, jsonSerializerOptions);
                // validate response
                if ((bool)response["success"]) 
                {
                    MessageBox.Show("You are now logged-in.", request.RequestUri.ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                    AuthenticationCode = (string)response["data"]; // store authentication code 
                    loggedInAs.Content = $"Logged-in as {loginWindow.textBox.Text}";
                    statusText.Content = "Ready.";
                    loginWindow.Hide(); // hide login window
                    Show(); // show main window
                    getAllUsers();
                }
                else // user login failed
                {
                    MessageBox.Show((string)response["message"], request.RequestUri.ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                    loginWindow.passwordBox.Focus();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, loginWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// /api/user/getAll
        /// </summary>
        async void getAllUsers()
        {
            try // consume /api/user/getAll
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7058/api/user/getAll");
                request.Headers.Add("authenticationCode", AuthenticationCode);
                var result = await new HttpClient().SendAsync(request);
                result.EnsureSuccessStatusCode();
                // receive response as json
                var content = await result.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<JsonNode>(content, jsonSerializerOptions);
                // validate response
                if ((bool)response["success"])
                {
                    // get response data
                    var data = JsonSerializer.Deserialize<JsonNode>(response["data"].ToString());
                    // push response data to a list
                    var users = new List<object>();
                    foreach (var node in data.AsArray()) 
                    {
                        var user = new
                        {
                            User = (string)node["username"],
                            Name = (string)node["name"],
                            DOB = ((DateTime)node["dob"]).ToShortDateString(),
                            Gender = (string)node["gender"],
                            Address = (string)node["address"],
                            Email = (string)node["email"],
                            Phone = (string)node["phone"],
                        };
                        users.Add(user);
                    }
                    // bind the list to grid
                    dataGrid.DataContext = users; 
                }
                else // user not authenticated
                {
                    MessageBox.Show((string)response["message"], request.RequestUri.ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, loginWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

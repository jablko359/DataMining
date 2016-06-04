using System.Windows;

namespace AGDSPresentationDB.Services
{
    public class WindowService : IWindowService
    {
        public Window ShowWindow<T>(object dataContext) where T : Window, new()
        {
            Window childWindow = new T();
            childWindow.DataContext = dataContext;
            ShowWindow(childWindow);
            return childWindow;
        }

        public Window ShowWindow<T>() where T : Window, new()
        {
            Window childWindow = new T();
            ShowWindow(childWindow);
            return childWindow;
        }

        private void ShowWindow(Window window)
        {
            if (window is IDialogWindow)
            {
                window.ShowDialog();
            }
            else
            {
                window.Show();
            }
        }
    }

    public interface IWindowService
    {
        Window ShowWindow<T>(object dataContext) where T : Window, new();
        Window ShowWindow<T>() where T : Window, new();
    }

    public interface IDialogWindow { }
}

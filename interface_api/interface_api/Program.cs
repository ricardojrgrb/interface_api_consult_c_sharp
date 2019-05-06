using System;
using Gtk;
using interface_api.Properties;

namespace interface_api
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            MainWindow win = new MainWindow();
            win.Show();
            Application.Run();
            Gtk.Application.Init();
            NodeViewExample node = new NodeViewExample();
            node.Show();
            Gtk.Application.Run();
        }
    }
}

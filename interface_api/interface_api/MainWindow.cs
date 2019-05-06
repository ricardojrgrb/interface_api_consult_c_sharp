using Gtk;
using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

using static Newtonsoft.Json.JsonConvert;

public partial class MainWindow : Gtk.Window
{
    string URI = "";
    int codeUser = 1;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        SetSizeRequest(500, 500);

        Gtk.NodeView view = new Gtk.NodeView(Store);
        Add(view);

        view.AppendColumn("Id", new Gtk.CellRendererText(), "text", 0);
        view.AppendColumn("Motivo", new Gtk.CellRendererText(), "text", 1);
        view.AppendColumn("Valor", new Gtk.CellRendererText(), "text", 1);
        view.AppendColumn("Data", new Gtk.CellRendererText(), "text", 1);
        view.ShowAll();

        Build();
    }

    protected override bool OnDeleteEvent(Gdk.Event ev)
    {
        Gtk.Application.Quit();
        return true;
    }

    Gtk.NodeStore store;
    Gtk.NodeStore Store
    {
        get
        {
            if (store == null)
            {
                store = new Gtk.NodeStore(typeof(User));
            }
            return store;
        }
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    [Gtk.TreeNode(ListOnly = true)]
    public class User : Gtk.TreeNode
    {
        [Gtk.TreeNodeValue(Column = 0)]
        public int Id { get; set; }
        [Gtk.TreeNodeValue(Column = 1)]
        public string Reason { get; set; }
        [Gtk.TreeNodeValue(Column = 2)]
        public decimal Value { get; set; }
        [Gtk.TreeNodeValue(Column = 3)]
        public string Date { get; set; }
    }

    private async void GetAllUsers()
    {
        URI = textURI.Text;
        using (var client = new HttpClient())
        {
            using (var response = await client.GetAsync(URI))
            {
                if (response.IsSuccessStatusCode)
                {
                    var UserJsonString = await response.Content.ReadAsStringAsync();
                    dataNodeView.DataSource = JsonConvert.DeserializeObject<User[]>(UserJsonString).ToList();
                }
                else
                {
                    Console.WriteLine("Não foi possível obter o cliente : " + response.StatusCode);
                }
            }
        }
    }

    private async void AddUser()
    {
        URI = textURI.Text;
        User user = new User();

        using (var client = new HttpClient())
        {
            var serializedUser = SerializeObject(user);
            var content = new StringContent(serializedUser, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(URI, content);
        }
        GetAllUsers();
    }

    private async void UpdateUser(int codUser)
    {
        URI = textURI.Text;
        User user = new User();

        using (var client = new HttpClient())
        {
            HttpResponseMessage responseMessage = await client.PutAsJsonAsync(URI + "/" + user.Id, user);
            if (responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("Cliente atualizado");
            }
            else
            {
                Console.WriteLine("Falha ao atualizar o cliente : " + responseMessage.StatusCode);
            }
        }
        GetAllUsers();
    }

    private async void DeleteUser(int codUser)
    {
        URI = textURI.Text;
        int UserID = codUser;

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(URI);
            HttpResponseMessage responseMessage = await client.DeleteAsync(String.Format("{0}/{1}", URI, UserID));
            if (responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("Cliente excluído com sucesso");
            }
            else
            {
                Console.WriteLine("Falha ao excluir o cliente  : " + responseMessage.StatusCode);
            }
        }
        GetAllUsers();
    }

    private void InputBox()
    {
        string Prompt = "Informe o código do Cliente.";
        string Titulo = "Cadastro - Atualização - Busca - Remoção";
        string Resultado = Microsoft.VisualBasic.Interaction.InputBox(Prompt, Titulo, "9", 600, 350);

        if (Resultado != "")
        {
            codeUser = Convert.ToInt32(Resultado);
        }
        else
        {
            codeUser = -1;
        }
    }

    private void btnAddUser_Click(object sender, EventArgs e)
    {
        AddUser();
    }

    private void btnShowUsers_Click(object sender, EventArgs e)
    {
        GetAllUsers();
    }

    private void btnUpdUser_Click(object sender, EventArgs e)
    {
        InputBox();
        if (codeUser != -1)
        {
            UpdateUser(codeUser);
        }
    }

    private void btnDeleteUser_Click(object sender, EventArgs e)
    {
        InputBox();
        if (codeUser != -1)
        {
            DeleteUser(codeUser);
        }
    }
}

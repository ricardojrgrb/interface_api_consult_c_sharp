using System;
using System.Collections.Generic;
using System.Net.Http;

namespace interface_api.Properties
{
    public class ConsuAPI
    {
        HttpClient client;
        Uri userName;

        public ConsuAPI()
        {
            if (client == null)
            {
                client = new HttpClient();
                client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        private void getAll()
        {
            System.Net.Http.HttpResponseMessage response = client.GetAsync("/users").Result;

            if (response.IsSuccessStatusCode)
            {
                userName = response.Headers.Location;

                var usuarios = response.Content.ReadAsAsync<IEnumerable<Usuario>>().Result;

                comboUserName.DataSource = usuarios;
                comboUserName.DataBind();
            }

            else
                Response.Write(response.StatusCode.ToString() + " - " + response.ReasonPhrase);
        }

    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Models.Purchases;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Models.Utils;
using Newtonsoft.Json;
using Xunit;

namespace Aries.WebAPI.Tests
{
    [CollectionDefinition("http-contract", DisableParallelization = true)]
    public class HttpContractCollection : ICollectionFixture<AriesApiFactory>
    {
    }

    [Collection("http-contract")]
    public class HttpContractTests
    {
        private readonly AriesApiFactory _factory;

        public HttpContractTests(AriesApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Auth_login_returns_jwt_and_user_newtonsoft_can_read()
        {
            var json = await LoginRaw("kenneth", "96321");
            Assert.Equal(HttpStatusCode.OK, json.StatusCode);

            var token = JsonConvert.DeserializeObject<WebToken>(json.Body);
            Assert.NotNull(token.User);
            Assert.Equal("kenneth", token.User.UserName);
            Assert.False(string.IsNullOrWhiteSpace(token.Token));
            Assert.Equal(3, token.Token.Split('.').Length);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Token);
            Assert.Equal("7", jwt.Claims.Single(c => c.Type == "UserId").Value);
            Assert.Equal("7", jwt.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Assert.True(string.IsNullOrEmpty(token.User.Password));
            Assert.DoesNotContain("\"Password\"", json.Body);
            Assert.DoesNotContain("96321", json.Body);
        }

        [Fact]
        public async Task Auth_login_rejects_wrong_password()
        {
            var json = await LoginRaw("kenneth", "nope");
            Assert.Equal(HttpStatusCode.Unauthorized, json.StatusCode);
        }

        [Fact]
        public async Task Company_getAll_requires_bearer()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/company/getAll");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Auth_and_company_routes_match_desktop_contract()
        {
            var client = await ClientWithToken();

            var companiesResponse = await client.GetAsync("/company/getAll");
            companiesResponse.EnsureSuccessStatusCode();
            var companies = JsonConvert.DeserializeObject<List<Company>>(
                await companiesResponse.Content.ReadAsStringAsync());
            Assert.Contains(companies, c => c.Code == "C001");

            var codeResponse = await client.GetAsync("/company/BuildCode");
            codeResponse.EnsureSuccessStatusCode();
            var next = JsonConvert.DeserializeObject<Company>(
                await codeResponse.Content.ReadAsStringAsync());
            Assert.Equal("C002", next.Code);

            var deleteResponse = await client.DeleteAsync("/company/delete/C001");
            deleteResponse.EnsureSuccessStatusCode();
            Assert.Equal("C001", _factory.Admin.LastDeletedCode);
        }

        [Fact]
        public async Task Company_create_sets_createdBy_from_jwt()
        {
            var client = await ClientWithToken();
            var payload = JsonConvert.SerializeObject(new Company
            {
                NumberId = "3-101-123456",
                CompanyName = "Nueva",
                Mail = "a@b.com"
            });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/company/Create", content);
            response.EnsureSuccessStatusCode();
            Assert.Equal(7, _factory.Admin.LastCreated.CreatedBy);
        }

        [Fact]
        public async Task Company_get_by_code_and_update_match_desktop()
        {
            var client = await ClientWithToken();

            var found = await client.GetAsync("/company/C001");
            found.EnsureSuccessStatusCode();
            var company = JsonConvert.DeserializeObject<Company>(await found.Content.ReadAsStringAsync());
            Assert.Equal("C001", company.Code);

            company.CompanyName = "Renamed";
            var payload = JsonConvert.SerializeObject(company);
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var update = await client.PostAsync("/company/Update", content);
            update.EnsureSuccessStatusCode();
            Assert.Equal("Renamed", _factory.Admin.LastUpdated.CompanyName);
            Assert.Equal(7, _factory.Admin.LastUpdated.UpdatedBy);
        }

        [Fact]
        public async Task JournalEntry_create_returns_int_id()
        {
            var client = await ClientWithToken();
            var payload = JsonConvert.SerializeObject(new JournalEntry { Number = 1, PostingPeriodId = 4 });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/JournalEntry/CreateJournalEntry", content);
            response.EnsureSuccessStatusCode();
            var id = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
            Assert.Equal(42, id);
            Assert.Equal(7, _factory.Financial.LastCreatedEntry.CreatedBy);
        }

        [Fact]
        public async Task SalesRegister_requires_bearer()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/salesRegister/byCompany/C001");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task SalesRegister_create_and_caja_estado()
        {
            var client = await ClientWithToken();
            var payload = JsonConvert.SerializeObject(new SalesRegister
            {
                CompanyId = "C001",
                Code = "Caja1",
                Name = "Mostrador"
            });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var created = await client.PostAsync("/salesRegister/Create", content);
            created.EnsureSuccessStatusCode();
            var register = JsonConvert.DeserializeObject<SalesRegister>(
                await created.Content.ReadAsStringAsync());
            Assert.Equal(1, register.Id);
            Assert.Equal(7, register.CreatedBy);

            var estado = await client.GetAsync($"/caja/estado/{register.Id}");
            estado.EnsureSuccessStatusCode();
            var status = JsonConvert.DeserializeObject<CashRegisterStatus>(
                await estado.Content.ReadAsStringAsync());
            Assert.False(status.Abierta);
        }

        [Fact]
        public async Task Supplier_requires_bearer()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/supplier/GetAll/C001");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Supplier_create_sets_createdBy_from_jwt()
        {
            var client = await ClientWithToken();
            var payload = JsonConvert.SerializeObject(new Supplier
            {
                CompanyId = "C001",
                Name = "Distribuidora Sol",
                IdType = IdType.CEDULA_JURIDICA,
                NumberId = "3-101-123456"
            });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var created = await client.PostAsync("/supplier/Create", content);
            created.EnsureSuccessStatusCode();
            var supplier = JsonConvert.DeserializeObject<Supplier>(
                await created.Content.ReadAsStringAsync());
            Assert.Equal(1, supplier.Id);
            Assert.Equal(7, supplier.CreatedBy);
            Assert.Equal("Distribuidora Sol", supplier.Name);
        }

        private async Task<HttpClient> ClientWithToken()
        {
            var client = _factory.CreateClient();
            var login = JsonConvert.DeserializeObject<WebToken>((await LoginRaw("kenneth", "96321")).Body);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Token);
            return client;
        }

        private async Task<(HttpStatusCode StatusCode, string Body)> LoginRaw(string user, string password)
        {
            var client = _factory.CreateClient();
            var payload = JsonConvert.SerializeObject(new Login { UserId = user, Password = password });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/auth/login", content);
            var body = await response.Content.ReadAsStringAsync();
            return (response.StatusCode, body);
        }
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.Users;
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

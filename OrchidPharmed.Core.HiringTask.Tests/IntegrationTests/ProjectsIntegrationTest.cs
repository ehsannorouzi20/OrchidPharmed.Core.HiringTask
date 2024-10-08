using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrchidPharmed.Core.HiringTask.Tests.IntegrationTests
{
    public class ProjectsIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public ProjectsIntegrationTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }
        //[Fact]
        public async Task GetAllProjects_ReturnsListOfProjects()
        {
            var response = await _client.GetAsync("/api/Project");

            response.EnsureSuccessStatusCode();
            var output = JsonSerializer.Deserialize<API.Structure.APIResponse>(await response.Content.ReadAsStringAsync());
            Assert.NotNull(output);
            Assert.IsAssignableFrom<API.Structure.DTO.ProjectDTO[]>(output.ResultObject);
        }
    }
}

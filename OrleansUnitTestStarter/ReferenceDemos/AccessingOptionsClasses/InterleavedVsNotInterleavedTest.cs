using DemoGrains;
using DemoInterfacesAndTypes;
using SamplePatterns;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePatternTests.AccessingOptionsClasses
{
    public class AccessingOptionsTests : IClassFixture<SingleSiloWithClientFixture>
    {
        readonly SingleSiloWithClientFixture _fixture;
        readonly IClusterClient _client;

        public AccessingOptionsTests(SingleSiloWithClientFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.Client!;
        }

        [Fact]
        public async Task CheckCollectionAgeTest()
        {
            var grain = _client.GetGrain<IReadOptionsTest>("test");

            var collectionAge = await grain.GetGrainCollectionAge();

            Assert.True(collectionAge.TotalSeconds>0);
        }
    }
}

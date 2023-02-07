using DemoInterfacesAndTypes;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoGrains
{
    [CollectionAgeLimit(Minutes =2)]
    public class ReadOptionsTest : Grain, IReadOptionsTest
    {
        GrainCollectionOptions _options;

        public ReadOptionsTest(IOptions<GrainCollectionOptions> options) 
            :base()
        {
            _options = options.Value;
        }

        public Task<TimeSpan> GetGrainCollectionAge()
        {
            var type = this.GetType();
            var assemblyQualifiedName = $"{type.FullName},{type.Assembly.GetName().Name}";
            if (_options.ClassSpecificCollectionAge.ContainsKey(assemblyQualifiedName))
            {
                return Task.FromResult(_options.ClassSpecificCollectionAge[assemblyQualifiedName]);
            }
            else
            {
                return Task.FromResult(_options.CollectionAge);
            }
        }
    }
}

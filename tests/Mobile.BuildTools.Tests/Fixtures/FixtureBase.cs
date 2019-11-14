using System;
using System.Collections.Generic;
using System.Text;
using Mobile.BuildTools.Tests.Mocks;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures
{
    public class FixtureBase
    {
        protected ITestOutputHelper _testOutputHelper { get; }

        public FixtureBase(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        protected TestBuildConfiguration GetConfiguration() =>
            new TestBuildConfiguration
            {
                Logger = new XunitLog(_testOutputHelper),
                Platform = Tasks.Utils.Platform.Unsupported,
            };
    }
}

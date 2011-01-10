﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveXaml.Tests;

namespace ReactiveXaml.Serialization.Tests
{
    public class SyncPointInformationTest : IEnableLogger
    {
        [Fact]
        public void EnsureSyncPointInformationCalculatesHash()
        {
            var template = new SyncPointInformation(Guid.NewGuid(), Guid.NewGuid(), typeof (string), "Foo", DateTimeOffset.MinValue);
            var fixtures = new[] {
                template,
                new SyncPointInformation(template.RootObjectHash, template.ParentSyncPoint, typeof (string), template.Qualifier, DateTimeOffset.MaxValue),
                new SyncPointInformation(template.RootObjectHash, template.ParentSyncPoint, typeof (string), "Bar", template.CreatedOn),
                new SyncPointInformation(template.RootObjectHash, template.ParentSyncPoint, typeof (int), template.Qualifier, template.CreatedOn),
                new SyncPointInformation(template.RootObjectHash, Guid.NewGuid(), typeof (string), template.Qualifier, template.CreatedOn),
                new SyncPointInformation(Guid.NewGuid(), template.ParentSyncPoint, typeof (string), template.Qualifier, template.CreatedOn),
            };

            //PexAssert.AreDistinctValues(fixtures.Select(x => x.ContentHash).ToArray());
            foreach(var v in fixtures) {
                var hash = v.ContentHash;
                Assert.True(fixtures.Count(x => x.ContentHash == hash) == 1);
            }
        }

        [Fact]
        public void EnsureSyncPointInformationDoesntSerializeExtraJunk()
        {
            var template = new SyncPointInformation(Guid.NewGuid(), Guid.NewGuid(), typeof (string), "Foo", DateTimeOffset.MinValue);
            var json = JSONHelper.Serialize(template);
            this.Log().Debug(json);

            var expected = new[] {
                "RootObjectHash",
                "ParentSyncPoint",
                "RootObjectTypeName",
                "Qualifier",
                "CreatedOn"
            };

            // TODO: This test sucks out loud
            expected.Run(x => Assert.True(json.Contains(x)));
        }
    }
}

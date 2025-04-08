// StudentInformationManagementSystem.Tests/Helpers/TestUtilities.cs
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Tests.Helpers
{
    public class MockHttpSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();

        public string Id => "testid";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear() => _sessionStorage.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _sessionStorage.Remove(key);

        public void Set(string key, byte[] value) => _sessionStorage[key] = value;

        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);

        // Helper methods for working with session values
        public void SetInt32(string key, int value)
        {
            var bytes = BitConverter.GetBytes(value);
            Set(key, bytes);
        }

        public void SetString(string key, string value)
        {
            Set(key, System.Text.Encoding.UTF8.GetBytes(value));
        }
    }
}
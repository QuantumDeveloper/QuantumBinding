using System;
using System.Collections.Generic;
using System.Linq;

namespace QuantumBinding.Generator.ProcessingFluentApi
{
    public partial class PostProcessingApi
    {
        private readonly Dictionary<string, DelegateExtension> delegates;

        public IFunctionParameter Delegate(string delegateName)
        {
            if (string.IsNullOrEmpty(delegateName))
            {
                throw new ArgumentNullException(nameof(delegateName));
            }

            if (!functions.TryGetValue(delegateName, out _currentFunction))
            {
                var @delegate = new DelegateExtension();
                @delegate.Name = delegateName;
                _currentFunction = @delegate;
                delegates.Add(delegateName, @delegate);
            }

            return this;
        }

        public bool TryGetDelegate(string delegateName, bool matchCase, out DelegateExtension @delegate)
        {
            if (matchCase)
            {
                return delegates.TryGetValue(delegateName, out @delegate);
            }

            var key = delegates.Keys.FirstOrDefault(x => x.Equals(delegateName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(key))
            {
                return delegates.TryGetValue(key, out @delegate);
            }

            @delegate = null;
            return false;
        }
    }
}

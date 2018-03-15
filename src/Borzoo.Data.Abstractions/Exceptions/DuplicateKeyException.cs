﻿using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Borzoo.Data.Abstractions
{
    public class DuplicateKeyException : RepositoryException
    {
        public IEnumerable<string> Keys { get; }

        public DuplicateKeyException(params string[] keys)
            : base(string.Format(@"Duplicate key{0}: ""{1}""",
                keys.Length > 1 ? "s" : string.Empty,
                string.Concat(", ", keys)
            ))
        {
            Keys = keys;
        }
    }
}
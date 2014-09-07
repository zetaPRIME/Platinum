// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

using System;
using System.Collections;
using System.Collections.Generic;

namespace FluentPath {
    public class FPathEnumerator : IEnumerator<FPath> {
        private IEnumerator<string> _pathEnumerator;

        public FPathEnumerator(IEnumerable<string> paths) {
            _pathEnumerator = paths.GetEnumerator();
        }

        FPath IEnumerator<FPath>.Current {
            get { return new FPath(_pathEnumerator.Current); }
        }

        void IDisposable.Dispose() {
            _pathEnumerator.Dispose();
        }

        object IEnumerator.Current {
            get { return new FPath(_pathEnumerator.Current); }
        }

        bool IEnumerator.MoveNext() {
            return _pathEnumerator.MoveNext();
        }

        void IEnumerator.Reset() {
            _pathEnumerator.Reset();
        }
    }
}

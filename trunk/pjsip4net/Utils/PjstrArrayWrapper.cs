using System;
using System.Collections;
using System.Collections.Generic;
using pjsip.Interop;

namespace pjsip4net.Utils
{
    public class PjstrArrayWrapper : Initializable, ICollection<string>
    {
        private readonly pj_str_t[] _array;
        private int _inx;

        internal PjstrArrayWrapper(pj_str_t[] array)
        {
            Helper.GuardNotNull(array);

            _array = array;

            for (int i = 0; i < _array.Length; i++)
                if (Equals(_array[i], default(pj_str_t)))
                {
                    _inx = i;
                    break;
                }
        }

        #region ICollection<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return new WrapperEnumerator(_array);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string item)
        {
            GuardNotInitializing();
            if (_inx >= _array.Length)
                throw new ArgumentOutOfRangeException("exceeded wrapped array size");

            _array[_inx++] = new pj_str_t(item);
        }

        public void Clear()
        {
            GuardNotInitializing();
            Array.Clear(_array, 0, _array.Length);
        }

        public bool Contains(string item)
        {
            return Array.IndexOf(_array, new pj_str_t(item)) != -1;
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string item)
        {
            GuardNotInitializing();
            if (_inx > 0)
            {
                _array[--_inx] = default(pj_str_t);
                return true;
            }
            return false;
        }

        public int Count
        {
            get { return _inx; }
        }

        public bool IsReadOnly
        {
            get { return _inx >= _array.Length; }
        }

        #endregion

        #region Nested type: WrapperEnumerator

        private class WrapperEnumerator : IEnumerator<string>
        {
            private pj_str_t[] _array;
            private IEnumerator _wrappedEnmr;

            public WrapperEnumerator(pj_str_t[] array)
            {
                _array = array;
                _wrappedEnmr = _array.GetEnumerator();
            }

            #region IEnumerator<string> Members

            public void Dispose()
            {
                _array = null;
                _wrappedEnmr = null;
            }

            public bool MoveNext()
            {
                return _wrappedEnmr.MoveNext();
            }

            public void Reset()
            {
                _wrappedEnmr.Reset();
            }

            public string Current
            {
                get { return (pj_str_t) _wrappedEnmr.Current; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            #endregion
        }

        #endregion
    }
}
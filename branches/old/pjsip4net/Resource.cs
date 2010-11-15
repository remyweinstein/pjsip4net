using System;

namespace pjsip4net
{
    public class Resource
    {
        protected bool _isDisposed;

        protected void GuardDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Initializable");
        }

        #region Implementation of IDisposable

        internal void InternalDispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }

            CleanUp();
        }

        ~Resource()
        {
            Dispose(false);
        }

        protected virtual void CleanUp()
        {
        }

        #endregion
    }
}
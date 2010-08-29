//using System;
//using System.Runtime.InteropServices;

//namespace DoxWox.SIPUserAgent.Utils
//{
//    public class PJSTRTMarshaler : ICustomMarshaler
//    {
//        #region Singleton

//        private static readonly object _lock = new object();
//        private static PJSTRTMarshaler _instance;

//        private PJSTRTMarshaler()
//        {}

//        internal static PJSTRTMarshaler GetInstance(string cookie)
//        {
//            if (_instance == null)
//                lock (_lock)
//                    if (_instance == null)
//                        _instance = new PJSTRTMarshaler();
//            return _instance;
//        }

//        #endregion

//        #region ICustomMarshaler Members

//        public void CleanUpManagedData(object ManagedObj)
//        {
//            //
//        }

//        public void CleanUpNativeData(IntPtr pNativeData)
//        {
//            Marshal.FreeHGlobal(pNativeData);
//        }

//        public int GetNativeDataSize()
//        {
//            throw new NotImplementedException();
//        }

//        public IntPtr MarshalManagedToNative(object ManagedObj)
//        {
//            //pj_str_t -> pj_str_t
//            pj_str_t str = (pj_str_t) ManagedObj;//unbox
//            IntPtr dest = Marshal.AllocHGlobal(2 * sizeof(int));
//            if (string.IsNullOrEmpty(str.ptr))
//            {
//                IntPtr source = Marshal.StringToHGlobalAnsi(str.ptr);
//                Marshal.WriteInt32(dest, (int)source);//char*
//                IntPtr pslen = (IntPtr)((int)dest + sizeof(int));
//                Marshal.WriteInt32(pslen, str.slen);
//            }
//            else
//            {
//                Marshal.WriteInt32(dest, 0);//char*
//                IntPtr pslen = (IntPtr)((int)dest + sizeof(int));
//                Marshal.WriteInt32(pslen, 0);
//            }

//            return dest;
//        }

//        public object MarshalNativeToManaged(IntPtr pNativeData)
//        {
//            return Marshal.PtrToStructure(pNativeData, typeof (pj_str_t));
//        }

//        #endregion
//    }
//}

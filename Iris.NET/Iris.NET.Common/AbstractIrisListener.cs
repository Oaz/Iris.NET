﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Iris.NET
{
    public abstract class AbstractIrisListener
    {
        #region Properties
        public bool IsListening => _thread != null && _keepListening;
        #endregion

        protected Thread _thread;
        private volatile bool _keepListening;
        protected int _failureAttempts;

        public AbstractIrisListener(int failureAttempts = 1)
        {
            _failureAttempts = failureAttempts;
        }

        #region Events
        internal delegate void InvalidDataHandler(object data);
        internal event InvalidDataHandler OnInvalidDataReceived;

        internal delegate void ExceptionHandler(Exception ex);
        internal event ExceptionHandler OnException;

        internal delegate void ErrorHandler(IrisError error);
        internal event ErrorHandler OnErrorReceived;

        internal delegate void MessageHandler(IUserSubmittedPacket packet);
        internal event MessageHandler OnUserSubmittedPacketReceived;

        internal delegate void MetaHandler(IrisMeta meta);
        internal event MetaHandler OnMetaReceived;

        internal delegate void VoidHandler();
        internal event VoidHandler OnNullReceived;
        #endregion

        #region Abstract
        protected abstract void InitListenCycle();

        protected abstract object ReadObject();

        protected abstract void OnStop();
        #endregion

        #region Public
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Start()
        {
            if (!IsListening)
            {
                _keepListening = true;
                _thread = new Thread(Listen);
                _thread.Start();
                // Loop until worker thread activates.
                while (!_thread.IsAlive) ;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Stop()
        {
            if (IsListening)
            {
                _keepListening = false;
                OnStop();
                _thread.Join();
                _thread = null;
            }
        }
        #endregion

        protected virtual void Listen()
        {
            while (_keepListening)
            {
                object data = null;
                InitListenCycle();

                try
                {
                    data = ReadObject();

                    if (_keepListening)
                    {
                        if (data == null)
                            OnNullReceived?.BeginInvoke(null, null);
                        else if (data is IrisError)
                            OnErrorReceived?.BeginInvoke((IrisError)data, null, null);
                        else if (data is IrisMeta)
                            OnMetaReceived?.BeginInvoke((IrisMeta)data, null, null);
                        else
                            OnUserSubmittedPacketReceived?.BeginInvoke((IUserSubmittedPacket)data, null, null);
                    }
                }
                catch (InvalidCastException)
                {
                    OnInvalidDataReceived?.BeginInvoke(data, null, null);
                }
                catch (Exception ex)
                {
                    OnException?.BeginInvoke(ex, null, null);
                }
            }
        }
    }
}

﻿using System;

namespace PlusGame.Framework.IO
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Sender : IDisposable
    {
        /// <summary>
        /// 编码
        /// </summary>
        public abstract void Encode();

        /// <summary>
        /// 加签名
        /// </summary>
        public abstract void AppendSign();

        /// <summary>
        /// 
        /// </summary>
        public abstract void Send();

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            DoDispose(true);
        }
		/// <summary>
		/// Dos the dispose.
		/// </summary>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected virtual void DoDispose(bool disposing)
        {
            if (disposing)
            {
                //清理托管对象
                GC.SuppressFinalize(this);
            }
            //清理非托管对象
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Format
{
    /// <summary>
    /// 自定义的一个字符串阅读器
    /// </summary>
    public class CustomStringReader : IDisposable
    {
        private int _next = 0;              // 下一个要读取的字符标记  
        private int _mark = 0;              // 对字符串中的字符进行标记  
        private string _str;
        private object _lockObj = new object();
        private int _length;

        public CustomStringReader(string s)
        {
            this._str = s;
            this._length = s.Length;
        }

        /// <summary>
        /// 记录当前读取位置
        /// </summary>
        /// <param name="readAheadLimit"></param>
        public virtual void Mark(int readAheadLimit)
        {
            if (readAheadLimit < 0)
            {
                throw new ArgumentException("Read-ahead limit < 0");
            }
            lock (_lockObj)
            {
                EnsureOpen();
                _mark = _next;   // 标记为当前的读取位置  
            }
        }
        /// <summary>
        /// 将读取位置恢复到之前mark的读取位置
        /// </summary>
        public virtual void Reset()
        {
            lock (_lockObj)
            {
                EnsureOpen();
                _next = _mark;
            }
        }

        /// <summary>
        /// 单个字节读取
        /// </summary>
        /// <returns></returns>
        public virtual int Read()
        {
            lock (_lockObj)
            {
                EnsureOpen();
                if (_next >= _length)
                    return -1;
                return _str[_next++];
            }
        }

        private void EnsureOpen()
        {
            if (_str == null)
                throw new IOException("Stream closed");
        }

        public void close()
        {
            _str = null;
        }
        
        public void Dispose()
        {
            _str = null;
            _lockObj = null;
        }
    }
}

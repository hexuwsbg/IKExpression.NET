using Expression.Format.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Format
{
    public class ExpressionReader : CustomStringReader
    {
        private static string IGNORE_CHAR = " \r\n\t";//词元间的忽略字符

        private int currentIndex = 0;//当前索引

        private int markIndex = 0;//被标记后索引

        private bool prefixBlank = false;//与上一个读到的ElementToken之间是否有空格

        public ExpressionReader(string s) : base(s)
        {
        }

        /**
         * 取得当前位置
         * @return
         */
        public int GetCurrentIndex()
        {
            return currentIndex;
        }

        /**
         * Element之前是否有空格
         * @return
         */
        public bool IsPrefixBlank()
        {
            return prefixBlank;
        }

        public void SetPrefixBlank(bool prefixBlank)
        {
            this.prefixBlank = prefixBlank;
        }

        public override int Read()
        {
            int c = base.Read();
            if (c != -1) {
                currentIndex++;
                markIndex++;
            }
            return c;
        }

        //    public int Read(char[] cbuf)
        //{
        //		int c = base.Read(cbuf);
        //		if (c > 0) {
        //        currentIndex += c;
        //        markIndex += c;
        //    }
        //		return c;
        //}

        //    public int Read(CharBuffer target)
        //{
        //		int c = base.Read(target);
        //		if (c > 0) {
        //        currentIndex += c;
        //        markIndex += c;
        //    }
        //		return c;
        //}
        //public override int Read(char[] cbuf, int off, int len)
        //{
        //    int c = base.Read(cbuf, off, len);
        //    if (c > 0) {
        //        currentIndex += c;
        //        markIndex += c;
        //    }
        //    return c;
        //}

        public override void Reset()
        {
            base.Reset();
            currentIndex = currentIndex - markIndex;
        }


        public override void Mark(int readAheadLimit)
        {
            base.Mark(readAheadLimit);
            markIndex = 0;
        }

        /**
         * 以流形式读取ExpressionToken
         * @return ExpressionToken
         * @throws IOException
         * @throws FormatException
         * @throws AntipathicException
         */
        public Element ReadToken()
        {
            prefixBlank = false;
            while (true) {
                //去除空格
                Mark(0);//标记
                int b = Read();
                if (b == -1) {
                    return null;
                }
                char c = (char)b;
                if (IGNORE_CHAR.IndexOf(c) >= 0) {//去除开始的空格
                    prefixBlank = true;
                    continue;
                }

                Reset();//重置

                //构造一个词元读取器
                IElementReader er = ElementReaderFactory.CreateElementReader(this);

                return er.Read(this);
            }
        }

        //public static void main(String[] a)
        //{
        //    ExpressionReader eReader = new ExpressionReader(" aa+\"AB\\\\CD\"!=null&&[2008-1-1 12:9]-$max(aa,bb,\"cc\")>2l3f4d1");

        //    Element ele = null;
        //    try
        //    {
        //        while ((ele = eReader.readToken()) != null)
        //        {
        //            System.out.println(ele.getType() + "……" + ele.getText() + "……" + ele.getIndex());
        //        }
        //    }
        //    catch (IOException e)
        //    {
        //        e.printStackTrace();
        //    }
        //    catch (FormatException e)
        //    {
        //        e.printStackTrace();
        //    }
        //}
    }
}

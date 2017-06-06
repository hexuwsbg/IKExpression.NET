using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Expression.Metadata.BaseMetadata;

namespace Expression.Metadata
{
    public abstract class BaseMetadata
    {
        //数据类型
        public enum DataType
        {
            //NULL类型
            DATATYPE_NULL,
            //字符窜
            DATATYPE_STRING,
            //布尔类
            DATATYPE_BOOLEAN,
            //整数
            DATATYPE_INT,
            //长整数
            DATATYPE_LONG,
            //浮点数
            DATATYPE_FLOAT,
            //双精度浮点
            DATATYPE_DOUBLE,
            //日期时间
            DATATYPE_DATE,
            //集合对象
            DATATYPE_LIST,
            //通用对象类型
            DATATYPE_OBJECT,
        }

        //数据类型
        public DataType _dataType;
        //值
        public object DataValue { protected set; get; }
        //引用类型标识
        public bool IsReference { set; get; }

        public BaseMetadata(DataType dataType, object dataValue, bool isReference = false)
        {
            this._dataType = dataType;
            this.DataValue = dataValue;
            IsReference = isReference;
            //参数类型校验
            VerifyMetadata();
        }

        public DataType GetDataType()
        {
            if (IsReference)
            {
                return this.GetReference().GetDataType();
            }
            else
            {
                return _dataType;
            }
        }

        public virtual void SetDataType(DataType dataType)
        {
            this._dataType = dataType;
        }

        public string GetDataValueText()
        {
            if (DataValue == null)
            {
                return null;

            }
            else if (DataType.DATATYPE_DATE == this._dataType)
            {
                return ((DateTime)DataValue).ToString("yyyy-MM-dd HH:mm:ss");

            }
            else if (DataType.DATATYPE_LIST == this._dataType)
            {
                StringBuilder buff = new StringBuilder("[");
                IList col = (List<object>)DataValue;
                foreach (var o in col)
                {
                    if (o == null)
                    {
                        buff.Append("null, ");
                    }
                    else if (o is DateTime)
                    {
                        buff.Append(((DateTime)o).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        buff.Append(o.ToString()).Append(", ");
                    }
                }
                buff.Append("]");
                if (buff.Length > 2)
                {
                    buff.Remove(buff.Length - 3, buff.Length - 1);
                }
                return buff.ToString();

            }
            else
            {
                return DataValue.ToString();
            }
        }


        /**
         * 获取Token的字符窜类型值
         * @return
         */
        public string GetStringValue()
        {
            return GetDataValueText();
        }
        /**
         * 获取Token的boolean类型值
         * @return
         */
        public bool GetBooleanValue()
        {
            if (DataType.DATATYPE_BOOLEAN != this._dataType)
            {
                throw new Exception("当前常量类型不支持此操作");
            }
            return (bool)DataValue;
        }

        /**
         * 获取Token的int类型值
         * @return
         */
        public int GetIntegerValue()
        {

            if (DataType.DATATYPE_INT != this._dataType)
            {
                throw new Exception("当前常量类型不支持此操作");
            }
            return (int)DataValue;
        }

        /**
         * 获取Token的long类型值
         * @return
         */
        public long GetLongValue()
        {

            if (DataType.DATATYPE_INT != this._dataType
                    && DataType.DATATYPE_LONG != this._dataType)
            {
                throw new Exception("当前常量类型不支持此操作");
            }
            if (DataValue == null)
            {
                throw new Exception("当前常量类型不能为空");
            }
            return long.Parse(DataValue.ToString());
        }

        /**
         * 获取Token的float类型值
         * @return
         */
        public float GetFloatValue()
        {

            if (DataType.DATATYPE_INT != this._dataType
                    && DataType.DATATYPE_FLOAT != this._dataType
                    && DataType.DATATYPE_LONG != this._dataType)
            {
                throw new Exception("当前常量类型不支持此操作");
            }
            if (DataValue == null)
            {
                throw new Exception("当前常量类型不能为空");
            }
            return float.Parse(DataValue.ToString());
        }

        /**
         * 获取Token的double类型值
         * @return
         */
        public double GetDoubleValue()
        {
            if (DataType.DATATYPE_INT != this._dataType
                    && DataType.DATATYPE_LONG != this._dataType
                    && DataType.DATATYPE_FLOAT != this._dataType
                    && DataType.DATATYPE_DOUBLE != this._dataType)
            {
                throw new Exception("当前常量类型不支持此操作");
            }
            if (DataValue == null)
            {
                throw new Exception("当前常量类型不能为空");
            }
            return double.Parse(DataValue.ToString());
        }

        /**
         * 获取Token的Date类型值
         * @return
         * @throws ParseException 
         */
        public DateTime GetDateValue()
        {
            if (DataType.DATATYPE_DATE != this._dataType)
            {
                throw new Exception("当前常量类型不支持此操作");
            }
            return (DateTime)DataValue;
        }


        /**
         * 获取数据的集合对象
         * @return
         */

        public List<object> GetCollection()
        {
            if (DataType.DATATYPE_LIST != this._dataType)
            {
                throw new Exception("当前常量类型不支持此操作");
            }
            return (List<object>)DataValue;
        }

        /**
         * 获取Token的引用对象
         * @return
         */
        public Reference GetReference()
        {
            if (!this.IsReference)
            {
                throw new Exception("当前常量类型不支持此操作");
            }
            return (Reference)DataValue;
        }

        public bool equals(object o)
        {

            if (o == this)
            {
                return true;

            }
            else if (o is BaseMetadata)
            {

                BaseMetadata bdo = (BaseMetadata)o;
                if (this.IsReference && bdo.IsReference)
                {
                    return this.GetReference() == bdo.GetReference();
                }

                if (bdo.GetDataType() == _dataType)
                {
                    if (bdo.DataValue != null
                            && bdo.DataValue.Equals(DataValue))
                    {
                        return true;
                    }
                    else if (bdo.DataValue == null
                           && DataValue == null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        /**
         * 校验数据类型和值得合法性
         */
        protected void VerifyMetadata()
        {
            if (DataValue != null)
            {
                if (DataType.DATATYPE_NULL == _dataType && DataValue != null && !IsReference)
                {
                    throw new ArgumentException("数据类型不匹配; 类型：" + _dataType + ",值不为空");

                }
                else if (DataType.DATATYPE_BOOLEAN == _dataType)
                {
                    try
                    {
                        GetBooleanValue();
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("数据类型不匹配; 类型：" + _dataType + ",值:" + DataValue);
                    }

                }
                else if (DataType.DATATYPE_DATE == _dataType)
                {
                    try
                    {
                        GetDateValue();
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("数据类型不匹配; 类型：" + _dataType + ",值:" + DataValue);
                    }

                }
                else if (DataType.DATATYPE_DOUBLE == _dataType)
                {
                    try
                    {
                        GetDoubleValue();
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("数据类型不匹配; 类型：" + _dataType + ",值:" + DataValue);
                    }

                }
                else if (DataType.DATATYPE_FLOAT == _dataType)
                {
                    try
                    {
                        GetFloatValue();
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("数据类型不匹配; 类型：" + _dataType + ",值:" + DataValue);
                    }

                }
                else if (DataType.DATATYPE_INT == _dataType)
                {
                    try
                    {
                        GetIntegerValue();
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("数据类型不匹配; 类型：" + _dataType + ",值:" + DataValue);
                    }

                }
                else if (DataType.DATATYPE_LONG == _dataType)
                {
                    try
                    {
                        GetLongValue();
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("数据类型不匹配; 类型：" + _dataType + ",值:" + DataValue);
                    }

                }
                else if (DataType.DATATYPE_STRING == _dataType)
                {
                    try
                    {
                        GetStringValue();
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("数据类型不匹配; 类型：" + _dataType + ",值:" + DataValue);
                    }

                }
                else if (DataType.DATATYPE_LIST == _dataType)
                {
                    try
                    {
                        GetCollection();
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("数据类型不匹配; 类型：" + _dataType + ",值:" + DataValue);
                    }

                }
                else if (this.IsReference)
                {
                    try
                    {
                        GetReference();
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("数据类型不匹配; 类型：" + _dataType + ",值:" + DataValue);
                    }

                }
            }
        }

        public object GetObjectValue()
        {
            if (null == this.DataValue)
            {
                return null;
            }

            if (DataType.DATATYPE_BOOLEAN == this.GetDataType())
            {
                return GetBooleanValue();

            }
            else if (DataType.DATATYPE_DATE == this.GetDataType())
            {
                return GetDateValue();

            }
            else if (DataType.DATATYPE_DOUBLE == this.GetDataType())
            {
                return GetDoubleValue();

            }
            else if (DataType.DATATYPE_FLOAT == this.GetDataType())
            {
                return GetFloatValue();

            }
            else if (DataType.DATATYPE_INT == this.GetDataType())
            {
                return GetIntegerValue();

            }
            else if (DataType.DATATYPE_LONG == this.GetDataType())
            {
                return GetLongValue();

            }
            else if (DataType.DATATYPE_STRING == this.GetDataType())
            {
                return GetStringValue();

            }
            else if (DataType.DATATYPE_LIST == this.GetDataType())
            {
                return GetCollection();

            }
            else if (DataType.DATATYPE_OBJECT == this.GetDataType())
            {
                return DataValue;

            }
            else
            {
                throw new Exception("映射类型失败：无法识别的数据类型");
            }
        }

        public Type mapTypeToJavaClass()
        {

            if (DataType.DATATYPE_BOOLEAN == this.GetDataType())
            {
                return typeof(bool);

            }
            else if (DataType.DATATYPE_DATE == this.GetDataType())
            {
                return typeof(DateTime);

            }
            else if (DataType.DATATYPE_DOUBLE == this.GetDataType())
            {
                return typeof(double);

            }
            else if (DataType.DATATYPE_FLOAT == this.GetDataType())
            {
                return typeof(float);

            }
            else if (DataType.DATATYPE_INT == this.GetDataType())
            {
                return typeof(int);

            }
            else if (DataType.DATATYPE_LONG == this.GetDataType())
            {
                return typeof(long);

            }
            else if (DataType.DATATYPE_STRING == this.GetDataType())
            {
                return typeof(string);

            }
            else if (DataType.DATATYPE_LIST == this.GetDataType())
            {
                return typeof(List<object>);

            }
            else if (DataType.DATATYPE_OBJECT == this.GetDataType())
            {
                return typeof(object);

            }
            else if (DataType.DATATYPE_NULL == this.GetDataType())
            {
                if (IsReference)
                {
                    return typeof(Reference);
                }
                return null;

            }
            throw new Exception("映射Java类型失败：无法识别的数据类型");
        }
    }
}

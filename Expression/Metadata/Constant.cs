using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression.Metadata
{
    public class Constant : BaseMetadata
    {
        public Constant(DataType dataType, object value) : base(dataType, value)
        {
            //如果为集合类型，生成集合容器
            if (DataType.DATATYPE_LIST == dataType)
            {
                if (DataValue == null)
                {
                    DataValue = new List<object>(0);
                }
            }

        }


        public Constant(Reference reference) : base(DataType.DATATYPE_NULL, reference, true)
        {
        }
    }
}

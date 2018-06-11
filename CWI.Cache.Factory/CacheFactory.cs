using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWI.Cache.Factory
{
    public class CacheFactory
    {
        /// <summary>
        /// 定义通用的Repository
        /// </summary>
        /// <returns></returns>
        public static ICache Cache()
        {
            return new CWI.Cache.Redis.Cache();
        }
    }
}

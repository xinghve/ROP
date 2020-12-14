using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.Cache
{
    public interface IRedisCache: ICacheService
    {
        void RemoveAll<V>();
    }
}

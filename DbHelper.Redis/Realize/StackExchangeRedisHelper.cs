using DbHelper.Redis.CommEnum;
using DbHelper.Redis.ConncetionHelper;
using DbHelper.Redis.Interface;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbHelper.Redis.Realize
{
    public class StackExchangeRedisHelper : IRedisHelper
    {
        private int dbNum { get; }
        private readonly IConnectionMultiplexer conn;
        private string SysCustomKey { get; set; }

        private string RedisConnectionString { get; set; }

        #region 构造函数

        public StackExchangeRedisHelper()
                : this(0, null, string.Empty)
        {

        }


        public StackExchangeRedisHelper(int dbNum, string redisConnectionString, string sysCustomKey)
        {
            this.dbNum = dbNum;
            this.SysCustomKey = sysCustomKey;
            if (string.IsNullOrWhiteSpace(sysCustomKey))
            {
                this.SysCustomKey = StackExchangeRedisConnectionHelper.SysCustomKey;
            }
            conn = string.IsNullOrWhiteSpace(redisConnectionString) ? StackExchangeRedisConnectionHelper.Instance : StackExchangeRedisConnectionHelper.GetConnectionMultiplexer(redisConnectionString);
        }

        #endregion 构造函数

        #region 辅助方法

        private string AddSysCustomKey(string oldKey)
        {
            var prefixKey = SysCustomKey ?? StackExchangeRedisConnectionHelper.SysCustomKey;
            return prefixKey + oldKey;
        }

        private List<string> AddSysCustomKeyList(IEnumerable<string> oldKeyList)
        {
            var prefixKey = SysCustomKey ?? StackExchangeRedisConnectionHelper.SysCustomKey;
            List<string> newKeyList = new List<string>();
            newKeyList.AddRange(oldKeyList);
            for (int i = 0; i < newKeyList.Count; i++)
            {
                newKeyList[i] = prefixKey + newKeyList[i];
            }
            return newKeyList;
        }

        private T Do<T>(Func<IDatabase, T> func)
        {
            var database = conn.GetDatabase(dbNum);
            return func(database);
        }

        private string ConvertJson<T>(T value)
        {
            string result = (value is string || value is int || value is long || value is float || value is double || value is decimal || value is byte || value is sbyte || value is ulong || value is short || value is ushort || value is uint || value is char || value is bool) ? value.ToString() : JsonConvert.SerializeObject(value);
            return result;
        }

        private List<string> ConvertJsonList<T>(IEnumerable<T> values)
        {
            List<string> result = new List<string>();
            foreach (var item in values)
            {
                var model = ConvertJson<T>(item);
                result.Add(model);
            }
            return result;
        }

        private T ConvertObj<T>(RedisValue value)
        {
            string result = "";
            var typeT = typeof(T);
            if (typeT.Equals(typeof(string)))
            {
                result = (!value.IsNull) && (!value.IsNullOrEmpty) ? value.ToString() : "";
                return (T)Convert.ChangeType(result, typeT);
            }
            else if (typeT.Equals(typeof(int)) || typeT.Equals(typeof(long)) || typeT.Equals(typeof(float)) || typeT.Equals(typeof(double)) || typeT.Equals(typeof(decimal)) || typeT.Equals(typeof(byte)) || typeT.Equals(typeof(sbyte)) || typeT.Equals(typeof(ulong)) || typeT.Equals(typeof(short)) || typeT.Equals(typeof(ushort)) || typeT.Equals(typeof(uint)) || typeT.Equals(typeof(char)) || typeT.Equals(typeof(bool)))
            {
                if ((value.IsNull) || (value.IsNullOrEmpty) || !value.HasValue)
                {
                    return default(T);
                }
                return (T)Convert.ChangeType(value, typeT);
            }
            return JsonConvert.DeserializeObject<T>(value);
        }

        private List<T> ConvetList<T>(IEnumerable<RedisValue> values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }

        private RedisKey[] ConvertRedisKeys(IEnumerable<string> redisKeys)
        {
            return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        }

        #endregion 辅助方法

        #region String 操作

        #region 同步方法

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool StringSet(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            return Do(db => db.StringSet(key, value, expiry));
        }

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public bool StringSet<T>(IEnumerable<KeyValuePair<string, T>> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newkeyValues =
                keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(AddSysCustomKey(p.Key), ConvertJson(p.Value))).ToList();
            return Do(db => db.StringSet(newkeyValues.ToArray()));
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool StringSet<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            string json = ConvertJson(obj);
            return Do(db => db.StringSet(key, json, expiry));
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public string StringGet(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.StringGet(key));
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public List<T> StringGet<T>(IEnumerable<string> listKey)
        {
            List<string> newKeys = AddSysCustomKeyList(listKey).ToList();
            return Do(db => ConvetList<T>(db.StringGet(ConvertRedisKeys(newKeys))));
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T StringGet<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => ConvertObj<T>(db.StringGet(key)));
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double StringIncrement(string key, double val = 1)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.StringIncrement(key, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double StringDecrement(string key, double val = 1)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.StringDecrement(key, val));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.StringSetAsync(key, value, expiry));
        }

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync<T>(IEnumerable<KeyValuePair<string, T>> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newkeyValues =
                keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(AddSysCustomKey(p.Key), ConvertJson<T>(p.Value))).ToList();
            return await Do(db => db.StringSetAsync(newkeyValues.ToArray()));
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            string json = ConvertJson(obj);
            return await Do(db => db.StringSetAsync(key, json, expiry));
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public async Task<string> StringGetAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.StringGetAsync(key));
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public async Task<List<T>> StringGetAsync<T>(IEnumerable<string> listKey)
        {
            List<string> newKeys = listKey.Select(AddSysCustomKey).ToList();
            var values = await Do(db => db.StringGetAsync(ConvertRedisKeys(newKeys)));
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> StringGetAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            string result = await Do(db => db.StringGetAsync(key));
            return ConvertObj<T>(result);
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double> StringIncrementAsync(string key, double val = 1)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.StringIncrementAsync(key, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> StringDecrementAsync(string key, double val = 1)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.StringDecrementAsync(key, val));
        }

        #endregion 异步方法

        #endregion String 操作

        #region List 操作

        #region 同步方法

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRemove<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            Do(db => db.ListRemove(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> ListRange<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(redis =>
            {
                var values = redis.ListRange(key);
                return ConvetList<T>(values);
            });
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRightPush<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            Do(db => db.ListRightPush(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListRightPop<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                var value = db.ListRightPop(key);
                return ConvertObj<T>(value);
            });
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListLeftPush<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            Do(db => db.ListLeftPush(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListLeftPop<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                var value = db.ListLeftPop(key);
                return ConvertObj<T>(value);
            });
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.ListLength(key));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListRemoveAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.ListRemoveAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> ListRangeAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            var values = await Do(redis => redis.ListRangeAsync(key));
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListRightPushAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.ListRightPushAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListRightPopAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            var value = await Do(db => db.ListRightPopAsync(key));
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListLeftPushAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.ListLeftPushAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListLeftPopAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            var value = await Do(db => db.ListLeftPopAsync(key));
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.ListLengthAsync(key));
        }

        #endregion 异步方法

        #endregion List 操作

        #region Hash 操作

        #region 同步方法

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashExists(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.HashExists(key, dataKey));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string dataKey, T t)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                string json = ConvertJson(t);
                return db.HashSet(key, dataKey, json);
            });
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.HashDelete(key, dataKey));
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public long HashDelete<T>(string key, IEnumerable<T> dataKeys)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.HashDelete(key, ConvertJsonList(dataKeys).Select(i => (RedisValue)i).ToArray()));
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                string value = db.HashGet(key, dataKey);
                return ConvertObj<T>(value);
            });
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double HashIncrement(string key, string dataKey, double val = 1)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.HashIncrement(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double HashDecrement(string key, string dataKey, double val = 1)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.HashDecrement(key, dataKey, val));
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                RedisValue[] values = db.HashKeys(key);
                return ConvetList<T>(values);
            });
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashExistsAsync(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.HashExistsAsync(key, dataKey));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync<T>(string key, string dataKey, T t)
        {
            key = AddSysCustomKey(key);
            return await Do(db =>
            {
                string json = ConvertJson(t);
                return db.HashSetAsync(key, dataKey, json);
            });
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.HashDeleteAsync(key, dataKey));
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public async Task<long> HashDeleteAsync<T>(string key, IEnumerable<T> dataKeys)
        {
            key = AddSysCustomKey(key);
            //List<RedisValue> dataKeys1 = new List<RedisValue>() {"1","2"};
            return await Do(db => db.HashDeleteAsync(key, ConvertJsonList(dataKeys).Select(i => (RedisValue)i).ToArray()));
        }

    /// <summary>
    /// 从hash表获取数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="dataKey"></param>
    /// <returns></returns>
    public async Task<T> HashGeAsync<T>(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            string value = await Do(db => db.HashGetAsync(key, dataKey));
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double> HashIncrementAsync(string key, string dataKey, double val = 1)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.HashIncrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> HashDecrementAsync(string key, string dataKey, double val = 1)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.HashDecrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> HashKeysAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            RedisValue[] values = await Do(db => db.HashKeysAsync(key));
            return ConvetList<T>(values);
        }

        #endregion 异步方法

        #endregion Hash 操作

        #region Set 集合

        #region 同步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public bool SetAdd<T>(string key, T value, double score)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SetAdd(key, ConvertJson<T>(value)));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SetRemove<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SetRemove(key, ConvertJson(value)));
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SetContains<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SetContains(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SetMembers<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(redis =>
            {
                var values = redis.SetMembers(key);
                return ConvetList<T>(values);
            });
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SetLength(string key)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SetLength(key));
        }

        /// <summary>
        /// 获取多个集合的交集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public List<T> SetIntersect<T>(IEnumerable<string> keyList)
        {
            var values = SetCombine<T>(keyList, SetCombineOperation.Intersect);
            return values;
        }

        /// <summary>
        /// 获取多个集合的并集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public List<T> SetUnion<T>(IEnumerable<string> keyList)
        {
            var values = SetCombine<T>(keyList, SetCombineOperation.Union);
            return values;
        }

        /// <summary>
        /// 获取多个集合的差集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public List<T> SetDifference<T>(IEnumerable<string> keyList)
        {
            var values = SetCombine<T>(keyList, SetCombineOperation.Difference);
            return values;
        }

        /// <summary>
        /// 获取多个集合的差集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public List<T> SetCombine<T>(IEnumerable<string> keyList, SetCombineOperation operation)
        {
            var tempkeyList = AddSysCustomKeyList(keyList);
            var keys = ConvertRedisKeys(tempkeyList);
            return ConvetList<T>(Do(redis => redis.SetCombine((SetOperation)operation, keys)));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<bool> SetAddAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SetAddAsync(key, ConvertJson<T>(value)));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<bool> SetRemoveAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SetRemoveAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<bool> SetContainsAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SetContainsAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> SetMembersAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            var values = await Do(redis => redis.SetMembersAsync(key));
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> SetLengthAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SetLengthAsync(key));
        }

        /// <summary>
        /// 获取多个集合的交集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public async Task<List<T>> SetIntersectAsync<T>(IEnumerable<string> keyList)
        {
            var values = await SetCombineAsync<T>(keyList, SetCombineOperation.Intersect);
            return values;
        }

        /// <summary>
        /// 获取多个集合的并集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public async Task<List<T>> SetUnionAsync<T>(IEnumerable<string> keyList)
        {
            var values = await SetCombineAsync<T>(keyList, SetCombineOperation.Union);
            return values;
        }

        /// <summary>
        /// 获取多个集合的差集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public async Task<List<T>> SetDifferenceAsync<T>(IEnumerable<string> keyList)
        {
            var values = await SetCombineAsync<T>(keyList, SetCombineOperation.Difference);
            return values;
        }

        /// <summary>
        /// 获取多个集合的并集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public async Task<List<T>> SetCombineAsync<T>(IEnumerable<string> keyList, SetCombineOperation operation)
        {
            var tempkeyList = AddSysCustomKeyList(keyList);
            var keys = ConvertRedisKeys(tempkeyList);
            var values = await Do(redis => redis.SetCombineAsync((SetOperation)operation, keys));
            return ConvetList<T>(values);
        }

        #endregion 异步方法

        #endregion Set集合

        #region SortedSet 有序集合

        #region 同步方法

        /// <summary>
        /// 添加一个数据到Key
        /// </summary>
        /// <typeparam name="T">要添加的数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="score">分数</param>
        /// <returns></returns>
        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SortedSetAdd(key, ConvertJson<T>(value), score));
        }

        /// <summary>
        /// 添加一个集合到Key
        /// </summary>
        /// <typeparam name="T">要添加的数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="valueMapScore">值与分数的对照集合</param>
        /// <returns></returns>
        public long SortedSetAdd<T>(string key, IEnumerable<KeyValuePair<T, double?>> valueMapScore)
        {
            key = AddSysCustomKey(key);
            SortedSetEntry[] rValue = valueMapScore.Select(o => new SortedSetEntry(ConvertJson<T>(o.Key), o.Value ?? 0)).ToArray();
            return Do(redis => redis.SortedSetAdd(key, rValue));
        }

        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public List<T> SortedSetRangeByRank<T>(string key)
        {
            key = AddSysCustomKey(key);
            var values = Do(redis => redis.SortedSetRangeByRank(key));
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 获取指定起始值到结束值的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="begValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <param name="rangeType">开闭区间控制</param>
        /// <returns></returns>
        public List<T> SortedSetRangeByValue<T>(string key, T begValue, T endValue, RangeType rangeType = RangeType.None)
        {
            key = AddSysCustomKey(key);
            var bValue = ConvertJson<T>(begValue);
            var eValue = ConvertJson<T>(endValue);
            var rValue = Do(redis => redis.SortedSetRangeByValue(key, bValue, eValue, (Exclude)rangeType));
            return ConvetList<T>(rValue);
        }

        /// <summary>
        /// 先根据分数Scores排序，再根据value进行排序，然后获取索引从startIndex到stopIndex的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="stopIndex">结束索引，-1表示到结束，0为1条</param>
        /// <param name="order">元素的排序规则</param>
        /// <returns></returns>
        public List<T> SortedSetRangeByLexical<T>(string key, long startIndex = 0, long stopIndex = -1, LexicalOrder order = LexicalOrder.Ascending)
        {
            key = AddSysCustomKey(key);
            Order orderBy = (Order)order;
            var rValue = Do(redis => redis.SortedSetRangeByRank(key, startIndex, stopIndex, orderBy));
            return ConvetList<T>(rValue);
        }

        /// <summary>
        /// 先根据分数Scores排序，再根据value进行排序，然后获取索引从startIndex到stopIndex的数据，数据 包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="stopIndex">结束索引，-1表示到结束，0为1条</param>
        /// <param name="desc">是否按降序排列</param>
        /// <returns></returns>
        public Dictionary<T, double> SortedSetRangeByRankWithScores<T>(string key, long startIndex = 0, long stopIndex = -1, LexicalOrder order = LexicalOrder.Ascending)
        {
            key = AddSysCustomKey(key);
            Order orderBy = (Order)order;
            var rValue = Do(redis => redis.SortedSetRangeByRankWithScores(key, startIndex, stopIndex, orderBy));
            Dictionary<T, double> dicList = new Dictionary<T, double>();
            foreach (var item in rValue)
            {
                dicList.Add(ConvertObj<T>(item.Element), item.Score);
            }
            return dicList;
        }

        /// <summary>
        /// 获取分数（Score）从 minScore 开始的 maxScore 的数据，然后根据分数Scores排序，再根据value进行排序
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="minScore">最小分数</param>
        /// <param name="maxScore">最大分数</param>
        /// <param name="order">分数Score与值value的排序规则</param>
        /// <returns></returns>
        public List<T> SortedSetRangeByScore<T>(string key, double minScore = double.NegativeInfinity, double maxScore = double.PositiveInfinity, LexicalOrder order = LexicalOrder.Ascending, RangeType rangeType = RangeType.None)
        {
            key = AddSysCustomKey(key);
            Order orderBy = (Order)order;
            Exclude exclude = (Exclude)rangeType;
            var rValue = Do(redis => redis.SortedSetRangeByScore(key, minScore, maxScore, exclude, orderBy));
            return ConvetList<T>(rValue);
        }

        /// <summary>
        /// 根据Score排序 获取分数（Score）从 minScore 开始的 maxScore 的数据，数据包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="minScore">最小分数</param>
        /// <param name="maxScore">最大分数</param>
        /// <param name="desc">分数Score与值value的排序规则</param>
        /// <returns></returns>
        public Dictionary<T, double> SortedSetRangeByScoreWithScores<T>(string key, double minScore = double.NegativeInfinity, double maxScore = double.PositiveInfinity, LexicalOrder order = LexicalOrder.Ascending, RangeType rangeType = RangeType.None)
        {
            key = AddSysCustomKey(key);
            Order orderBy = (Order)order;
            Exclude exclude = (Exclude)rangeType;
            var rValue = Do(redis => redis.SortedSetRangeByScoreWithScores(key, minScore, maxScore, exclude, orderBy));
            Dictionary<T, double> dicList = new Dictionary<T, double>();
            foreach (var item in rValue)
            {
                dicList.Add(ConvertObj<T>(item.Element), item.Score);
            }
            return dicList;
        }

        /// <summary>
        /// 获取集合中数据的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SortedSetLength(key));
        }

        /// <summary>
        /// 获取值（Score）从 begValue 到 endValue 的数据个数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="begValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <param name="rangeType">开闭区间控制</param>
        /// <returns></returns>
        public long SortedSetLengthByValue<T>(string key, T begValue, T endValue, RangeType rangeType = RangeType.None)
        {
            key = AddSysCustomKey(key);
            var sValue = ConvertJson<T>(begValue);
            var eValue = ConvertJson<T>(endValue);
            return Do(redis => redis.SortedSetLengthByValue(key, sValue, eValue, (Exclude)rangeType));
        }

        /// <summary>
        /// 获取集合中值为value的分数Score值
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public double? SortedSetScore<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            var rValue = ConvertJson<T>(value);
            return Do(redis => redis.SortedSetScore(key, rValue));
        }

        /// <summary>
        /// 获取指定Key中最小分数（Score）值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public double SortedSetMinScore(string key)
        {
            key = AddSysCustomKey(key);
            double dValue = 0;
            var rValue = Do(redis => redis.SortedSetRangeByRankWithScores(key, 0, 0, Order.Ascending)).FirstOrDefault();
            dValue = rValue != null ? rValue.Score : 0;
            return dValue;
        }

        /// <summary>
        /// 获取指定Key中最大分数（Score）值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public double SortedSetMaxScore(string key)
        {
            key = AddSysCustomKey(key);
            double dValue = 0;
            var rValue = Do(redis => redis.SortedSetRangeByRankWithScores(key, 0, 0, Order.Descending)).FirstOrDefault();
            dValue = rValue != null ? rValue.Score : 0;
            return dValue;
        }

        /// <summary>
        /// 删除Key中指定的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public long SortedSetRemove<T>(string key, IEnumerable<T> valueList)
        {
            key = AddSysCustomKey(key);
            var values = valueList.Select(i => (RedisValue)ConvertJson<T>(i)).ToArray();
            return Do(redis => redis.SortedSetRemove(key, values));
        }

        /// <summary>
        /// 删除key中值为 value 的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public bool SortedSetRemove<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SortedSetRemove(key, ConvertJson(value)));
        }

        /// <summary>
        /// 删除key中指定起始值（begValue）到结束值（endValue）的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="begValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <returns></returns>
        public long SortedSetRemoveRangeByValue<T>(string key, T begValue, T endValue)
        {
            key = AddSysCustomKey(key);
            var sValue = ConvertJson<T>(begValue);
            var eValue = ConvertJson<T>(endValue);
            return Do(redis => redis.SortedSetRemoveRangeByValue(key, sValue, eValue));
        }

        /// <summary>
        /// 根据分数（Score）排序，删除索引从 startIndex 开始到 stopIndex 的数据（闭区间）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="startIndex">开始索引</param>
        /// <param name="stopIndex">结束索引</param>
        /// <returns></returns>
        public long SortedSetRemoveRangeByRank(string key, long startIndex, long stopIndex)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SortedSetRemoveRangeByRank(key, startIndex, stopIndex));
        }

        /// <summary>
        /// 根据分数（Score）排序，删除分数（Score）从 scoreStart 开始到 scoreStop 的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="scoreStart">开始分数</param>
        /// <param name="scoreStop">结束分数</param>
        /// <returns></returns>
        public long SortedSetRemoveRangeByScore(string key, double scoreStart, double scoreStop)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.SortedSetRemoveRangeByScore(key, scoreStart, scoreStop));
        }

        /// <summary>
        /// 为有序集key的成员 value 的分数（score）值加上增量score
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="increment">增量</param>
        /// <returns></returns>
        public double SortedSetIncrement<T>(string key, T value, double increment)
        {
            key = AddSysCustomKey(key);
            var strValue = ConvertJson<T>(value);
            return Do(redis => redis.SortedSetIncrement(key, strValue, increment));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 添加一个数据到Key
        /// </summary>
        /// <typeparam name="T">要添加的数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="score">分数</param>
        /// <returns></returns>
        public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SortedSetAddAsync(key, ConvertJson<T>(value), score));
        }

        /// <summary>
        /// 添加一个集合到Key
        /// </summary>
        /// <typeparam name="T">要添加的数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="valueMapScore">值与分数的对照集合</param>
        /// <returns></returns>
        public async Task<long> SortedSetAddAsync<T>(string key, IEnumerable<KeyValuePair<T, double?>> valueMapScore)
        {
            key = AddSysCustomKey(key);
            SortedSetEntry[] rValue = valueMapScore.Select(o => new SortedSetEntry(ConvertJson<T>(o.Key), o.Value ?? 0)).ToArray();
            return await Do(redis => redis.SortedSetAddAsync(key, rValue));
        }

        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            var values = await Do(redis => redis.SortedSetRangeByRankAsync(key));
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 获取指定起始值到结束值的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="begValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <param name="rangeType">开闭区间控制</param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetRangeByValueAsync<T>(string key, T begValue, T endValue, RangeType rangeType = RangeType.None)
        {
            key = AddSysCustomKey(key);
            var bValue = ConvertJson<T>(begValue);
            var eValue = ConvertJson<T>(endValue);
            var rValue = await Do(redis => redis.SortedSetRangeByValueAsync(key, bValue, eValue, (Exclude)rangeType));
            return ConvetList<T>(rValue);
        }

        /// <summary>
        /// 先根据分数Scores排序，再根据value进行排序，然后获取索引从startIndex到stopIndex的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="stopIndex">结束索引，-1表示到结束，0为1条</param>
        /// <param name="order">元素的排序规则</param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetRangeByLexicalAsync<T>(string key, long startIndex = 0, long stopIndex = -1, LexicalOrder order = LexicalOrder.Ascending)
        {
            key = AddSysCustomKey(key);
            Order orderBy = (Order)order;
            var rValue = await Do(redis => redis.SortedSetRangeByRankAsync(key, startIndex, stopIndex, orderBy));
            return ConvetList<T>(rValue);
        }

        /// <summary>
        /// 先根据分数Scores排序，再根据value进行排序，然后获取索引从startIndex到stopIndex的数据，数据 包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="stopIndex">结束索引，-1表示到结束，0为1条</param>
        /// <param name="desc">是否按降序排列</param>
        /// <returns></returns>
        public async Task<Dictionary<T, double>> SortedSetRangeByRankWithScoresAsync<T>(string key, long startIndex = 0, long stopIndex = -1, LexicalOrder order = LexicalOrder.Ascending)
        {
            key = AddSysCustomKey(key);
            Order orderBy = (Order)order;
            var rValue = await Do(redis => redis.SortedSetRangeByRankWithScoresAsync(key, startIndex, stopIndex, orderBy));
            Dictionary<T, double> dicList = new Dictionary<T, double>();
            foreach (var item in rValue)
            {
                dicList.Add(ConvertObj<T>(item.Element), item.Score);
            }
            return dicList;
        }

        /// <summary>
        /// 获取分数（Score）从 minScore 开始的 maxScore 的数据，然后根据分数Scores排序，再根据value进行排序
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="minScore">最小分数</param>
        /// <param name="maxScore">最大分数</param>
        /// <param name="order">分数Score与值value的排序规则</param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetRangeByScoreAsync<T>(string key, double minScore = double.NegativeInfinity, double maxScore = double.PositiveInfinity, LexicalOrder order = LexicalOrder.Ascending, RangeType rangeType = RangeType.None)
        {
            key = AddSysCustomKey(key);
            Order orderBy = (Order)order;
            Exclude exclude = (Exclude)rangeType;
            var rValue = await Do(redis => redis.SortedSetRangeByScoreAsync(key, minScore, maxScore, exclude, orderBy));
            return ConvetList<T>(rValue);
        }

        /// <summary>
        /// 根据Score排序 获取分数（Score）从 minScore 开始的 maxScore 的数据，数据包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="minScore">最小分数</param>
        /// <param name="maxScore">最大分数</param>
        /// <param name="desc">分数Score与值value的排序规则</param>
        /// <returns></returns>
        public async Task<Dictionary<T, double>> SortedSetRangeByScoreWithScoresAsync<T>(string key, double minScore = double.NegativeInfinity, double maxScore = double.PositiveInfinity, LexicalOrder order = LexicalOrder.Ascending, RangeType rangeType = RangeType.None)
        {
            key = AddSysCustomKey(key);
            Order orderBy = (Order)order;
            Exclude exclude = (Exclude)rangeType;
            var rValue = await Do(redis => redis.SortedSetRangeByScoreWithScoresAsync(key, minScore, maxScore, exclude, orderBy));
            Dictionary<T, double> dicList = new Dictionary<T, double>();
            foreach (var item in rValue)
            {
                dicList.Add(ConvertObj<T>(item.Element), item.Score);
            }
            return dicList;
        }

        /// <summary>
        /// 获取集合中数据的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> SortedSetLengthAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SortedSetLengthAsync(key));
        }

        /// <summary>
        /// 获取值（Score）从 begValue 到 endValue 的数据个数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="begValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <param name="rangeType">开闭区间控制</param>
        /// <returns></returns>
        public async Task<long> SortedSetLengthByValueAsync<T>(string key, T begValue, T endValue, RangeType rangeType = RangeType.None)
        {
            key = AddSysCustomKey(key);
            var sValue = ConvertJson<T>(begValue);
            var eValue = ConvertJson<T>(endValue);
            return await Do(redis => redis.SortedSetLengthByValueAsync(key, sValue, eValue, (Exclude)rangeType));
        }

        /// <summary>
        /// 获取集合中值为value的分数Score值
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<double?> SortedSetScoreAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            var rValue = ConvertJson<T>(value);
            return await Do(redis => redis.SortedSetScoreAsync(key, rValue));
        }

        /// <summary>
        /// 获取指定Key中最小分数（Score）值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<double> SortedSetMinScoreAsync(string key)
        {
            key = AddSysCustomKey(key);
            double dValue = 0;
            var rValue = await Do(redis => redis.SortedSetRangeByRankWithScoresAsync(key, 0, 0, Order.Ascending));
            var value = rValue.FirstOrDefault();
            dValue = value != null ? value.Score : 0;
            return dValue;
        }

        /// <summary>
        /// 获取指定Key中最大分数（Score）值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<double> SortedSetMaxScoreAsync(string key)
        {
            key = AddSysCustomKey(key);
            double dValue = 0;
            var rValue = await Do(redis => redis.SortedSetRangeByRankWithScoresAsync(key, 0, 0, Order.Descending));
            var value = rValue.FirstOrDefault();
            dValue = value != null ? value.Score : 0;
            return dValue;
        }

        /// <summary>
        /// 删除Key中指定的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public async Task<long> SortedSetRemoveAsync<T>(string key, IEnumerable<T> valueList)
        {
            key = AddSysCustomKey(key);
            var values = valueList.Select(i => (RedisValue)ConvertJson<T>(i)).ToArray();
            return await Do(redis => redis.SortedSetRemoveAsync(key, values));
        }

        /// <summary>
        /// 删除key中值为 value 的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public async Task<bool> SortedSetRemoveAsync<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SortedSetRemoveAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 删除key中指定起始值（begValue）到结束值（endValue）的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="begValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <returns></returns>
        public async Task<long> SortedSetRemoveRangeByValueAsync<T>(string key, T begValue, T endValue)
        {
            key = AddSysCustomKey(key);
            var sValue = ConvertJson<T>(begValue);
            var eValue = ConvertJson<T>(endValue);
            return await Do(redis => redis.SortedSetRemoveRangeByValueAsync(key, sValue, eValue));
        }

        /// <summary>
        /// 根据分数（Score）排序，删除索引从 startIndex 开始到 stopIndex 的数据（闭区间）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="startIndex">开始索引</param>
        /// <param name="stopIndex">结束索引</param>
        /// <returns></returns>
        public async Task<long> SortedSetRemoveRangeByRankAsync(string key, long startIndex, long stopIndex)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SortedSetRemoveRangeByRankAsync(key, startIndex, stopIndex));
        }

        /// <summary>
        /// 根据分数（Score）排序，删除分数（Score）从 scoreStart 开始到 scoreStop 的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="scoreStart">开始分数</param>
        /// <param name="scoreStop">结束分数</param>
        /// <returns></returns>
        public async Task<long> SortedSetRemoveRangeByScoreAsync(string key, double scoreStart, double scoreStop)
        {
            key = AddSysCustomKey(key);
            return await Do(redis => redis.SortedSetRemoveRangeByScoreAsync(key, scoreStart, scoreStop));
        }

        /// <summary>
        /// 为有序集key的成员 value 的分数（score）值加上增量score
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="increment">增量</param>
        /// <returns></returns>
        public async Task<double> SortedSetIncrementAsync<T>(string key, T value, double increment)
        {
            key = AddSysCustomKey(key);
            var strValue = ConvertJson<T>(value);
            return await Do(redis => redis.SortedSetIncrementAsync(key, strValue, increment));
        }

        #endregion 异步方法

        #endregion SortedSet 有序集合

        #region key 管理

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public bool KeyDelete(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.KeyDelete(key));
        }

        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        public long KeyDelete(IEnumerable<string> keys)
        {
            List<string> newKeys = keys.Select(AddSysCustomKey).ToList();
            return Do(db => db.KeyDelete(ConvertRedisKeys(newKeys)));
        }

        /// <summary>
        /// 判断key是否存储
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.KeyExists(key));
        }

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public bool KeyRename(string key, string newKey)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.KeyRename(key, newKey));
        }

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool KeyExpire(string key, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            return Do(db => db.KeyExpire(key, expiry));
        }

        #endregion key 管理

        #region 发布订阅

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, Action<string, string> handler = null)
        {
            ISubscriber sub = conn.GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            });
        }

        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public long Publish<T>(string channel, T msg)
        {
            ISubscriber sub = conn.GetSubscriber();
            return sub.Publish(channel, ConvertJson(msg));
        }

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            ISubscriber sub = conn.GetSubscriber();
            sub.Unsubscribe(channel);
        }

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            ISubscriber sub = conn.GetSubscriber();
            sub.UnsubscribeAll();
        }

        #endregion 发布订阅

        #region 其他

        public ITransaction CreateTransaction()
        {
            return GetDatabase().CreateTransaction();
        }

        public IDatabase GetDatabase()
        {
            return conn.GetDatabase(dbNum);
        }

        public IServer GetServer(string hostAndPort)
        {
            return conn.GetServer(hostAndPort);
        }

        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="customKey"></param>
        public void SetSysCustomKey(string customKey)
        {
            SysCustomKey = customKey;
        }

        #endregion 其他
    }

}

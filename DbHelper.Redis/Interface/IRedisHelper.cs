using DbHelper.Redis.CommEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbHelper.Redis.Interface
{
    interface IRedisHelper
    {

        #region String 操作

        #region 同步方法

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        bool StringSet(string key, string value, TimeSpan? expiry = default(TimeSpan?));

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        bool StringSet<T>(IEnumerable<KeyValuePair<string, T>> keyValues);

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        bool StringSet<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?));

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        string StringGet(string key);

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        List<T> StringGet<T>(IEnumerable<string> listKey);

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T StringGet<T>(string key);

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        double StringIncrement(string key, double val = 1);

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        double StringDecrement(string key, double val = 1);

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = default(TimeSpan?));

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        Task<bool> StringSetAsync<T>(IEnumerable<KeyValuePair<string, T>> keyValues);

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?));

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        Task<string> StringGetAsync(string key);

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        Task<List<T>> StringGetAsync<T>(IEnumerable<string> listKey);

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> StringGetAsync<T>(string key);

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        Task<double> StringIncrementAsync(string key, double val = 1);

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        Task<double> StringDecrementAsync(string key, double val = 1);

        #endregion 异步方法

        #endregion String 操作

        #region List 操作

        #region 同步方法

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void ListRemove<T>(string key, T value);

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> ListRange<T>(string key);

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void ListRightPush<T>(string key, T value);

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T ListRightPop<T>(string key);

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void ListLeftPush<T>(string key, T value);

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T ListLeftPop<T>(string key);

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long ListLength(string key);

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task<long> ListRemoveAsync<T>(string key, T value);

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<T>> ListRangeAsync<T>(string key);

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task<long> ListRightPushAsync<T>(string key, T value);

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> ListRightPopAsync<T>(string key);

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task<long> ListLeftPushAsync<T>(string key, T value);

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> ListLeftPopAsync<T>(string key);

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> ListLengthAsync(string key);

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
        bool HashExists(string key, string dataKey);

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        bool HashSet<T>(string key, string dataKey, T t);

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        bool HashDelete(string key, string dataKey);

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        long HashDelete<T>(string key, IEnumerable<T> dataKeys);

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        T HashGet<T>(string key, string dataKey);

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        double HashIncrement(string key, string dataKey, double val = 1);

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        double HashDecrement(string key, string dataKey, double val = 1);

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> HashKeys<T>(string key);

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        Task<bool> HashExistsAsync(string key, string dataKey);

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<bool> HashSetAsync<T>(string key, string dataKey, T t);

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        Task<bool> HashDeleteAsync(string key, string dataKey);

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        Task<long> HashDeleteAsync<T>(string key, IEnumerable<T> dataKeys);

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        Task<T> HashGeAsync<T>(string key, string dataKey);

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        Task<double> HashIncrementAsync(string key, string dataKey, double val = 1);

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        Task<double> HashDecrementAsync(string key, string dataKey, double val = 1);

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<T>> HashKeysAsync<T>(string key);

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
        bool SetAdd<T>(string key, T value, double score);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        bool SetRemove<T>(string key, T value);

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        bool SetContains<T>(string key, T value);

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> SetMembers<T>(string key);

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long SetLength(string key);

        /// <summary>
        /// 获取多个集合的交集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        List<T> SetIntersect<T>(IEnumerable<string> keyList);

        /// <summary>
        /// 获取多个集合的并集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        List<T> SetUnion<T>(IEnumerable<string> keyList);

        /// <summary>
        /// 获取多个集合的差集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        List<T> SetDifference<T>(IEnumerable<string> keyList);

        /// <summary>
        /// 获取多个集合的差集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        List<T> SetCombine<T>(IEnumerable<string> keyList, SetCombineOperation operation);

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task<bool> SetAddAsync<T>(string key, T value);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task<bool> SetRemoveAsync<T>(string key, T value);

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task<bool> SetContainsAsync<T>(string key, T value);

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<T>> SetMembersAsync<T>(string key);

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> SetLengthAsync(string key);

        /// <summary>
        /// 获取多个集合的交集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        Task<List<T>> SetIntersectAsync<T>(IEnumerable<string> keyList);

        /// <summary>
        /// 获取多个集合的并集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        Task<List<T>> SetUnionAsync<T>(IEnumerable<string> keyList);

        /// <summary>
        /// 获取多个集合的差集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        Task<List<T>> SetDifferenceAsync<T>(IEnumerable<string> keyList);

        /// <summary>
        /// 获取多个集合的并集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyList"></param>
        /// <returns></returns>
        Task<List<T>> SetCombineAsync<T>(IEnumerable<string> keyList, SetCombineOperation operation);

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
        bool SortedSetAdd<T>(string key, T value, double score);

        /// <summary>
        /// 添加一个集合到Key
        /// </summary>
        /// <typeparam name="T">要添加的数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="valueMapScore">值与分数的对照集合</param>
        /// <returns></returns>
        long SortedSetAdd<T>(string key, IEnumerable<KeyValuePair<T, double?>> valueMapScore);

        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        List<T> SortedSetRangeByRank<T>(string key);

        /// <summary>
        /// 获取指定起始值到结束值的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="begValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <param name="rangeType">开闭区间控制</param>
        /// <returns></returns>
        List<T> SortedSetRangeByValue<T>(string key, T begValue, T endValue, RangeType rangeType = RangeType.None);

        /// <summary>
        /// 先根据分数Scores排序，再根据value进行排序，然后获取索引从startIndex到stopIndex的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="stopIndex">结束索引，-1表示到结束，0为1条</param>
        /// <param name="order">元素的排序规则</param>
        /// <returns></returns>
        List<T> SortedSetRangeByLexical<T>(string key, long startIndex = 0, long stopIndex = -1, LexicalOrder order = LexicalOrder.Ascending);

        /// <summary>
        /// 先根据分数Scores排序，再根据value进行排序，然后获取索引从startIndex到stopIndex的数据，数据 包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="stopIndex">结束索引，-1表示到结束，0为1条</param>
        /// <param name="desc">是否按降序排列</param>
        /// <returns></returns>
        Dictionary<T, double> SortedSetRangeByRankWithScores<T>(string key, long startIndex = 0, long stopIndex = -1, LexicalOrder order = LexicalOrder.Ascending);

        /// <summary>
        /// 获取分数（Score）从 minScore 开始的 maxScore 的数据，然后根据分数Scores排序，再根据value进行排序
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="minScore">最小分数</param>
        /// <param name="maxScore">最大分数</param>
        /// <param name="order">分数Score与值value的排序规则</param>
        /// <returns></returns>
        List<T> SortedSetRangeByScore<T>(string key, double minScore = double.NegativeInfinity, double maxScore = double.PositiveInfinity, LexicalOrder order = LexicalOrder.Ascending, RangeType rangeType = RangeType.None);

        /// <summary>
        /// 根据Score排序 获取分数（Score）从 minScore 开始的 maxScore 的数据，数据包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="minScore">最小分数</param>
        /// <param name="maxScore">最大分数</param>
        /// <param name="desc">分数Score与值value的排序规则</param>
        /// <returns></returns>
        Dictionary<T, double> SortedSetRangeByScoreWithScores<T>(string key, double minScore = double.NegativeInfinity, double maxScore = double.PositiveInfinity, LexicalOrder order = LexicalOrder.Ascending, RangeType rangeType = RangeType.None);

        /// <summary>
        /// 获取集合中数据的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long SortedSetLength(string key);

        /// <summary>
        /// 获取值（Score）从 begValue 到 endValue 的数据个数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="begValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <param name="rangeType">开闭区间控制</param>
        /// <returns></returns>
        long SortedSetLengthByValue<T>(string key, T begValue, T endValue, RangeType rangeType = RangeType.None);

        /// <summary>
        /// 获取集合中值为value的分数Score值
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        double? SortedSetScore<T>(string key, T value);

        /// <summary>
        /// 获取指定Key中最小分数（Score）值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        double SortedSetMinScore(string key);

        /// <summary>
        /// 获取指定Key中最大分数（Score）值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        double SortedSetMaxScore(string key);

        /// <summary>
        /// 删除Key中指定的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        long SortedSetRemove<T>(string key, IEnumerable<T> valueList);

        /// <summary>
        /// 删除key中值为 value 的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        bool SortedSetRemove<T>(string key, T value);

        /// <summary>
        /// 删除key中指定起始值（begValue）到结束值（endValue）的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="begValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <returns></returns>
        long SortedSetRemoveRangeByValue<T>(string key, T begValue, T endValue);

        /// <summary>
        /// 根据分数（Score）排序，删除索引从 startIndex 开始到 stopIndex 的数据（闭区间）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="startIndex">开始索引</param>
        /// <param name="stopIndex">结束索引</param>
        /// <returns></returns>
        long SortedSetRemoveRangeByRank(string key, long startIndex, long stopIndex);

        /// <summary>
        /// 根据分数（Score）排序，删除分数（Score）从 scoreStart 开始到 scoreStop 的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="scoreStart">开始分数</param>
        /// <param name="scoreStop">结束分数</param>
        /// <returns></returns>
        long SortedSetRemoveRangeByScore(string key, double scoreStart, double scoreStop);

        /// <summary>
        /// 为有序集key的成员 value 的分数（score）值加上增量score
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="increment">增量</param>
        /// <returns></returns>
        double SortedSetIncrement<T>(string key, T value, double increment);

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
        Task<bool> SortedSetAddAsync<T>(string key, T value, double score);

        /// <summary>
        /// 添加一个集合到Key
        /// </summary>
        /// <typeparam name="T">要添加的数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="valueMapScore">值与分数的对照集合</param>
        /// <returns></returns>
        Task<long> SortedSetAddAsync<T>(string key, IEnumerable<KeyValuePair<T, double?>> valueMapScore);

        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<List<T>> SortedSetRangeByRankAsync<T>(string key);

        /// <summary>
        /// 获取指定起始值到结束值的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="begValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <param name="rangeType">开闭区间控制</param>
        /// <returns></returns>
        Task<List<T>> SortedSetRangeByValueAsync<T>(string key, T begValue, T endValue, RangeType rangeType = RangeType.None);

        /// <summary>
        /// 先根据分数Scores排序，再根据value进行排序，然后获取索引从startIndex到stopIndex的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="stopIndex">结束索引，-1表示到结束，0为1条</param>
        /// <param name="order">元素的排序规则</param>
        /// <returns></returns>
        Task<List<T>> SortedSetRangeByLexicalAsync<T>(string key, long startIndex = 0, long stopIndex = -1, LexicalOrder order = LexicalOrder.Ascending);

        /// <summary>
        /// 先根据分数Scores排序，再根据value进行排序，然后获取索引从startIndex到stopIndex的数据，数据 包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="stopIndex">结束索引，-1表示到结束，0为1条</param>
        /// <param name="desc">是否按降序排列</param>
        /// <returns></returns>
        Task<Dictionary<T, double>> SortedSetRangeByRankWithScoresAsync<T>(string key, long startIndex = 0, long stopIndex = -1, LexicalOrder order = LexicalOrder.Ascending);

        /// <summary>
        /// 获取分数（Score）从 minScore 开始的 maxScore 的数据，然后根据分数Scores排序，再根据value进行排序
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="minScore">最小分数</param>
        /// <param name="maxScore">最大分数</param>
        /// <param name="order">分数Score与值value的排序规则</param>
        /// <returns></returns>
        Task<List<T>> SortedSetRangeByScoreAsync<T>(string key, double minScore = double.NegativeInfinity, double maxScore = double.PositiveInfinity, LexicalOrder order = LexicalOrder.Ascending, RangeType rangeType = RangeType.None);

        /// <summary>
        /// 根据Score排序 获取分数（Score）从 minScore 开始的 maxScore 的数据，数据包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="minScore">最小分数</param>
        /// <param name="maxScore">最大分数</param>
        /// <param name="desc">分数Score与值value的排序规则</param>
        /// <returns></returns>
        Task<Dictionary<T, double>> SortedSetRangeByScoreWithScoresAsync<T>(string key, double minScore = double.NegativeInfinity, double maxScore = double.PositiveInfinity, LexicalOrder order = LexicalOrder.Ascending, RangeType rangeType = RangeType.None);

        /// <summary>
        /// 获取集合中数据的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> SortedSetLengthAsync(string key);

        /// <summary>
        /// 获取值（Score）从 begValue 到 endValue 的数据个数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="begValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <param name="rangeType">开闭区间控制</param>
        /// <returns></returns>
        Task<long> SortedSetLengthByValueAsync<T>(string key, T begValue, T endValue, RangeType rangeType = RangeType.None);

        /// <summary>
        /// 获取集合中值为value的分数Score值
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<double?> SortedSetScoreAsync<T>(string key, T value);

        /// <summary>
        /// 获取指定Key中最小分数（Score）值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<double> SortedSetMinScoreAsync(string key);

        /// <summary>
        /// 获取指定Key中最大分数（Score）值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<double> SortedSetMaxScoreAsync(string key);

        /// <summary>
        /// 删除Key中指定的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        Task<long> SortedSetRemoveAsync<T>(string key, IEnumerable<T> valueList);

        /// <summary>
        /// 删除key中值为 value 的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        Task<bool> SortedSetRemoveAsync<T>(string key, T value);

        /// <summary>
        /// 删除key中指定起始值（begValue）到结束值（endValue）的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="begValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <returns></returns>
        Task<long> SortedSetRemoveRangeByValueAsync<T>(string key, T begValue, T endValue);

        /// <summary>
        /// 根据分数（Score）排序，删除索引从 startIndex 开始到 stopIndex 的数据（闭区间）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="startIndex">开始索引</param>
        /// <param name="stopIndex">结束索引</param>
        /// <returns></returns>
        Task<long> SortedSetRemoveRangeByRankAsync(string key, long startIndex, long stopIndex);

        /// <summary>
        /// 根据分数（Score）排序，删除分数（Score）从 scoreStart 开始到 scoreStop 的数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="scoreStart">开始分数</param>
        /// <param name="scoreStop">结束分数</param>
        /// <returns></returns>
        Task<long> SortedSetRemoveRangeByScoreAsync(string key, double scoreStart, double scoreStop);

        /// <summary>
        /// 为有序集key的成员 value 的分数（score）值加上增量score
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="increment">增量</param>
        /// <returns></returns>
        Task<double> SortedSetIncrementAsync<T>(string key, T value, double increment);

        #endregion 异步方法

        #endregion SortedSet 有序集合

        #region key 管理

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        bool KeyDelete(string key);

        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        long KeyDelete(IEnumerable<string> keys);

        /// <summary>
        /// 判断key是否存储
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        bool KeyExists(string key);

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        bool KeyRename(string key, string newKey);

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        bool KeyExpire(string key, TimeSpan? expiry = default(TimeSpan?));

        #endregion key 管理

        #region 发布订阅

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        void Subscribe(string subChannel, Action<string, string> handler = null);

        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        long Publish<T>(string channel, T msg);

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        void Unsubscribe(string channel);

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        void UnsubscribeAll();

        #endregion 发布订阅

        #region 其他

        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="customKey"></param>
        void SetSysCustomKey(string customKey);

        #endregion 其他
    }

}

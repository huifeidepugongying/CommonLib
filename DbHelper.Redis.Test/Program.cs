using DbHelper.Redis.Interface;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace DbHelper.Redis.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*****************UnityContainer*********************");
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "ConfigFiles\\Unity.Config");//找配置文件的路径
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                UnityConfigurationSection section = (UnityConfigurationSection)configuration.GetSection(UnityConfigurationSection.SectionName);

                IUnityContainer container = new UnityContainer();
                section.Configure(container, "iocTest");
                IRedisHelper redis = container.Resolve<IRedisHelper>();
                IRedisHelper redis1 = container.Resolve<IRedisHelper>();
                Console.WriteLine(redis.Equals(redis1));
                redis.StringSet("name", "你是谁");
                string name = redis.StringGet("name");
                Console.WriteLine(name);
                Console.ReadKey();
            }
        }
    }
}

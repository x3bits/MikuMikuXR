using System.ComponentModel;
using System.IO;
using System.Text;
using MikuMikuXR.Utils;
using Newtonsoft.Json;

namespace MikuMikuXR.UserConfig.Resource
{
    public class ResourceListStore
    {
        public static readonly ResourceListStore Instance = new ResourceListStore();

        private ResourceListStore()
        {
        }

        public AllResources Load(string path)
        {
            var text = File.ReadAllText(path, Encoding.UTF8);
            return JsonConvert.DeserializeObject<AllResources>(text);
        }

        public void Save(string path, AllResources resourceList)
        {
            var text = JsonConvert.SerializeObject(resourceList);
            FileUtils.WriteTextFile(text, path, Encoding.UTF8);
        }
        
    }
}
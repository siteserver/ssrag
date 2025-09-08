using SSRAG.Datory;
using SSRAG.Datory.Annotations;

namespace SSRAG.Models
{
    [DataTable("ssrag_plugin_config")]
    public class PluginConfig : Entity
    {
        [DataColumn]
        public string PluginId { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string ConfigName { get; set; }

        [DataColumn(Text = true)]
        public string ConfigValue { get; set; }
    }
}

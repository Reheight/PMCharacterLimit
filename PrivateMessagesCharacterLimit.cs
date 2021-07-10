using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Plugins
{
    [Info("PrivateMessagesCharacterLimit", "Reheight", "1.0.0")]
    [Description("Limits the amount of characters allowed in a private message.")]
    class PrivateMessagesCharacterLimit : CovalencePlugin
    {
        PluginConfig _config;

        private void Init()
        {
            _config = Config.ReadObject<PluginConfig>();
        }

        protected override void LoadDefaultConfig() => _config = GetDefaultConfig();

        protected override void LoadConfig()
        {
            base.LoadConfig();

            try
            {
                _config = Config.ReadObject<PluginConfig>();

                if (_config == null)
                {
                    throw new JsonException();
                }

                if (!_config.ToDictionary().Keys.SequenceEqual(Config.ToDictionary(x => x.Key, x => x.Value).Keys))
                {
                    PrintWarning($"PluginConfig file {Name}.json updated.");

                    SaveConfig();
                }
            }
            catch
            {
                LoadDefaultConfig();

                PrintError("Config file contains an error and has been replaced with the default file.");
            }

        }

        protected override void SaveConfig() => Config.WriteObject(_config, true);

        private class PluginConfig
        {
            [JsonProperty(PropertyName = "Prefix", Order = 0)]
            public string Prefix { get; set; }

            [JsonProperty(PropertyName = "Chat Icon", Order = 1)]
            public int ChatIcon { get; set; }

            [JsonProperty(PropertyName = "Character Limit", Order = 2)]
            public int CharacterLimit { get; set; }

            public string ToJson() => JsonConvert.SerializeObject(this);

            public Dictionary<string, object> ToDictionary() => JsonConvert.DeserializeObject<Dictionary<string, object>>(ToJson());
        }

        private PluginConfig GetDefaultConfig()
        {
            return new PluginConfig
            {
                Prefix = "<color=#42f566>PM Limit:</color> ",
                ChatIcon = 0,
                CharacterLimit = 300
            };
        }
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["MessageTooLarge"] = "<size=12><color=#b6bab7>The private message you attempted to send is too large!</color></size>"
            }, this);
        }

        private string Lang(string key, params object[] args) => _config.Prefix + String.Format(lang.GetMessage(key, this, _config.ChatIcon.ToString()), args);

        object OnPMProcessed(IPlayer sender, IPlayer target, string message)
        {
            if (message.Length > _config.CharacterLimit)
            {
                sender.Reply(Lang("MessageTooLarge"));
                return true;
            }

            return null;
        }
    }
}

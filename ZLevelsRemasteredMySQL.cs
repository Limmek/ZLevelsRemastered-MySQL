using Rust;
using System.Collections.Generic;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Core.Database;

namespace Oxide.Plugins
{
    [Info("ZLevelsRemasteredMySQL", "Limmek", "1.0.3")]
    [Description("MySQL database add-on for ZLevels Remastered")]
    class ZLevelsRemasteredMySQL : RustPlugin
    {
        [PluginReference]
        Plugin ZLevelRemastered;

        #region configuration
        public class ConfigData
        {
            public Options Options = new Options();
            public MySQL MySQL = new MySQL();
        }
        public class Options
        {
            public int saveTimer = 600;
            public bool updateAtStart;
        }

        public class MySQL
        {
            public bool useMySQL;
            public string host = "hostname";
            public int port = 3306;
            public string user = "username";
            public string pass = "password";
            public string db = "database";
            public string table = "table";
        }

		class PlayerData
		{
			public Dictionary<ulong, PlayerInfo> PlayerInfo = new Dictionary<ulong, PlayerInfo>();
			public PlayerData(){}
		}

		class PlayerInfo
		{
			public long WCL;
			public long WCP;
			public long ML;
			public long MP;
			public long SL;
			public long SP;
			public long AL;
			public long AP;
			public long CL;
			public long CP;
			public long LD;
			public long LLD;
			public long XPM;
			public bool CUI;
			public bool ONOFF;
		}
        #endregion
        
        private ConfigData conf;
        private PlayerData storedData;
        
        Core.MySql.Libraries.MySql sqlLibrary = Interface.Oxide.GetLibrary<Core.MySql.Libraries.MySql>();
        Connection sqlConnection;
        
        public void executeQuery(string query, params object[] data) {
            var sql = Sql.Builder.Append(query, data);
            sqlLibrary.Insert(sql, sqlConnection);
        }                
        
        protected override void LoadDefaultConfig()
        {
            PrintWarning("Creating new config file.");
            var config = new ConfigData();
            SaveConfig();
        }
        
        private void LoadConfigVariables()
        {
            conf = Config.ReadObject<ConfigData>();
            SaveConfig(Config.ReadObject<ConfigData>());
        }
        
        void SaveConfig(ConfigData config)=>Config.WriteObject(config, true);
        
        void Unloaded()
        {
            sqlLibrary.CloseDb(sqlConnection);
        }

        private void Init()
        {
            LoadConfigVariables();
            if (conf.MySQL.useMySQL)
            {
                sqlConnection = sqlLibrary.OpenDb(conf.MySQL.host, conf.MySQL.port, conf.MySQL.db, conf.MySQL.user, conf.MySQL.pass, this);
                executeQuery($@"CREATE TABLE IF NOT EXISTS {conf.MySQL.table} (player_id BIGINT(20) NOT NULL, 
                    WCL INT(11) NULL, 
                    WCP INT(11) NULL, 
                    ML INT(11) NULL, 
                    MP INT(11) NULL, 
                    SL INT(11) NULL, 
                    SP INT(11) NULL, 
                    AL INT(11) NULL, 
                    AP INT(11) NULL, 
                    CL INT(11) NULL, 
                    CP INT(11) NULL, 
                    LD INT(11) NULL, 
                    LLD INT(11) NULL, 
                    XPM INT(11) NULL, 
                    CUI BOOLEAN NULL, 
                    ONOFF BOOLEAN NULL,
                    PRIMARY KEY (`player_id`),
                    UNIQUE (`player_id`) ) ENGINE=InnoDB;"
                );
            }
        }

        private void OnServerInitialized()
        {
            if (conf.Options.updateAtStart && conf.MySQL.useMySQL) UpdateStatsData();
                        
            timer.Repeat((float)conf.Options.saveTimer, 0, () =>
            {
                if (conf.MySQL.useMySQL) UpdateStatsData();
            });
            
        }

        private void UpdateStatsData()
        {
            Puts("Please do not reload, or unload the plugin until save is completed.");
            storedData = Interface.Oxide.DataFileSystem.ReadObject<PlayerData>("ZLevelsRemastered");
            foreach (var item in storedData.PlayerInfo)
            {
                executeQuery($@"INSERT INTO {conf.MySQL.table} (player_id, WCL, WCP, ML, MP, SL, SP, AL, AP, CL, CP, LD, LLD, XPM, CUI, ONOFF) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15) ON DUPLICATE KEY UPDATE player_id={item.Key}, WCL={item.Value.WCL}, WCP={item.Value.WCP}, ML={item.Value.ML}, MP={item.Value.MP}, SL={item.Value.SL}, SP={item.Value.SP}, AL={item.Value.AL}, AP={item.Value.AP}, CL={item.Value.CL}, CP={item.Value.CP}, LD={item.Value.LD}, LLD={item.Value.LLD}, XPM={item.Value.XPM}, CUI={item.Value.CUI}, ONOFF={item.Value.ONOFF}",
                    item.Key,
                    item.Value.WCL,
                    item.Value.WCP,
                    item.Value.ML,
                    item.Value.MP,
                    item.Value.SL,
                    item.Value.SP,
                    item.Value.AL,
                    item.Value.AP,
                    item.Value.CL,
                    item.Value.CP,
                    item.Value.LD,
                    item.Value.LLD,
                    item.Value.XPM,
                    item.Value.CUI,
                    item.Value.ONOFF
                );         
            }
            Puts("SQL saving is complete.");
        }

    }
}
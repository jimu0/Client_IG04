//这是一个Luban向CoreSim提供的数据解析服务，规范上Unity侧不直接读取Luban配置

using System.IO;
using IGC.RPGCore_IG04;
using Mycelia;
using Luban;
using cfg;

namespace SUEngine
{
    public class LubanConfigService : IConfigService
    {
        private const string gameConfDir = "Assets/StreamingAssets/GameConfig/Bin";
        public LubanConfigService()
        {
            DeckFactory.tables = new Tables(file => new ByteBuf(File.ReadAllBytes($"{gameConfDir}/{file}.bytes")));
        }
        //Debug.LogWarning($"{DeckFactory.tables.UnitDataTable.Get(1000000001).Name}");//表数据读取示例
        //Debug.LogWarning($"{DeckFactory.tables.UnitDataTable.DataMap[1000000001].Name}");//表数据读取示例
        //Debug.LogWarning($"{MC.GetUnitConfig(1000000001).Name}");//表数据读取示例(dll封装)
        public UnitConfig GetUnit(int id)
        {
            if (DeckFactory.tables == null) return default;
            UnitDataCfg row = DeckFactory.tables.UnitDataTable.Get(id);
            return new UnitConfig
            {
                Id = row.Id,
                position = intToVec3(row.InitialPosition),
                orientation = intToVec3(row.InitialOrientation),
                scale = Vec3.One,
                //TODO: 其他数据
            };
        }

        public CardConfig GetCard(int id)
        {
            if (DeckFactory.tables == null) return default;
            CardDataCfg row = DeckFactory.tables.CardDataTable.Get(id);
            return new CardConfig
            {
                Id = row.Id,
                name = row.Name,
                desc = row.Desc,
                className = row.ClassName,
                combatPwr = row.CombatPwr,
                defensivePwr = row.DefensivePwr,
                level = row.Level
                //TODO: 其他数据
            };
        }


        private Vec3 intToVec3(int value)
        {
            int zInt = value % 1000;
            int yInt = (value / 1000) % 1000;
            int xInt = (value / 1000000) % 1000;
            return new Vec3(xInt, yInt, zInt);
        }
    }
    
}


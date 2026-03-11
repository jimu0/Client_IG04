//这是一个Luban向CoreSim提供的数据解析服务，规范上Unity侧不直接读取Luban配置

using System.Collections.Generic;
using IGC.CardCore_IG04;
using Mycelia;
using IGC.CardCore_IG04.Luban;
using IGC.CardCore_IG04.cfg;
using CardConfig = Mycelia.CardConfig;

namespace SUEngine
{
    public class LubanConfigService : IConfigService
    {
        private const string GameConfDir = "Assets/Scripts/GameConfig/Bin";
        private readonly Tables tables = DeckFactory.tables;

        public UnitConfig GetUnit(int id)
        {
            UnitDataCfg row = tables.UnitDataTable.Get(id);
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
            CardDataCfg row = tables.CardDataTable.Get(id);
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


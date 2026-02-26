//这是一个Luban向CoreSim提供的数据解析服务，规范上Unity侧不直接读取Luban配置

using IGC.Engine;
using IGC.Game;
using Luban;
using cfg;

namespace SUEngine
{
    public class LubanConfigService : IConfigService
    {
        private const string GameConfDir = "Assets/Scripts/GameConfig/Bin";
        private readonly Tables tables = new cfg.Tables(file => new ByteBuf(System.IO.File.ReadAllBytes($"{GameConfDir}/{file}.bytes")));

        public UnitConfig GetUnit(int id)
        {
            var row = tables.UnitDataTable.Get(id);
            return new UnitConfig
            {
                Id = row.Id,
                position = intToVec3(row.InitialPosition),
                orientation = intToVec3(row.InitialOrientation),
                scale = Vec3.One,
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


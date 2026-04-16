//这个脚本用于解释读表系统工作原理
//Client_IG04\Luban\DataTables\Datas中写表，每次表变更需要依次执行Client_IG04\Luban\DataTables\gen_client.bat和copy.bat
//配置中如有新增条目或变量需要更新Mycelia核心库

using IGC.RPGCore_IG04;
using Mycelia;
using UnityEngine;

namespace Jim.Test
{
    public class Test_cfg : MonoBehaviour
    {
        void Start()
        {
            //New过MC.InitTables(new LubanConfigService());后(初始化LuBan配置)
            // 通过获取IGC.RPGCore_IG04核心库的DeckFactory类读表，读表方式有以下三种：
            //LuBan插件方法
            Debug.LogWarning($"{DeckFactory.tables?.UnitDataTable.Get(1000000001).Name}");//表数据读取示例
            Debug.LogWarning($"{DeckFactory.tables?.UnitDataTable.DataMap[1000000001].Name}");//表数据读取示例
            //Mycelia自定义框架封装的读取方法。
            Debug.LogWarning($"{MC.GetUnitConfig(1000000001).Id}");//表数据读取示例(dll封装)
        }
    
    }
}


# Magic Castle Co-op｜双人魔法城堡

Unity + C# 开发的本地双人合作原型，以《双人成行》城堡地牢段落为玩法研究参考，制作原创的冰火合作冒险。

**当前已编写 M1 双人移动和 M2 冰火训练场源码。仓库提供 Unity 导入包，尚未生成完整 Unity 工程或游戏安装包；源码尚未通过 Unity 编译和实机验收。**

## 已实现的源码

- 两个手柄或键盘加手柄加入，独立设备绑定、断线暂停和设备替换。
- 双人 CharacterController 移动、碰撞、活动边界和共享俯视镜头。
- 冰角色远程命中并冻结，火角色近身命中并消费冻结触发破冰。
- 训练木桩血量、受击反馈、冻结配色、击败后重置；攻击受射程、障碍和冷却限制。
- C# 编辑器菜单生成庭院、角色、木桩、镜头、灯光及材质。
- 21 个独立 C# 核心行为检查，以及 16 个 Unity EditMode 设备和相机用例。

游戏运行逻辑、编辑器工具和测试统一使用 C#，无 Python 依赖。

## 在 Unity 中启动

1. 使用 **Unity 6.3 LTS** 创建 **Universal 3D / URP** 工程，安装兼容的 **Input System** 和 **Test Framework**。
2. 将 Active Input Handling 设为 `Input System Package (New)` 或 `Both`，按提示重启编辑器。
3. 复制本仓库 `UnityImport/Assets/MagicCastle/` 到工程的 `Assets/`，等待编译。
4. 执行菜单 **Magic Castle → Create Local Co-op Prototype**，点击 Play，点击 Game 视图获得焦点。
5. 两位玩家分别加入。第一位是冰角色，第二位是火角色；靠近绿色木桩练习冰火组合。

| 操作 | 键盘玩家 | 手柄玩家 |
| --- | --- | --- |
| 加入 | Enter | 下方主按钮：Xbox A / PS Cross |
| 移动 | WASD | 左摇杆 |
| 攻击 | Space | 左侧主按钮：Xbox X / PS Square |
| 暂停 / 继续 | Esc | Start / Menu / Options |
| 重置已击败木桩 | R | Select / View / Share，以设备映射为准 |

目前一把键盘只能加入一位玩家；第二位需要手柄。攻击自动瞄准唯一训练目标，火角色需要走到较近距离。导入细节见 [M1 操作说明](docs/M1_SETUP.md)，战斗规则见 [M2 战斗说明](docs/M2_SETUP.md)。

## 测试与当前验证状态

安装 .NET 8 SDK 后，可以在仓库根目录执行：

```sh
dotnet run --project tests/CoreChecks/CoreChecks.csproj --configuration Release
```

这个程序直接编译游戏使用的 `PlayerSlots.cs` 和 `CombatRules.cs`，不依赖测试 NuGet 包。它检查设备归属、伤害、防重复命中、冻结消费 / 过期、冷却与暂停时钟，不编译 Unity 组件。

- 已完成：源码结构、程序集 JSON、文档链接和 Git 忽略规则检查。
- 已配置 [GitHub Actions](https://github.com/star-ysp/magic-castle-coop/actions)。首次运行在获得 runner、执行步骤之前失败，没有编译或测试日志，因此 **21 个核心检查尚未证实通过**。具体失败原因未能从当前可读取的结果确定。
- 待执行：核心 C# 检查、Unity 编译、Test Runner 中 16 个 EditMode 用例、Play Mode、手柄热插拔、物理遮挡和桌面构建。

当前开发环境没有 Unity 或 .NET SDK，不将静态检查等同于编译成功。完整人工验收清单和潜在问题见 [M2 战斗说明](docs/M2_SETUP.md)。

## 文档与后续方向

- [调研与参考资料](docs/RESEARCH.md)：原作关卡定位、已核实与待核实资料、技术来源。
- [开发方案](docs/DEVELOPMENT_PLAN.md)：原创角色、房间、机关、Boss 和里程碑。
- [协作规则](CLAUDE.md)：已确认技术路线、小单元提交、缺陷复现与验证要求。

下一步先在 Unity 中验证当前训练场，再加入敌人追踪 / 攻击、玩家受伤 / 复活和第一间合作战斗房。冲刺、闪现、机关、Boss、异地联机、美术动画与音效均未实现。

首次 Unity 验证后提交编辑器实际生成的版本文件、Packages 锁定文件、ProjectSettings、场景和 `.meta` 文件。当前导入源包不冒充可由 Unity Hub 直接打开的完整工程。

原作仅用于玩法研究。本项目采用原创角色、地图和有明确使用许可的素材，不作为官方续作发布。

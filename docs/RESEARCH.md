# 双人魔法城堡：原型调研与参考资料

调研日期：2026-09-10。用途：决定项目方向、准备 GitHub 文档、制定首个可玩版本。

## 1. 当前结论与证据边界

你所说的“魔法城堡”，初步对应《双人成行》Rose’s Room（罗斯的房间）中的城堡区域，重点参考对象很可能是 **Dungeon Crawler（地牢探险家）** 的冰火战斗段落。中文视频标题有时会把城堡探索和地牢战斗统称“魔法城堡”，因此暂不把它当作范围完全一致的官方小节名称。

本次读到了 Hazelight 官方介绍、制作人访谈、引擎文档和官方示例仓库；视频资料主要取得搜索索引中的标题与描述，未能完整观看录像。下面把原作线索与新设计分开，避免把推测写成事实。

| 信息 | 当前证据 | 结论或使用方式 |
| --- | --- | --- |
| 原作定位为合作动作冒险 | Hazelight 官方产品页已读取 | 可以作为双人冒险的总体定位依据 |
| 制作时从两名不同角色与不同机制出发 | Josef Fares 的官方访谈已读取 | 新项目应先设计双人分工，再设计房间与敌人 |
| 罗斯的房间包含 Dungeon Crawler | 多个实况视频标题和描述 | 可定位到重点研究段落，中文小节范围仍需截图或录像确认 |
| 地牢段落具有火焰骑士与冰法师主题 | 多个实况标题交叉出现 Fire Knight / Ice Wizard | 可作为冰火双角色原型线索 |
| 科迪传送进栅栏区域、小梅火焰冲刺跨越缺口 | 一条实况搜索描述明确提到两者分工 | 属于具体玩法线索，尚未逐帧核实，不能据此锁定完整技能表 |
| 棋王、棋后及擀面杖玩偶遭遇 | 相关实况标题中出现 | 可列入后续观片清单，本次不宣称已核实完整战斗顺序 |
| 摄像机精确俯角、攻击距离、伤害、冷却和敌人数 | 未取得可靠参数证据 | 全部待实测；开发中的数值须作为原创调参 |

对应依据：[Hazelight 官方介绍](https://hazelight.se/games/it-takes-two)、[制作人访谈](https://www.unrealengine.com/developer-interviews/it-takes-two-lovingly-marries-story-and-gameplay-together?lang=en-US)、[Dungeon Crawler 实况索引](https://www.youtube.com/watch?v=FEE9g1rBkOc)、[冰火职业实况索引](https://www.youtube.com/watch?v=if14xu0mtkU)、[角色移动能力线索](https://www.youtube.com/watch?v=ny_daDNzP5k)、[Boss 相关录像入口](https://www.youtube.com/watch?v=AtNrlhE7fdQ)。

## 2. 最值得提炼的设计原则

制作人访谈强调从合作出发设计角色差异，并把玩法与故事相结合；同时通过测试避免玩家长时间卡关。这是比单纯延长地图更有价值的参考。[访谈原文](https://www.unrealengine.com/developer-interviews/it-takes-two-lovingly-marries-story-and-gameplay-together?lang=en-US)

下面是据此提出的新项目设计建议，不是原作机制的逐项还原。

| 设计重点 | 建议的实现方向 | 要验证的问题 |
| --- | --- | --- |
| 双人互补 | 一名角色提供控制或开启机会，另一名角色完成输出或行动 | 是否会自然产生“你先冻住，我来打”的交流？ |
| 两人都能参与 | 两人均有普通攻击、移动和自保手段；关键机关要求配合 | 是否出现某位玩家长时间等待？ |
| 能力贯穿关卡 | 同一种冰火能力同时用于战斗、机关和 Boss | 玩家能否把刚学会的能力迁移到新场景？ |
| 合作反馈 | 组合技提供独立音效、命中特效和清晰标记 | 不看伤害数字，玩家能否知道配合成功？ |
| 失败可恢复 | 房间级检查点、明确的复活规则、短时间重试 | 失败后是否能迅速继续，机关会不会卡死？ |
| 视觉清楚 | 暖色火焰、冷色冰霜，同时配合形状和图标区分 | 两人和敌人重叠时是否仍能判断危险？ |

建议先做一段完整线性冒险，再考虑随机房间、装备成长或更多职业。首版的成败标准是合作是否有趣、操作是否清晰。

## 3. 参考视频与观片清单

| 来源 | 入口 | 建议观察 |
| --- | --- | --- |
| 中文分集攻略 | [B 站：分集式教学流程](https://www.bilibili.com/video/BV1gV411J7oa/) | 索引中的“罗斯的房间·魔法城堡乐园”，确认你指的是哪一段 |
| 中文实况 | [GamePlayHK：魔法城堡](https://www.youtube.com/watch?v=LGoNzz06IJk) | 城堡氛围、两名玩家的交流、战斗与移动的切换 |
| 地牢专题 | [Dungeon Crawler Walkthrough](https://www.youtube.com/watch?v=FEE9g1rBkOc) | 地牢房间、能力教学与挑战顺序 |
| 冰火职业 | [Inside the Magic Castle: Fire Knight and Ice Wizard](https://www.youtube.com/watch?v=if14xu0mtkU) | 两种角色的攻击方式和不同职责 |
| 移动能力 | [Rose’s Room: Dungeon Crawler](https://www.youtube.com/watch?v=ny_daDNzP5k) | 传送与冲刺如何服务机关、障碍和战斗 |

这些链接是检索到的参考入口，不是已经逐帧分析完毕的录像。B 站正文访问返回 412，部分 YouTube 页面无法完整读取；本次没有据此编造截图、时码或测试结果。

后续逐段观察时，记录“时间点、角色动作、另一玩家的任务、成功反馈、失败反馈、镜头变化”。优先核对：技能归属、共享镜头与分屏切换、死亡恢复、机关触发条件、Boss 阶段。拿到你确认的截图或片段后即可把原型边界定准。

## 4. 技术选型

建议以 **Unity + C#** 为首选。你有 Java 开发经验，转向 C# 的语言学习负担预计较小；真正需要重点学习的是场景组件、游戏循环、物理、动画和输入系统。这个判断是结合项目目标的建议。

| 方案 | 适用方向 | 本项目判断 |
| --- | --- | --- |
| Unity 6.3 LTS + C# + URP | 桌面 3D、角色战斗、本地多人 | 首选；官方有多人输入、镜头工具和合作 RPG 示例 |
| Godot 4 + GDScript 或 C# | 自主控制较多的小型 2D/3D 项目 | 可选；若首版目标为网页，需注意当前 Godot 4 C# 工程不支持 Web 导出 |
| Unreal Engine + Blueprint/C++ | 3D 场景、蓝图原型及更复杂表现 | 可选；官方 Top Down 模板可起步，但要评估学习与制作投入 |

依据：[Unity 6 发布页](https://unity.com/releases/unity-6)、[Godot C# 平台支持](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/index.html)、[Unreal Top Down 模板](https://dev.epicgames.com/documentation/en-us/unreal-engine/top-down-template-in-unreal-engine)。截至调研时，Unity 页面列出了 6.3 LTS。工程创建时再锁定具体补丁版本，不把文档中的示例版本当作已验证依赖组合。

建议依赖与用途：

| 组件 | 用途 | 纳入阶段 |
| --- | --- | --- |
| URP | 城堡场景、灯光、角色及技能表现 | 基础工程 |
| Input System / PlayerInputManager | 两名玩家的设备绑定、加入与退出 | 第一个原型 |
| Cinemachine Target Group / Group Framing | 以两名角色为镜头目标，调整构图 | 第一个原型 |
| ScriptableObject | 保存技能、敌人、房间等静态配置 | 战斗与关卡阶段 |
| Unity Test Framework | 核心逻辑和场景交互的自动化验证 | 建立对应功能时 |
| Netcode for GameObjects | 实时状态同步 | 如果确定需要在线双人，再纳入工程 |
| Blender | 原创模型与动画制作 | 灰盒玩法确认后逐步替换美术 |

多人输入管理和目标组分别见 [PlayerInputManager 官方文档](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/PlayerInputManager.html)、[Cinemachine Target Group 文档](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineTargetGroup.html)。静态配置依据 [ScriptableObject 官方说明](https://docs.unity3d.com/6000.0/Documentation/Manual/class-ScriptableObject.html)。这里引用的是机制说明，未测试这些文档版本之间的兼容性。

对本地双人原型，建议让游戏进程管理关卡、战斗和存档。如果后续加入账号、云存档或排行榜，再按需要增加 Java 服务。若你的首要目标是异地两台电脑联机，应在第一个原型阶段就验证联网，而不是等内容完成后才引入。

## 5. 已读取的官方参考仓库

### Unity Boss Room

[Unity-Technologies/com.unity.multiplayer.samples.coop](https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop)

本次通过 GitHub 读取了 README 和 LICENSE。它是完整的合作多人 RPG 教学示例，覆盖技能、对象同步、RPC、连接与会话等主题，采用服务端权威的结构。适合按专题阅读技能、房间状态和联网流程，不建议直接把整套项目作为首版本地双人的依赖。

README 当前标示 Unity `6000.0.52f1` 和 Netcode `2.4.3`。这是该参考项目的版本，不代表新项目要照抄，也不代表已经验证它能直接升级到 Unity 6.3。

许可证为 Unity Companion License，文件明确说明用于 Unity-dependent projects。引用代码或素材前应按具体许可证保留说明。[许可证文件](https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop/blob/main/LICENSE.md)

### Godot 官方示例

[godotengine/godot-demo-projects](https://github.com/godotengine/godot-demo-projects)

本次读取了 README。仓库包含 2D/3D 示例；说明中明确 `master` 对应引擎开发分支，使用稳定版时要选择匹配分支。README 标注示例采用 MIT 许可。若选择 Godot，可从小示例逐项学习输入、角色、相机和多人机制。

## 6. 美术起步资源

先用简单几何体验证角色大小、攻击范围、门宽、房间尺度和相机遮挡，再替换模型。

| 来源 | 资源 | 适合用途 | 页面标注 |
| --- | --- | --- | --- |
| Kenney | [Castle Kit](https://kenney.nl/assets/castle-kit) | 城墙、塔楼和城堡空间原型 | 3D，75 个文件，CC0 |
| Kenney | [Fantasy Town Kit](https://kenney.nl/assets/fantasy-town-kit) | 城堡外围与幻想城镇建筑 | 3D，160 个文件，CC0 |
| Kenney | [Platformer Kit](https://kenney.nl/assets/platformer-kit) | 临时角色、基础平台与运动测试 | 3D，150 个文件，CC0 |

数量和许可仅对应上述具体页面，不推广为站内全部资源的保证。此次没有下载或导入这些素材。

美术方向建议：圆润的角色轮廓、容易辨认的武器、夸张但清楚的动作，城堡采用模块化构件。模型统一尺度、枢轴和命名；角色先验证待机、移动、攻击、受击和倒地动画。场景墙体要允许镜头看见角色，冰火特效不应盖住地面预警。

## 7. 当前未知项

- “魔法城堡”具体是城堡探索段、冰火地牢段，还是希望把两者合并。
- 首发平台与游玩方式：同一台电脑还是异地两台电脑。
- 是否已有模型、角色草图、工程或开发伙伴；本次没有取得相关材料。
- 可投入时间、目标画面、最低运行设备和手柄条件。
- 是否希望做线性合作冒险，还是增加随机房间和技能成长。

这些未知项不影响资料整理，但会影响代码工程和第一个里程碑的范围。当前开发方案以“冰火地牢、本地双人、桌面 3D、短篇线性冒险”为可审阅的假设。

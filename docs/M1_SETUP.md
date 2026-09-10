# M1：Unity C# 双人灰盒原型

状态：已编写源码；未在 Unity 中编译运行。此包是导入材料，不是 Unity Hub 可直接打开的完整工程，也不是游戏安装包。

## 1. 已编写的功能

- 两名玩家各绑定一个独立设备，支持两个手柄或一个键盘加一个手柄。
- 第一位加入者控制蓝色角色，第二位控制橙色角色；两人加入后可移动。
- 设备断开时停止双方移动；原设备恢复时保留角色。新设备也可通过加入操作接替断开的角色。
- 使用 CharacterController 移动、重力和碰撞，限制庭院活动范围。
- 共享俯视镜头跟踪双方，并根据完整角色边界与窗口比例调整视野。
- C# 编辑器菜单生成地面、围墙、障碍物、角色、灯光、镜头和会话对象。
- 用 NUnit / Unity Test Framework 编写设备分配与相机投影测试。

最新源码已追加 [M2 冰火训练场](M2_SETUP.md)：菜单同时生成训练木桩和冰火攻击组件。本页保留 M1 输入、移动与相机的导入和验收说明；攻击、重置、数值与测试状态见 M2 文档。冲刺、闪现、敌人 AI、机关和 Boss 尚未实现。

## 2. 创建本机工程

1. 在 Unity Hub 安装 **Unity 6.3 LTS** 的一个可用正式补丁；安装你需要的桌面构建支持。
2. 新建 **Universal 3D / URP** 项目，名称可使用 `MagicCastleCoop`。如果之后整理到仓库，建议项目放在 `game/`。
3. 在 Package Manager 的 Unity Registry 中安装 **Input System**。包版本选择编辑器标记为兼容的正式版本。
4. 安装 **Test Framework**；它用于执行本资料包中的测试。
5. 在 Player 设置中将 **Active Input Handling** 设为 `Input System Package (New)` 或 `Both`，按提示重启编辑器。
6. Input System 的 Update Mode 保持动态更新 `Process Events In Dynamic Update`。
7. 把资料包中 `UnityImport/Assets/MagicCastle/` 整个文件夹复制到新项目的 `Assets/`，等待导入和编译。

当前并未实际解析 Unity 包依赖，因此没有伪造 `manifest.json` 或 `ProjectVersion.txt`。首次成功运行后，把编辑器写入的具体版本、Packages 锁定文件、ProjectSettings 和 .meta 文件一起提交，保证后续开发者使用同一工程。

参考文档：[Unity 6 发布信息](https://unity.com/releases/unity-6)、[Input System 设备管理](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/Devices.html)、[Unity 测试程序集](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/workflow-create-test-assembly.html)。这些链接解释机制，不构成已验证的包版本组合。

## 3. 生成场景与操作

1. Console 无编译错误后，点击 **Magic Castle → Create Local Co-op Prototype**。
2. 如当前场景有未保存改动，编辑器会提示先保存。生成器不会覆盖已有原型场景，而是在 `Assets/MagicCastle/Generated/` 下建立独立目录。
3. 新场景自动保存为该目录内的 `LocalCoopPrototype.unity`。
4. 点击 Play，再点击 Game 视图使它获得输入焦点。
5. 每位玩家分别用自己的设备加入；只有两人都加入且设备在线时，角色才会移动。

| 设备 | 加入 | 移动 | 暂停 / 继续 |
| --- | --- | --- | --- |
| 键盘 | Enter | WASD | Esc |
| 手柄 | 下方主按钮，Xbox A / PlayStation Cross | 左摇杆 | Start / Menu / Options |

本单元只支持一名键盘玩家。键盘加手柄或双手柄均可，暂不实现同一键盘的双人按键划分。两个角色的身份按加入顺序决定。

角色断开时会留在原位置，整个会话暂停移动。若系统把重连的手柄识别为同一设备 ID，会自动恢复原角色；若识别为新设备，按加入键接替断开的角色。两名设备同时被重新识别时，接替顺序由加入顺序决定，需要真机验证。

## 4. 文件职责

| 文件 | 职责 |
| --- | --- |
| `Runtime/PlayerSlots.cs` | 两个玩家槽位，拒绝重复与第三设备，断开与替换规则 |
| `Runtime/LocalPlayerController.cs` | 输入方向归一化、移动、重力、边界与朝向 |
| `Runtime/LocalCoopSession.cs` | 绑定 Input System 设备、加入、暂停、驱动玩家和显示提示 |
| `Runtime/SharedCameraRig.cs` | 收集角色边界、相机投影构图、缩放和跟踪 |
| `Editor/PrototypeSceneBuilder.cs` | 在编辑器中生成并保存测试庭院 |
| `Tests/EditMode/PlayerSlotsTests.cs` | 验证设备归属和热插拔状态 |
| `Tests/EditMode/SharedCameraTests.cs` | 用实际 Camera 投影验证角色边界可见性 |

Runtime、Editor、Tests 分别有程序集定义；编辑器生成逻辑不进入普通游戏运行时。

M1 使用自己的共享相机脚本，因此当前不需要安装 Cinemachine。所有项目脚本均为 C#，无需 Python 或外部服务。

## 5. 测试步骤与当前结果

**实际完成：** 文件清单、程序集 JSON 和文档相对链接检查。未安装 Unity / C# 编译器，不能据此声明源代码编译成功。新增的独立 C# 检查及 GitHub Actions 状态见 [M2 说明](M2_SETUP.md)。

**尚未执行：** Unity 编译、以下自动化测试、Play Mode、手柄与构建验证。

在 **Window → General → Test Runner** 中选择 EditMode，运行 `MagicCastle.EditModeTests`。当前编写了 **16 个展开后的测试用例**：9 个设备状态用例，7 个相机用例。

| 自动化范围 | 预期 |
| --- | --- |
| 两名不同设备加入 | 两个槽位就绪后才可开始 |
| 同一设备重复加入 | 不生成第二名玩家 |
| 第三设备加入 | 不替换在线玩家 |
| 断开 / 同 ID 重连 | 保留角色分配并恢复就绪 |
| 替换断开的设备 | 另一位玩家的角色归属不变 |
| 旧设备重新出现 | 不能抢回已被替换的在线槽位 |
| 非法或未知设备 | 不产生有效玩家 |
| 16:9、4:3、9:16 构图 | 对角位置的角色边界均位于实际相机视口内 |
| 相机跟踪尚未到达中心 | 缩放仍覆盖两名角色 |
| 空目标与非法窗口比例 | 返回最小视野或拒绝非法参数 |

自动化测试不覆盖真实手柄驱动、物理移动或完整游戏体验，仍需以下人工验收：

| ID | 操作 | 预期 |
| --- | --- | --- |
| M1-01 | 双手柄同时移动；再重开改为键盘加手柄 | 操作各自对应角色，不串号 |
| M1-02 | 单人加入 | 展示等待提示，双方尚不可开始移动 |
| M1-03 | 两人就绪后按第三个手柄的加入键 | 仍只有原先两名角色 |
| M1-04 | 拔掉一个手柄，再插回或用新设备加入 | 双方暂时停下；恢复后角色归属符合重连规则 |
| M1-05 | 沿墙、障碍拐角和角落持续移动 | 不穿墙、不掉出地面；对角移动不比单轴更快 |
| M1-06 | 两人走到庭院相反角落，并调整窗口比例 | 角色保持可见；检查 HUD 是否遮挡关键区域 |
| M1-07 | 两个玩家同时按暂停键 | 暂停只切换一次，按一次可以继续 |
| M1-08 | 游戏窗口失焦后再返回 | 失焦时不移动，返回后按状态继续 |
| M1-09 | 连续进入 / 退出 Play Mode 三次 | 每次可以重新正常加入，Console 无持续错误 |
| M1-10 | 在 Build Profiles 中加入生成场景并导出桌面包 | 可脱离编辑器启动并完成 M1-01 至 M1-08 |

若任一用例失败，保存 Unity 版本、包版本、设备型号、Console 堆栈与复现步骤；先补最小复现测试，再修复。

## 6. 潜在问题与下一单元

- 当前源码未经 Unity 编译，首次导入可能暴露包引用或编辑器 API 兼容问题。
- 手柄重新连接后的设备 ID 由操作系统与 Input System 决定，槽位状态测试不能替代真机测试。
- 共享镜头已考虑角色边界，但障碍遮挡、提示栏覆盖和不同窗口比例仍需实际画面验证。
- CharacterController 的墙角、台阶和两名玩家互相阻挡需试玩调节；目前没有跳跃、冲刺或解卡系统。
- 当前参数面向生成的庭院。调整地图尺寸、角色比例或相机角度时应同步调整边界与测试。
- 本次未制作模型动画、音效、正式 UI 或性能基线，不承诺目标帧率。

最小战斗闭环的源码现已加入 [M2](M2_SETUP.md)，包含训练木桩、冻结、破冰、冷却与防重复命中。先一起完成 M1 / M2 的引擎验收，其后再加入敌人 AI、房间、机关与 Boss。

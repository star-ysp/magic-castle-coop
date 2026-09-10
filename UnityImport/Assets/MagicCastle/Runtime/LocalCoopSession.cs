using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MagicCastle
{
    public sealed class LocalCoopSession : MonoBehaviour
    {
        [SerializeField] private LocalPlayerController[] players = new LocalPlayerController[2];
        [SerializeField] private TrainingEnemy trainingTarget;
        private readonly PlayerSlots slots = new PlayerSlots();
        private readonly SimulationClock clock = new SimulationClock();
        private readonly PlayerCombat[] combat = new PlayerCombat[2];
        private readonly string[] deviceNames = { "Not joined", "Not joined" };
        private bool manuallyPaused;
        private GUIStyle textStyle;
        private GUIStyle titleStyle;

        public void Configure(LocalPlayerController[] scenePlayers, TrainingEnemy target = null)
        {
            if (scenePlayers == null || scenePlayers.Length != PlayerSlots.Capacity)
                throw new ArgumentException("Exactly two players are required.", nameof(scenePlayers));
            players = scenePlayers;
            trainingTarget = target;
        }

        private void Awake()
        {
            if (players == null || players.Length != PlayerSlots.Capacity ||
                players[0] == null || players[1] == null || players[0] == players[1])
            {
                Debug.LogError("Magic Castle needs two distinct player references.", this);
                enabled = false;
                return;
            }

            for (int i = 0; i < players.Length; i++)
            {
                combat[i] = players[i].GetComponent<PlayerCombat>();
                players[i].gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            // Resolve by stored device ID, never Gamepad.current for movement.
            for (int i = 0; i < PlayerSlots.Capacity; i++)
            {
                InputDevice device = InputSystem.GetDeviceById(slots.DeviceId(i));
                slots.SetConnected(slots.DeviceId(i), IsAvailable(device));
            }

            if (!Application.isFocused)
                return;

            Keyboard keyboard = Keyboard.current;
            if (IsAvailable(keyboard) && keyboard.enterKey.wasPressedThisFrame)
                Join(keyboard);
            foreach (Gamepad pad in Gamepad.all)
                if (IsAvailable(pad) && pad.buttonSouth.wasPressedThisFrame)
                    Join(pad);

            bool togglePause = false;
            for (int i = 0; i < PlayerSlots.Capacity; i++)
            {
                InputDevice device = InputSystem.GetDeviceById(slots.DeviceId(i));
                if (!IsAvailable(device))
                    continue;
                if (device is Keyboard boundKeyboard)
                    togglePause |= boundKeyboard.escapeKey.wasPressedThisFrame;
                if (device is Gamepad boundPad)
                    togglePause |= boundPad.startButton.wasPressedThisFrame;
            }

            // Two simultaneous pause inputs toggle once, rather than canceling each other.
            if (slots.Ready && togglePause)
                manuallyPaused = !manuallyPaused;

            if (!slots.Ready || manuallyPaused)
                return;

            clock.Advance(true, Time.deltaTime);
            bool resetTarget = false;
            for (int i = 0; i < PlayerSlots.Capacity; i++)
            {
                InputDevice device = InputSystem.GetDeviceById(slots.DeviceId(i));
                players[i].Tick(ReadMovement(device), Time.deltaTime);
                resetTarget |= device is Keyboard k && k.rKey.wasPressedThisFrame;
                resetTarget |= device is Gamepad p && p.selectButton.wasPressedThisFrame;
            }

            if (trainingTarget != null && trainingTarget.Defeated && resetTarget)
                trainingTarget.ResetTarget();

            // Movement finishes first; raycasts must see this frame's collider positions.
            Physics.SyncTransforms();
            for (int i = 0; i < PlayerSlots.Capacity; i++)
            {
                InputDevice device = InputSystem.GetDeviceById(slots.DeviceId(i));
                bool attack = device is Keyboard k && k.spaceKey.wasPressedThisFrame;
                attack |= device is Gamepad p && p.buttonWest.wasPressedThisFrame;
                if (attack && combat[i] != null)
                    combat[i].TryAttack(clock.Now);
            }
            if (trainingTarget != null)
                trainingTarget.Tick(clock.Now);
        }

        private void Join(InputDevice device)
        {
            if (!slots.TryJoin(device.deviceId, out int slot))
                return;
            deviceNames[slot] = device.displayName;
            players[slot].gameObject.SetActive(true);
        }

        private static bool IsAvailable(InputDevice device) =>
            device != null && device.added && device.enabled;

        private static Vector2 ReadMovement(InputDevice device)
        {
            if (device is Gamepad pad)
                return pad.leftStick.ReadValue();
            if (device is Keyboard keyboard)
                return new Vector2(
                    (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f),
                    (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f));
            return Vector2.zero;
        }

        private void OnGUI()
        {
            if (textStyle == null)
            {
                textStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true };
                titleStyle = new GUIStyle(textStyle) { fontSize = 24, fontStyle = FontStyle.Bold };
            }

            // A logical 960px viewport keeps prompts readable in small editor Game views.
            Matrix4x4 oldMatrix = GUI.matrix;
            float scale = Mathf.Clamp(Screen.width / 960f, 0.55f, 1.5f);
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            GUILayout.BeginArea(new Rect(16f, 12f, Mathf.Min(690f, Screen.width / scale - 32f), 320f),
                GUI.skin.box);
            GUILayout.Label("MAGIC CASTLE", titleStyle);
            GUILayout.Label("Enter / gamepad South: join   |   WASD / left stick: move", textStyle);
            GUILayout.Label("Esc / gamepad Start: pause", textStyle);
            GUILayout.Label("Space / gamepad West: attack   |   R / Select: reset defeated target", textStyle);
            GUILayout.Label(SlotLabel(0, "ICE"), textStyle);
            GUILayout.Label(SlotLabel(1, "FIRE"), textStyle);
            string status = !Application.isFocused ? "Click the game window to continue." :
                !slots.Ready ? "Waiting for two players. Reconnect, or press Join on a replacement device." :
                manuallyPaused ? "Paused. Press Esc / Start to continue." : "Ice freezes; fire shatters. Get close for fire!";
            GUILayout.Label(status, textStyle);
            if (trainingTarget != null)
            {
                string effect = trainingTarget.Defeated ? "DEFEATED" :
                    trainingTarget.IsFrozen(clock.Now) ? "FROZEN" : "READY";
                GUILayout.Label($"Target: {trainingTarget.Health}/{trainingTarget.MaxHealth} HP - {effect}", textStyle);
                GUILayout.Label(trainingTarget.LastHit, textStyle);
                for (int i = 0; i < combat.Length; i++)
                    if (combat[i] != null)
                        GUILayout.Label($"P{i + 1}: {combat[i].Feedback}", textStyle);
            }
            GUILayout.EndArea();
            GUI.matrix = oldMatrix;
        }

        private string SlotLabel(int index, string role)
        {
            string state = slots.DeviceId(index) < 0 ? "Not joined" :
                slots.IsConnected(index) ? deviceNames[index] : "Disconnected";
            return $"P{index + 1} - {role}: {state}";
        }
    }
}

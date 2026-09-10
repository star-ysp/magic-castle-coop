using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagicCastle
{
    [RequireComponent(typeof(Camera))]
    public sealed class SharedCameraRig : MonoBehaviour
    {
        [SerializeField] private LocalPlayerController[] targets = new LocalPlayerController[2];
        [SerializeField] private float minSize = 6f;
        [SerializeField] private float padding = 2f;
        [SerializeField] private float smoothTime = 0.18f;
        private readonly List<Vector3> corners = new List<Vector3>(16);
        private Camera view;
        private Vector3 focus;
        private Vector3 focusVelocity;
        private float zoomVelocity;
        private bool initialized;

        public void Configure(LocalPlayerController[] players) => targets = players;

        private void Awake()
        {
            view = GetComponent<Camera>();
            view.orthographic = true;
        }

        private void LateUpdate()
        {
            corners.Clear();
            foreach (LocalPlayerController player in targets)
            {
                if (player == null || !player.gameObject.activeInHierarchy)
                    continue;
                Bounds bounds = player.GetComponent<CharacterController>().bounds;
                bounds.Expand(new Vector3(0.6f, 0.4f, 0.6f));
                for (int x = -1; x <= 1; x += 2)
                    for (int y = -1; y <= 1; y += 2)
                        for (int z = -1; z <= 1; z += 2)
                            corners.Add(bounds.center + Vector3.Scale(bounds.extents, new Vector3(x, y, z)));
            }

            if (corners.Count == 0)
                return;

            Bounds group = new Bounds(corners[0], Vector3.zero);
            for (int i = 1; i < corners.Count; i++)
                group.Encapsulate(corners[i]);
            focus = initialized ? Vector3.SmoothDamp(focus, group.center, ref focusVelocity,
                smoothTime, Mathf.Infinity, Time.unscaledDeltaTime) : group.center;

            Quaternion rotation = Quaternion.Euler(55f, 0f, 0f);
            transform.SetPositionAndRotation(focus - rotation * Vector3.forward * 40f, rotation);
            float required = RequiredOrthographicSize(corners, focus, rotation, view.aspect, minSize, padding);

            if (!initialized || required >= view.orthographicSize)
            {
                // Widen immediately: smoothing an expansion can clip a player at the screen edge.
                view.orthographicSize = required;
                zoomVelocity = 0f;
            }
            else
            {
                view.orthographicSize = Mathf.Max(required, Mathf.SmoothDamp(view.orthographicSize,
                    required, ref zoomVelocity, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime));
            }
            initialized = true;
        }

        public static float RequiredOrthographicSize(IReadOnlyList<Vector3> points, Vector3 center,
            Quaternion rotation, float aspect, float minimum, float margin)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            if (aspect <= 0f || float.IsNaN(aspect) || float.IsInfinity(aspect))
                throw new ArgumentOutOfRangeException(nameof(aspect));
            if (minimum <= 0f || float.IsNaN(minimum) || float.IsInfinity(minimum))
                throw new ArgumentOutOfRangeException(nameof(minimum));
            if (margin < 0f || float.IsNaN(margin) || float.IsInfinity(margin))
                throw new ArgumentOutOfRangeException(nameof(margin));

            Quaternion worldToView = Quaternion.Inverse(rotation);
            float size = minimum;
            foreach (Vector3 point in points)
            {
                Vector3 local = worldToView * (point - center);
                size = Mathf.Max(size, Mathf.Max(Mathf.Abs(local.y) + margin,
                    (Mathf.Abs(local.x) + margin) / aspect));
            }
            return size;
        }
    }
}

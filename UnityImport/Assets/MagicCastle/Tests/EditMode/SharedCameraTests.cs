using System;
using NUnit.Framework;
using UnityEngine;

namespace MagicCastle.Tests
{
    public sealed class SharedCameraTests
    {
        [TestCase(16f / 9f)]
        [TestCase(4f / 3f)]
        [TestCase(9f / 16f)]
        public void BothOppositePlayersFitActualCameraProjection(float aspect)
        {
            // Includes top/bottom corners, rather than checking only character pivots.
            Vector3[] points =
            {
                new Vector3(-11.6f, 0f, -8.6f), new Vector3(-10.4f, 2.4f, -7.4f),
                new Vector3(10.4f, 0f, 7.4f), new Vector3(11.6f, 2.4f, 8.6f)
            };
            // Deliberately offset from the group center to represent camera follow lag.
            Vector3 focus = new Vector3(2f, 1f, -1f);
            Quaternion rotation = Quaternion.Euler(55f, 0f, 0f);
            var cameraObject = new GameObject("Test camera");
            try
            {
                Camera camera = cameraObject.AddComponent<Camera>();
                camera.orthographic = true;
                camera.aspect = aspect;
                camera.orthographicSize = SharedCameraRig.RequiredOrthographicSize(points,
                    focus, rotation, aspect, 6f, 2f);
                camera.transform.SetPositionAndRotation(focus - rotation * Vector3.forward * 40f, rotation);
                foreach (Vector3 point in points)
                {
                    Vector3 viewport = camera.WorldToViewportPoint(point);
                    Assert.That(viewport.x, Is.InRange(0.001f, 0.999f));
                    Assert.That(viewport.y, Is.InRange(0.001f, 0.999f));
                    Assert.That(viewport.z, Is.GreaterThan(camera.nearClipPlane));
                    Assert.That(viewport.z, Is.LessThan(camera.farClipPlane));
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(cameraObject);
            }
        }

        [Test]
        public void EmptyGroupKeepsMinimumViewSize()
        {
            float size = SharedCameraRig.RequiredOrthographicSize(Array.Empty<Vector3>(),
                Vector3.zero, Quaternion.identity, 16f / 9f, 6f, 2f);
            Assert.That(size, Is.EqualTo(6f));
        }

        [TestCase(0f)]
        [TestCase(-1f)]
        [TestCase(float.NaN)]
        public void InvalidAspectIsRejected(float aspect)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SharedCameraRig.RequiredOrthographicSize(
                Array.Empty<Vector3>(), Vector3.zero, Quaternion.identity, aspect, 6f, 2f));
        }
    }
}

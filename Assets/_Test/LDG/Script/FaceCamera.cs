using System;
using UnityEngine;

namespace _Test.LDG.Script
{
    public class FaceCamera : MonoBehaviour
    {
        private Transform mainCamera;
        private void Start() => mainCamera = Camera.main.transform;
        private void LateUpdate() => transform.LookAt(transform.position + mainCamera.rotation * Vector3.forward, mainCamera.rotation * Vector3.up);
    }
}
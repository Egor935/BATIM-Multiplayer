using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BATIM_Multiplayer
{
    public class ReadData : MonoBehaviour
    {
        private Vector3 _Position;
        private float _Rotation;
        private Vector3 _MovingSpeed;
        private float _RotatingSpeed;
        private Vector2 _Input;
        private float _Cam;
        private bool _IsSlowed;

        private Vector3 _CameraOrigin;
        private Vector3 _CameraDirection;

        private PlayerController _Player;
        private Camera _Camera;

        void Start()
        {
            _Player = GetComponent<PlayerController>();
            _Camera = GetComponentInChildren<Camera>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ReadCamera();
                SendData.SendInteraction(_CameraOrigin, _CameraDirection);
            }

            if (BaseWeapon_CheckHitRaycast.Hit)
            {
                ReadCamera();
                SendData.SendAttack(_CameraOrigin, _CameraDirection);
                BaseWeapon_CheckHitRaycast.Hit = false;
            }
        }

        void FixedUpdate()
        {
            ReadMovement();
            SendData.SendMovement(_Position, _Rotation, _MovingSpeed, _RotatingSpeed, _Input, _Cam, _IsSlowed);
        }

        float cachedTime;
        private void ReadMovement()
        {
            var deltaTime = Time.time - cachedTime;
            var movingSpeed = (this.transform.position - _Position - Vector3.up * 2.955f) / deltaTime;
            var rotatingSpeed = Mathf.DeltaAngle(_Rotation, this.transform.rotation.eulerAngles.y) / deltaTime;

            _Position = this.transform.position - Vector3.up * 2.955f;
            _Rotation = this.transform.rotation.eulerAngles.y;
            _MovingSpeed = movingSpeed;
            _RotatingSpeed = rotatingSpeed;
            _Input = !_Player.m_IsPaused ? _Player.m_Input : Vector2.zero;

            var cam = _Camera.transform.parent.rotation.eulerAngles.x;
            _Cam = cam < 180f ? Mathf.Min(cam, 50f) : Mathf.Max(cam, 300f);

            _IsSlowed = _Player.isSlowed;

            cachedTime = Time.time;
        }

        private void ReadCamera()
        {
            _CameraOrigin = _Camera.transform.position;
            _CameraDirection = _Camera.transform.forward;
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.name == "ink_Water")
            {
                _Player.SetSlowed(true);
                _Player.SetJump(false);
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.name == "ink_Water")
            {
                _Player.SetSlowed(false);
                _Player.SetJump(true);
            }
        }
    }
}

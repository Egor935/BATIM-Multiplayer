using S13Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMG.UI;
using UnityEngine;

namespace BATIM_Multiplayer
{
    public class Boris : MonoBehaviour
    {
        public static Vector3 ReceivedPosition;
        public static float ReceivedRotation;
        public static Vector3 MovingSpeed;
        public static float RotatingSpeed;
        public static Vector2 ReceivedInput;
        public static float ReceivedCam;
        public static bool IsSlowed;
        public static bool MovementReceived;
        public static bool AttackReceived;

        private float extrapolatedRotation;

        private Animator _Animator;
        private S13Switch _AudioSwitch;
        private GameObject _Axe;
        private GameObject _Head;
        void Start()
        {
            _Animator = GetComponent<Animator>();
            _AudioSwitch = GetComponentInChildren<S13Switch>();

            _Axe = GameObject.Find("Boris_Other_Axe");
            _Axe.SetActive(false);

            _Head = GameObject.Find("lnk_head001");
        }

        void Update()
        {
            if (MovementReceived)
            {
                this.transform.position = ReceivedPosition;
                extrapolatedRotation = ReceivedRotation;

                MovementReceived = false;
            }
            else
            {
                this.transform.position += MovingSpeed * Time.deltaTime;
                extrapolatedRotation = Mathf.Repeat(extrapolatedRotation + RotatingSpeed * Time.deltaTime, 360f);
            }

            if (AttackReceived)
            {
                _Animator.SetTrigger("Attack");
                AttackReceived = false;
            }

            if (GameManager.Instance.Player != null)
            {
                if (GameManager.Instance.Player.m_Interaction.HasWeapon)
                {
                    _Axe.SetActive(true);
                }
                else
                {
                    _Axe.SetActive(false);
                }
            }
        }

        float t1;
        float t2;
        Quaternion cachedHeadRotation;
        Quaternion cachedBodyRotation;
        void LateUpdate()
        {
            if (ReceivedInput != Vector2.zero)
            {
                t1 += Time.deltaTime / 0.1f;

                var newY = Mathf.LerpAngle(cachedBodyRotation.eulerAngles.y, extrapolatedRotation, t1);
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, newY, this.transform.rotation.eulerAngles.z);

                _Head.transform.localRotation = Quaternion.Lerp(cachedHeadRotation, Quaternion.identity, t1);

                t2 = 0;

                return;
            }

            var difference = Mathf.DeltaAngle(this.transform.rotation.eulerAngles.y, extrapolatedRotation);

            if (difference > 30f)
            {
                var newY = this.transform.rotation.eulerAngles.y + difference - 30f;
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, newY, this.transform.rotation.eulerAngles.z);
            }

            if (difference < -30f)
            {
                var newY = this.transform.rotation.eulerAngles.y + difference + 30f;
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, newY, this.transform.rotation.eulerAngles.z);
            }

            t2 += Time.deltaTime / 0.1f;

            difference = Mathf.Clamp(difference, -30f, 30f);
            var newRot = Quaternion.Euler(-difference, _Head.transform.localRotation.eulerAngles.y, ReceivedCam);
            _Head.transform.localRotation = Quaternion.Lerp(Quaternion.identity, newRot, t2);

            cachedHeadRotation = _Head.transform.localRotation;
            cachedBodyRotation = this.transform.rotation;

            t1 = 0;
        }

        void FixedUpdate()
        {
            if (ReceivedInput != Vector2.zero)
            {
                if (ReceivedInput.y < 0f)
                {
                    _Animator.SetFloat("Speed", -7f / 12f);
                }
                else if (ReceivedInput.y > 0f && !IsSlowed)
                {
                    _Animator.SetFloat("Speed", 1f);
                }
                else
                {
                    _Animator.SetFloat("Speed", 7f / 12f);
                }
                _Animator.SetInteger("MovementMode", 1);
            }
            else
            {
                _Animator.SetInteger("MovementMode", 0);
            }
        }

        public void Hit(Vector3 point)
        {
            _Animator.SetTrigger("Hit");
            _AudioSwitch.Play("hit");
            GameObject fromPool = GameManager.Instance.PoolingManager.GetFromPool("GamePlay/Particles/InkHit");
            fromPool.transform.position = point;
            fromPool.transform.localScale = Vector3.one * 0.2f;

            SendData.SendHit(point);
        }
    }
}

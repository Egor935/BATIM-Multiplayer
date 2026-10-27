using S13Audio;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;

namespace BATIM_Multiplayer
{
    public class HandleData
    {
        public static void HandleMovement(Packet _packet)
        {
            Boris.ReceivedPosition = _packet.ReadVector3();
            Boris.ReceivedRotation = _packet.ReadFloat();
            Boris.MovingSpeed = _packet.ReadVector3();
            Boris.RotatingSpeed = _packet.ReadFloat();
            Boris.ReceivedInput = _packet.ReadVector2();
            Boris.ReceivedCam = _packet.ReadFloat();
            Boris.IsSlowed = _packet.ReadBool();
            Boris.MovementReceived = true;
        }

        public static void HandleInteraction(Packet _packet)
        {
            var origin = _packet.ReadVector3();
            var direction = _packet.ReadVector3();

            var sphereCastThickness = GameManager.Instance.Player.m_Interaction.m_SphereCastThickness;
            var ignoreLayers = GameManager.Instance.Player.m_Interaction.m_IgnoreLayers;

            if (Physics.SphereCast(origin, sphereCastThickness, direction, out RaycastHit raycastHit, 8f, ~ignoreLayers))
            {
                Interactable component = raycastHit.transform.GetComponent<Interactable>();

                if (component != null)
                {
                    component.Interact();
                }
            }
        }

        public static void HandleAttack(Packet _packet)
        {
            var origin = _packet.ReadVector3();
            var direction = _packet.ReadVector3();

            var baseWeapon = FindObjectOfType<BaseWeapon>();
            var weaponRange = baseWeapon.m_WeaponRange;
            var ignoreLayers = baseWeapon.m_IgnoreLayers;

            if (Physics.SphereCast(origin, 0.7f, direction, out RaycastHit raycastHit, weaponRange + 1.5f, ~ignoreLayers))
            {
                GameObject gameObject = raycastHit.transform.gameObject;
                IHittable componentInParent = gameObject.GetComponentInParent<IHittable>();
                if (componentInParent != null)
                {
                    componentInParent.Hit(raycastHit, baseWeapon.m_WeaponInfo);
                }
            }

            int index = UnityEngine.Random.Range(0, baseWeapon.m_AttackClips.Count);
            AudioClip audioClip = baseWeapon.m_AttackClips[index];
            GameManager.Instance.AudioManager.PlayAtPosition(audioClip, origin);

            Boris.AttackReceived = true;
        }

        public static void HandleBendyTrigger(Packet _packet)
        {
            var ch1BendyFinaleController = FindObjectOfType<CH1BendyFinaleController>();
            ch1BendyFinaleController.m_CanScare = true;
            MeshRenderer[] array = FindObjectsOfType<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in array)
            {
                if (meshRenderer.isVisible)
                {
                    ch1BendyFinaleController.m_InkMachineMeshRenderer = meshRenderer;
                    break;
                }
            }
        }

        private static S13Switch _AudioSwitch;
        public static void HandleHit(Packet _packet)
        {
            var point = _packet.ReadVector3();

            if (_AudioSwitch == null)
            {
                var assetBundle = AssetBundle.GetAllLoadedAssetBundles_Native().FirstOrDefault();
                var prefab = assetBundle.LoadAsset<GameObject>("AudioProxy");
                var audioProxy = Instantiate(prefab, GameManager.Instance.Player.transform);
                _AudioSwitch = audioProxy.GetComponentInChildren<S13Switch>();
            }

            _AudioSwitch.Play("hit");

            GameObject fromPool = GameManager.Instance.PoolingManager.GetFromPool("GamePlay/Particles/InkHit");
            fromPool.transform.position = point;
            fromPool.transform.localScale = Vector3.one * 0.2f;
        }
    }
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BATIM_Multiplayer
{
    [HarmonyPatch(typeof(CH1BendyFinaleController), nameof(CH1BendyFinaleController.HandleBendyEventTriggerOnEnter))]
    class CH1BendyFinaleController_HandleBendyEventTriggerOnEnter
    {
        public static void Postfix()
        {
            SendData.SendBendyTrigger();
        }
    }

    [HarmonyPatch(typeof(BaseWeapon), nameof(BaseWeapon.CheckHitRaycast))]
    class BaseWeapon_CheckHitRaycast
    {
        public static bool Hit;

        public static void Postfix(BaseWeapon __instance)
        {
            Hit = true;

            Transform transform = GameManager.Instance.GameCamera.transform;
            if (Physics.SphereCast(transform.position, 0.7f, transform.forward, out RaycastHit hitData, __instance.m_WeaponRange + 1.5f))
            {
                if (hitData.transform.name.Contains("Boris"))
                {
                    var boris = hitData.transform.GetComponent<Boris>();
                    boris.Hit(hitData.point);
                }
            }
        }
    }

    [HarmonyPatch(typeof(InkWaterController), nameof(InkWaterController.InitOnComplete))]
    class InkWaterController_InitOnComplete
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.PauseGame))]
    class GameManager_PauseGame
    {
        public static void Postfix()
        {
            Time.timeScale = 1f;
        }
    }
}

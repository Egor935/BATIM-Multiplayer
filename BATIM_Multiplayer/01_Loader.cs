using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BATIM_Multiplayer
{
    [BepInPlugin("Egor_ICE", "BATIM Multiplayer", "1.1")]
    public class Loader : BasePlugin
    {
        public override void Load()
        {
            AddComponent<Core>();
            AddComponent<Steam>();
            AddComponent<UI>();

            ClassInjector.RegisterTypeInIl2Cpp<ReadData>();
            ClassInjector.RegisterTypeInIl2Cpp<Boris>();

            var harmony = new Harmony("Harmony");
            harmony.PatchAll();
        }
    }
}

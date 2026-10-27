using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BATIM_Multiplayer
{
    public class Core : MonoBehaviour
    {
        public static bool IsMultiplayer;

        private GameObject _Player;

        void Start()
        {
            CreateBoris();
        }

        void Update()
        {
            if (_Player == null)
            {
                var playerController = FindObjectOfType<PlayerController>();
                if (playerController != null)
                {
                    playerController.gameObject.AddComponent<ReadData>();
                    _Player = playerController.gameObject;

                    RemoveBlockers();
                }
            }
        }

        public static void CreateBoris()
        {
            var assetBundle = AssetBundle.LoadFromMemory(Properties.Resources.boris);
            var prefab = assetBundle.LoadAsset<GameObject>("Boris");
            var boris = Instantiate(prefab);
            boris.AddComponent<Boris>();
            DontDestroyOnLoad(boris);
        }

        public static void RemoveBlockers()
        {
            for (int i = 1; i <= 5; i++)
            {
                var collapseContent = GameObject.Find($"[ENVIRONMENT]/[SECTIONS]/InkMachineRoom/Section_Controller/BendyFinaleController/StudioCollapseContainer/CollapseContent_{i}");
                if (collapseContent != null)
                {
                    foreach (var collider in collapseContent.GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                }
            }
            var blocker = GameObject.Find("[ENVIRONMENT]/[SECTIONS]/MainRoom_4/Section_Controller/Finale/ToActivate/BlockCollider_02");
            if (blocker != null)
            {
                blocker.GetComponent<Collider>().enabled = false;
            }
        }
    }
}

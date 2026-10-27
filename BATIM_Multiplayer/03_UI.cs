using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BATIM_Multiplayer
{
    public class UI : MonoBehaviour
    {
        bool menuState = true;
        bool lobbyCreated;
        void OnGUI()
        {
            if (!menuState)
                return;

            GUI.color = Color.yellow;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            
            GUI.Box(new Rect(10, 10, 160, 160), "<b>BATIM Multiplayer</b>");

            if (Steam.ConnectedPlayer == string.Empty)
            {
                if (GUI.Button(new Rect(40, 40, 100, 25), "Invite"))
                {
                    if (!lobbyCreated)
                    {
                        Steam.CreateLobby();
                        lobbyCreated = true;
                    }

                    Steam.Invite();
                }
            }
            else
            {
                GUI.Label(new Rect(15, 40, 150, 25), "Connected friend:");
                GUI.Label(new Rect(15, 70, 150, 25), Steam.ConnectedPlayer);

                if (GUI.Button(new Rect(40, 130, 100, 25), "Close (~)"))
                {
                    menuState = false;
                }
            }

            GUI.Box(new Rect(10, 175, 160, 95), string.Empty);
            GUI.Label(new Rect(15, 180, 150, 25), "Version: 1.1");
            GUI.Label(new Rect(15, 210, 150, 25), "Made by Egor_ICE");
            if (GUI.Button(new Rect(15, 240, 150, 25), "Support the author"))
            {
                Application.OpenURL("https://boosty.to/egor_ice/donate");
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                menuState = !menuState;
            }
        }
    }
}

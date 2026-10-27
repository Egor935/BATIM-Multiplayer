using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BATIM_Multiplayer
{
    public class SendData
    {
        public static void SendMovement(Vector3 pos, float rot, Vector3 movingSpeed, float rotatingSpeed, Vector2 input, float cam, bool isSlowed)
        {
            using (Packet _packet = new Packet((int)Packets.Movement))
            {
                _packet.Write(pos);
                _packet.Write(rot);
                _packet.Write(movingSpeed);
                _packet.Write(rotatingSpeed);
                _packet.Write(input);
                _packet.Write(cam);
                _packet.Write(isSlowed);
                Networking.SendUnreliableData(_packet);
            }
        }

        public static void SendInteraction(Vector3 origin, Vector3 direction)
        {
            using (Packet _packet = new Packet((int)Packets.Interaction))
            {
                _packet.Write(origin);
                _packet.Write(direction);
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendAttack(Vector3 origin, Vector3 direction)
        {
            using (Packet _packet = new Packet((int)Packets.Attack))
            {
                _packet.Write(origin);
                _packet.Write(direction);
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendBendyTrigger()
        {
            using (Packet _packet = new Packet((int)Packets.BendyTrigger))
            {
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendHit(Vector3 point)
        {
            using (Packet _packet = new Packet((int)Packets.Hit))
            {
                _packet.Write(point);
                Networking.SendReliableData(_packet);
            }
        }
    }
}

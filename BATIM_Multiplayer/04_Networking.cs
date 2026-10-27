using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BATIM_Multiplayer
{
    public class Networking
    {
        private delegate void PacketHandler(Packet _packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        public static void ListenData()
        {
            while (SteamNetworking.IsP2PPacketAvailable(out uint size))
            {
                byte[] _data = new byte[size];
                if (SteamNetworking.ReadP2PPacket(_data, size, out uint bytesRead, out CSteamID remoteId))
                {
                    HandleReceivedData(_data);
                }
            }
        }

        public static void SendReliableData(Packet packet)
        {
            byte[] data = packet.ToArray();
            SteamNetworking.SendP2PPacket(Steam.Receiver, data, (uint)data.Length, EP2PSend.k_EP2PSendReliable, 0);
        }

        public static void SendUnreliableData(Packet packet)
        {
            byte[] data = packet.ToArray();
            SteamNetworking.SendP2PPacket(Steam.Receiver, data, (uint)data.Length, EP2PSend.k_EP2PSendUnreliable, 0);
        }

        public static bool HandlePacket;
        private static void HandleReceivedData(byte[] _data)
        {
            HandlePacket = true;

            using (Packet _packet = new Packet(_data))
            {
                int _packetId = _packet.ReadInt();
                packetHandlers[_packetId].Invoke(_packet);
            }

            HandlePacket = false;
        }

        public static void InitializePackets()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)Packets.Movement, HandleData.HandleMovement },
                { (int)Packets.Interaction, HandleData.HandleInteraction },
                { (int)Packets.Attack, HandleData.HandleAttack },
                { (int)Packets.BendyTrigger, HandleData.HandleBendyTrigger },
                { (int)Packets.Hit, HandleData.HandleHit }
            };
        }
    }
}

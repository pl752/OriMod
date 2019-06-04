﻿using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OriMod {
  internal class OriPlayerPacketHandler : PacketHandler {
    internal OriPlayerPacketHandler(byte handlerType) : base(handlerType)
		{
			HandlerType = handlerType;
		}
    internal const byte OriState = 1;
    internal override void HandlePacket(BinaryReader reader, int fromWho) {
      byte packetType = reader.ReadByte();
      if (Main.netMode == NetmodeID.MultiplayerClient) {
		    fromWho = reader.ReadUInt16();
	    }
      switch(packetType) {
        case OriState:
          ReceiveOriState(reader, fromWho);
          break;
        default:
          Main.NewText("Unknown OriStateHandler byte " + packetType);
          break;
      }
    }
    internal void SendOriState(int toWho, int fromWho) {
      ModPacket packet = GetPacket(OriState, fromWho);
      OriPlayer fromPlayer = Main.player[fromWho].GetModPlayer<OriPlayer>();

      BitsByte flags = new BitsByte();
      flags[0] = fromPlayer.OriSet;
      flags[1] = fromPlayer.Flashing;
      flags[2] = fromPlayer.Transforming;
      flags[3] = fromPlayer.UnrestrictedMovement;
      packet.Write((byte)flags);
      if (fromPlayer.Transforming) {
        packet.Write((short)fromPlayer.TransformTimer);
      }

      packet.Send(toWho, fromWho);
    }

    internal void ReceiveOriState(BinaryReader r, int fromWho) {
      OriPlayer fromPlayer = Main.player[fromWho].GetModPlayer<OriPlayer>();
      BitsByte flags = r.ReadByte();
      bool oriSet = flags[0];
      bool flashing = flags[1];
      bool transforming = flags[2];
      bool unrestrictedMovement = flags[3];
      short transformTimer = 0;
      if (transforming) {
        transformTimer = r.ReadInt16();
      }

      fromPlayer.OriSet = oriSet;
      fromPlayer.Flashing = flashing;
      fromPlayer.Transforming = transforming;
      fromPlayer.UnrestrictedMovement = unrestrictedMovement;
      fromPlayer.TransformTimer = transformTimer;
      
      if (Main.netMode == NetmodeID.Server) {
				SendOriState(-1, fromWho);
			}
    }
  }
}
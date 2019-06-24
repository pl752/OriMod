using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OriMod {
  public static class OriLayers {
    internal static readonly PlayerLayer PlayerSprite = new PlayerLayer("OriMod", "OriPlayer", delegate (PlayerDrawInfo drawInfo) {
      Mod mod = ModLoader.GetMod("OriMod");
      OriPlayer oPlayer = drawInfo.drawPlayer.GetModPlayer<OriPlayer>(mod);
      DrawData data = DefaultDrawData(oPlayer, oPlayer.Animations.PlayerAnim);
      data.color = oPlayer.Flashing ? Color.Red : oPlayer.Transforming && oPlayer.AnimName == "TransformStart" ? Color.White : oPlayer.SpriteColor;
      Main.playerDrawData.Add(data);
    });
    internal static readonly PlayerLayer Trail = new PlayerLayer("OriMod", "OriTrail", delegate (PlayerDrawInfo drawInfo) {
      Mod mod = ModLoader.GetMod("OriMod");
      Player player = drawInfo.drawPlayer;
      OriPlayer oPlayer = player.GetModPlayer<OriPlayer>(mod);
      Texture2D texture = mod.GetTexture("PlayerEffects/OriGlow");
      for (int i = 0; i < 26; i++) {
        oPlayer.Trails[i].Tick();
      }
      if (!player.dead && !player.invis) {
        oPlayer.TrailIndex++;
        if (oPlayer.TrailIndex > 25) {
          oPlayer.TrailIndex = 0;
        }
        Trail trail = oPlayer.Trails[oPlayer.TrailIndex];
        trail.Position = player.Center;
        trail.Frame = oPlayer.AnimFrame;
        trail.Direction.X = player.direction;
        trail.Direction.Y = (int)player.gravDir;
        float alpha = player.velocity.Length() * 0.002f;
        if (alpha > 0.08f) alpha = 0.08f;
        trail.Alpha = trail.StartAlpha = alpha;
        trail.Rotation = oPlayer.AnimRads;
        if (trail.Alpha > 104) {
          trail.Alpha = 104;
        }
      }
      for (int i = 0; i < 26; i++) {
        Trail trail = oPlayer.Trails[i];
        SpriteEffects effect = SpriteEffects.None;
        if (trail.Direction.X == -1) effect = effect | SpriteEffects.FlipHorizontally;
        if (trail.Direction.Y == -1) effect = effect | SpriteEffects.FlipVertically;

        Color color = oPlayer.SpriteColor * (trail.Alpha * 10);
        DrawData data = new DrawData(
          texture,
          new Vector2(trail.Position.X - Main.screenPosition.X, trail.Position.Y - Main.screenPosition.Y),
          new Rectangle(trail.X, trail.Y, OriPlayer.SpriteWidth, OriPlayer.SpriteHeight), color, trail.Rotation,
          new Vector2(OriPlayer.SpriteWidth / 2, OriPlayer.SpriteHeight / 2 + 6), 1, effect, 0
        );
        data.position += Offset();
        Main.playerDrawData.Add(data);
      }
    });
    internal static readonly PlayerLayer BashArrow = new PlayerLayer("OriMod", "BashArrow", delegate (PlayerDrawInfo drawInfo) {
      Mod mod = ModLoader.GetMod("OriMod");
      Player player = drawInfo.drawPlayer;
      OriPlayer oPlayer = player.GetModPlayer<OriPlayer>(mod);
      Animation anim = oPlayer.Animations.BashAnim;
      Texture2D texture = anim.Texture(mod);
      SpriteEffects effect = SpriteEffects.None;
      int y = oPlayer.bash.CurrDuration < 40 ? 0 : oPlayer.bash.CurrDuration < 50 ? 1 : 2;

      DrawData data = new DrawData(texture,
        new Vector2(oPlayer.bash.Npc.Center.X - Main.screenPosition.X, oPlayer.bash.Npc.Center.Y - Main.screenPosition.Y),
        new Rectangle(0, y * 20, 152, 20),
        Color.White, oPlayer.bash.Npc.AngleTo(Main.MouseWorld),
        new Vector2(76, 10), 1, effect, 0);
      Main.playerDrawData.Add(data);
    });
    internal static readonly PlayerLayer FeatherSprite = new PlayerLayer("OriMod", "Feather", delegate (PlayerDrawInfo drawInfo) {
      Mod mod = ModLoader.GetMod("OriMod");
      OriPlayer oPlayer = drawInfo.drawPlayer.GetModPlayer<OriPlayer>(mod);
      DrawData data = DefaultDrawData(oPlayer, oPlayer.Animations.GlideAnim);
      Main.playerDrawData.Add(data);
    });

    // Used with frames that are not aligned to center
    private static Vector2 Offset(int x=0, int y=0) {
      Vector2 offset = new Vector2(x, y);
      return offset;
    }
    private static DrawData DefaultDrawData(OriPlayer oPlayer, Animation anim) {
      Player player = oPlayer.player;
      Texture2D texture = anim.Texture(oPlayer.mod);
      Vector2 pos = new Vector2(player.Center.X - Main.screenPosition.X, player.Center.Y - Main.screenPosition.Y);
      SpriteEffects effect = SpriteEffects.None;
      if (player.direction == -1) effect = effect | SpriteEffects.FlipHorizontally;
      if (player.gravDir == -1) effect = effect | SpriteEffects.FlipVertically;
      Point tile = anim.ActiveTile;
      Rectangle rect = new Rectangle(tile.X, tile.Y, anim.TileSize.X, anim.TileSize.Y);
      Vector2 orig = new Vector2(OriPlayer.SpriteWidth / 2, OriPlayer.SpriteHeight / 2 + 5 * player.gravDir);
      DrawData data = new DrawData(texture, pos, rect, oPlayer.SpriteColor, player.direction * oPlayer.AnimRads, orig, 1, effect, 0);
      data.position += Offset();
      return data;
    }
  }
}
﻿using OriMod.Abilities;
using Terraria.ID;

namespace OriMod.Items.Abilities {
  /// <summary>
  /// Medallion that grants <see cref="Stomp"/> Level 1.
  /// </summary>
  public class StompLevel1 : AbilityMedallionBase {
    public override byte ID => AbilityID.Stomp;

    public override void AddRecipes() {
      var recipe = GetAbilityRecipe();
      recipe.AddRecipeGroup("OriMod:HardmodeBars3", 18);
      recipe.AddIngredient(ItemID.SoulofMight, 12);
      recipe.AddRecipe();
    }
  }
}
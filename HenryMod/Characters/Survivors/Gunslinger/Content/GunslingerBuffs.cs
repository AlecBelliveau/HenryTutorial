using RoR2;
using UnityEngine;

namespace DestinyGuardiansMod.Survivors.Gunsligner
{
    public static class GunslingerBuffs
    {
        // armor buff gained during roll
        public static BuffDef armorBuff;
        public static BuffDef cranialSpikeBuff;
        public static BuffDef RadiantBuff;
        public static void Init(AssetBundle assetBundle)
        {
            armorBuff = Modules.Content.CreateAndAddBuff("HenryArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);

            cranialSpikeBuff = Modules.Content.CreateAndAddBuff("CranialSpikeBuff",
				LegacyResourcesAPI.Load<BuffDef>("BuffDefs/DeathMark").iconSprite,
				Color.yellow,
				true,
				false);

			RadiantBuff = Modules.Content.CreateAndAddBuff("RadiantBuff",
				LegacyResourcesAPI.Load<BuffDef>("BuffDefs/DeathMark").iconSprite,
				new Color(1f,0.5f,0f),
				true,
				false);;

		}
    }
}

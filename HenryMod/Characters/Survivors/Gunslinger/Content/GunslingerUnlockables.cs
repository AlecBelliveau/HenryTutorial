using DestinyGuardiansMod.Survivors.Gunsligner.Achievements;
using RoR2;
using UnityEngine;

namespace DestinyGuardiansMod.Survivors.Gunsligner
{
    public static class GunslingerUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            //masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
            //    GunslingerAchievement.unlockableIdentifier,
            //    Modules.Tokens.GetAchievementNameToken(GunslingerAchievement.identifier),
            //    GunslingerSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}

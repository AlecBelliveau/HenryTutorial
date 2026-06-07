using DestinyGuardiansMod.Survivors.Gunsligner.SkillStates;

namespace DestinyGuardiansMod.Survivors.Gunsligner
{
    public static class GunslingerStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(SlashCombo));

            Modules.Content.AddEntityState(typeof(Shoot));

            Modules.Content.AddEntityState(typeof(AcrobatsDodgeJump));

            Modules.Content.AddEntityState(typeof(ThrowBomb));
        }
    }
}

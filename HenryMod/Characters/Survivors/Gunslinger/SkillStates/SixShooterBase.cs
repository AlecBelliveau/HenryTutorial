using EntityStates;
using DestinyGuardiansMod.Survivors.Gunsligner;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace DestinyGuardiansMod.Survivors.Gunsligner.SkillStates
{
	public class SixShooterBase : BaseSkillState
	{
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}
	}
}
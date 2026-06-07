using EntityStates;
using DestinyGuardiansMod.Survivors.Gunsligner;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace DestinyGuardiansMod.Survivors.Gunsligner.SkillStates
{
	public class SixShooterStart : SixShooterBase
	{
		public static float baseDuration = 0.5f;
		private float duration;
		
		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration / attackSpeedStat;
			
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && duration <= base.fixedAge)
			{
				outer.SetNextState(new SixShooterPaint());
				Debug.Log("Paint");
			}
		}
	}
}
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

public class JobGiver_AIRemoteDetonate : ThinkNode_JobGiver {
	
	protected override Job TryGiveJob(Pawn pawn) {
		ThingWithComps equipment = pawn.equipment.Primary;
		if (equipment == null) {
			return (Job) null;
		}
		
		CompWeaponRemoteDetonates compDet = equipment.TryGetComp<CompWeaponRemoteDetonates>();
		CompEquippableAbility compAbility = equipment.TryGetComp<CompEquippableAbility>();
		if (compDet == null || compAbility == null) {
			return (Job) null;
		}
		
		Vector4 nearby = compDet.ThingFactionsNearby(pawn.Faction);
		float chance = (nearby.w * .25f) + (nearby.x * .1f) - (nearby.y * .1f);
		// Log.Message("Chance to give job: " + chance);

		if (!Rand.Chance(chance))
		{
			return (Job) null;
		}
		
		Ability ability = compAbility.AbilityForReading;
		
		Job abilityJob = JobMaker.MakeJob(JobDefOf.CastAbilityOnThing, new LocalTargetInfo(pawn));
		abilityJob.verbToUse = ability.verb;
		abilityJob.targetA = (LocalTargetInfo) pawn;
		abilityJob.ability = ability;
		return abilityJob;
	}
}

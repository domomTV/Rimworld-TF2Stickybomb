using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

public class CompAbilityEffect_RemoteDetonate : CompAbilityEffect {
	public override bool AICanTargetNow(LocalTargetInfo target) {
		// Log.Message("AI is checking to use");
		if (!(target.IsValid && target.HasThing)) 
		{
			return false;
		}

		Pawn caster = target.Pawn;
		ThingWithComps equipment = caster.equipment.Primary;
		CompWeaponRemoteDetonates comp = equipment.TryGetComp<CompWeaponRemoteDetonates>();
		if (comp == null) 
		{
			return false;
		}
		if (comp.projectiles.Count == 0) 
		{
			return false;
		}

		Vector4 nearby = comp.ThingFactionsNearby(caster.Faction);
		float chance = nearby.w * 0.25f;
		
		// Log.Message("Chance to target: " + chance);
		bool result = Rand.Chance(chance);
		
		// if (result) { Log.Message("Succeeded to use"); }
		// else { Log.Message("Failed to use"); }

		return result;
	}
}

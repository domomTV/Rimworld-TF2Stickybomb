using Verse;
using RimWorld;

public class Verb_RemoteDetonate : Verb {
	protected override bool TryCastShot() {
		if (!this.CasterIsPawn) 
		{
			Log.Error("Error: Non-pawn tried to cast verb " + this.verbProps.label);
			return false;
		}
		Pawn casterPawn = this.CasterPawn;
		ThingWithComps equipment = casterPawn.equipment.Primary;
		CompWeaponRemoteDetonates comp = equipment.TryGetComp<CompWeaponRemoteDetonates>();
		if (comp == null) 
		{
			Log.Error("Error: Primary equipment does not have CompWeaponRemoteDetonates");
			return false;
		}
		comp.RemoteDetonateAllProjectiles();
		return true;
	}
}
using System;
using RimWorld;
using Verse;

public class Ability_RemoteDetonate : Ability {

	private void Init() {
		this.maxCharges = 8;
		this.RemainingCharges = 0;
	}
	
	public Ability_RemoteDetonate() {
		Init();
	}

	public Ability_RemoteDetonate(Pawn pawn) {
		this.pawn = pawn;
		Init();
	}

	public Ability_RemoteDetonate(Pawn pawn, Precept sourcePrecept) {
		this.pawn = pawn;
		this.sourcePrecept = sourcePrecept;
		Init();
	}

	public Ability_RemoteDetonate(Pawn pawn, AbilityDef def) {
		this.pawn = pawn;
		this.def = def;
		this.Initialize();
		Init();
	}

	public Ability_RemoteDetonate(Pawn pawn, Precept sourcePrecept, AbilityDef def) {
		this.pawn = pawn;
		this.def = def;
		this.sourcePrecept = sourcePrecept;
		this.Initialize();
		Init();
	}

	public void AddProjectileCount(int count) {
		this.SetProjectileCount(this.RemainingCharges + count);
	}

	public void SetProjectileCount(int count) {
		if (count <= this.maxCharges && count >= 0)
		{
			this.RemainingCharges = count;
		}
	}
}
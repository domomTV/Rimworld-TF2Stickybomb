using RimWorld;
using UnityEngine;
using Verse;


public class Projectile_RemoteExplosive : Projectile_Explosive {
	/// <summary>
	/// Determines if this projectile will explode or fizzle when Explode() is called.
	/// </summary>
	private bool primed = false;
	/// <summary>
	/// Determines if this projectile can be remote detonated.
	/// </summary>
	public bool armed = false;
	/// <summary>
	/// Used instead of ticksToDetonation because its private &amp; I needed to override Impact().
	/// Defined by "explosionDelay" XML tag.
	/// </summary>
	public int ticksToFizzle;
	
	/// <summary>
	/// Sets the projectile to detonate if it is armed.
	/// </summary>
	/// <returns>True if detonation succeeded</returns>
	public bool RemoteDetonate() {
		if (!this.armed) 
		{
			return false;
		}
		this.primed = true;
		this.Explode();
		return true;
	}

	protected override void Explode() {
		if (!this.armed) 
		{
			return;
		}
		if (this.primed) 
		{
			base.Explode();
		} 
		else 
		{
			this.Fizzle();
		}
	}

	public void Fizzle(bool wasRemoved = false) {
		if (!wasRemoved) 
		{
			CompWeaponRemoteDetonates comp = this.equipment.TryGetComp<CompWeaponRemoteDetonates>();
			if (comp == null) 
			{
				Log.Error("Error: Projectile_RemoteExplosive was shot from equipment without CompWeaponRemoteDetonates");
				return;
			}
			comp.RemoveProjectile(this);
		}
		
		this.Destroy();
	}

	public override void Launch(
		Thing p_launcher, 
		Vector3 p_origin, 
		LocalTargetInfo p_usedTarget, 
		LocalTargetInfo p_intendedTarget,
		ProjectileHitFlags p_hitFlags, 
		bool p_preventFriendlyFire = false, 
		Thing p_equipment = null,
		ThingDef p_targetCoverDef = null) 
	{
		base.Launch(p_launcher, p_origin, p_usedTarget, p_intendedTarget, p_hitFlags, p_preventFriendlyFire, p_equipment, p_targetCoverDef);
		CompWeaponRemoteDetonates comp = p_equipment.TryGetComp<CompWeaponRemoteDetonates>();
		if (comp == null) 
		{
			Log.Error("Error: Projectile_RemoteExplosive was shot from equipment without CompWeaponRemoteDetonates");
			return;
		}
		comp.AddProjectile(this);
	}

	// Public var for other classes
	public bool Landed => this.landed;
	
	// public override Quaternion ExactRotation
	// {
	// 	get
	// 	{
	// 		Quaternion ret = Quaternion.
	// 	}
	// }
	
	// ==================================================================
	// Overriding these to stop pawns from getting notified of explosions
	// ==================================================================
	protected override void TickInterval(int delta)
	{
		base.TickInterval(delta);
		if (this.ticksToFizzle <= 0)
			return;
		this.ticksToFizzle -= delta;
		if (this.ticksToFizzle > 0)
			return;
		this.Fizzle();
	}
	
	protected override void Impact(Thing hitThing, bool blockedByShield = false) {
		this.armed = true;
		if (blockedByShield || this.def.projectile.explosionDelay == 0)
		{
			this.Explode();
		}
		else
		{
			this.landed = true;
			this.ticksToFizzle = this.def.projectile.explosionDelay;
			// GenExplosion.NotifyNearbyPawnsOfDangerousExplosive((Thing) this, this.DamageDef, this.launcher.Faction, this.launcher);
		}
	}
	
	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look<int>(ref this.ticksToFizzle, "ticksToFizzle");
		Scribe_Values.Look<bool>(ref this.armed, "armed");
		Scribe_Values.Look<bool>(ref this.primed, "primed");
	}
}
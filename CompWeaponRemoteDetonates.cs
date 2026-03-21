using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

public class CompWeaponRemoteDetonates : ThingComp {
	private CompProperties_WeaponRemoteDetonates Props => (CompProperties_WeaponRemoteDetonates)this.props;

	public LinkedList<Projectile_RemoteExplosive> projectiles = new LinkedList<Projectile_RemoteExplosive>();

	public int MaxProjectiles
	{
		get { return Props.maxProjectiles; }
	}

	public void RemoteDetonateAllProjectiles() {
		// Log.Message("Detonating projectiles");
		foreach (Projectile_RemoteExplosive proj in projectiles)
		{
			if (proj.RemoteDetonate())
			{
				this.MarkForDeletion(proj);
			}
		}

		this.DoDelete(false);
	}

	public void AddProjectile(Thing thing) {
		if (thing.GetType() != typeof(Projectile_RemoteExplosive))
		{
			return;
		}

		Projectile_RemoteExplosive proj = (Projectile_RemoteExplosive) thing;
		projectiles.AddLast(proj);
		
		while (projectiles.Count > MaxProjectiles)
		{
			Projectile_RemoteExplosive projToExplode = projectiles.First.Value;
			projectiles.RemoveFirst();
			projToExplode.Fizzle(true);
		}
		
		Ability_RemoteDetonate ability = this.GetAbility();
		ability.AddProjectileCount(1);
	}

	public void RemoveProjectile(Thing thing) {
		if (thing.GetType() != typeof(Projectile_RemoteExplosive))
		{
			return;
		}

		Projectile_RemoteExplosive proj = (Projectile_RemoteExplosive)thing;
		if (projectiles.Contains(proj))
		{
			projectiles.Remove(proj);
			Ability_RemoteDetonate ability = this.GetAbility();
			ability.AddProjectileCount(-1);
		}
		else
		{
			Log.Warning("CompWeaponRemoteDetonates: Tried to remove projectile from list that wasn't there.");
		}
	}

	/// <summary>
	/// Looks around all shot projectiles for Things in a 3x3 cube.
	/// </summary>
	/// <param name="casterFaction">The caster's faction</param>
	/// <returns>A Vector4 of: w=EnemyPawns, x=EnemyBuildings, y=AllyPawns, z=AllyBuildings</returns>
	public Vector4 ThingFactionsNearby(Faction casterFaction) {
		Vector4 ret = Vector4.zero;
		int numCells = GenRadial.NumCellsInRadius(1.9f);
		foreach (Projectile_RemoteExplosive proj in this.projectiles)
		{
			if (proj == null || proj.Destroyed)
			{
				this.MarkForDeletion(proj);
				continue;
			}
			
			if (!proj.Landed) { continue; }

			for (int idxCell = 0; idxCell < numCells; ++idxCell)
			{
				IntVec3 c = proj.Position + GenRadial.RadialPattern[idxCell];
				if (!c.InBounds(proj.Map)) { continue; }

				List<Thing> thingList = c.GetThingList(proj.Map);
				foreach (Thing thing in thingList)
				{
					if (thing is Pawn && thing.Faction.HostileTo(casterFaction))
					{
						ret.w += 1;
					}
					else if (thing is Building && thing.Faction.HostileTo(casterFaction))
					{
						ret.x += 1;
					}
					else if (thing is Pawn && thing.Faction.AllyOrNeutralTo(casterFaction))
					{
						ret.y += 1;
					}
					else if (thing is Building && thing.Faction.AllyOrNeutralTo(casterFaction))
					{
						ret.z += 1;
					}
				}
			}
		}

		DoDelete(true);
		return ret;
	}

	private List<Projectile_RemoteExplosive> toDelete = new List<Projectile_RemoteExplosive>();

	private void MarkForDeletion(Projectile_RemoteExplosive proj) {
		toDelete.Add(proj);
	}

	private void DoDelete(bool destroy = false) {
		foreach (Projectile_RemoteExplosive proj in toDelete)
		{
			if (projectiles.Contains(proj))
			{
				this.RemoveProjectile(proj);
			}

			if (destroy && !proj.Destroyed)
			{
				proj.Destroy();
			}
		}
	}

	private Ability_RemoteDetonate GetAbility() {
		CompEquippableAbility comp = this.parent.TryGetComp<CompEquippableAbility>();
		if (comp == null)
		{
			return null;
		}

		Ability a = comp.AbilityForReading;
		if (a.GetType() != typeof(Ability_RemoteDetonate))
		{
			return null;
		}

		return (Ability_RemoteDetonate) a;
	}
}

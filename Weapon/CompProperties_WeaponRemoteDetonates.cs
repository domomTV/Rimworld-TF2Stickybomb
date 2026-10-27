using Verse;

public class CompProperties_WeaponRemoteDetonates : CompProperties {
	public int maxProjectiles = 8;
	
	public CompProperties_WeaponRemoteDetonates() => this.compClass = typeof (CompWeaponRemoteDetonates);
}
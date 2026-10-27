using Verse;

public class CompProperties_RemoteExplosive: CompProperties {
	public int armDelay = 1;
	
	public CompProperties_RemoteExplosive() => this.compClass = typeof (CompRemoteExplosive);
}

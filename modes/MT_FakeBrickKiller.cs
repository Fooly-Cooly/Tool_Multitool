MT_AddMode("Fake Brick Killer", $TypeMasks::FxBrickAlwaysObjectType, "MT_FakeBrickKiller");

function MT_FakeBrickKiller(%cl,%col)
{
	if((getTrustLevel(%cl, %col) > 1) || %cl.isSuperAdmin) %col.fakekillBrick("0 200 100", 5);
	else centerPrint(%cl, %col.getGroup().name @ " does not trust you enough to do that.", 1);
}
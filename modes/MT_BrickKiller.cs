MT_AddMode("Brick-Killer", $TypeMasks::FxBrickAlwaysObjectType, "MT_BrickKiller");

function MT_BrickKiller(%cl, %col)
{
	if((getTrustLevel(%cl, %col) > 1) || %cl.isSuperAdmin) %col.killBrick();
	else centerPrint(%cl, %col.getGroup().name @ " does not trust you enough to do that.", 1);
}
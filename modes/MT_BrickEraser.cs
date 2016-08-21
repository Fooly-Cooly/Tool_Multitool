MT_AddMode("Brick-Eraser", $TypeMasks::FxBrickAlwaysObjectType, "MT_BrickEraser");

function MT_BrickEraser(%cl, %col)
{
	if((getTrustLevel(%cl, %col) > 1) || %cl.isSuperAdmin) %col.delete();
	else centerPrint(%cl, %col.getGroup().name @ " does not trust you enough to do that.", 1);
}
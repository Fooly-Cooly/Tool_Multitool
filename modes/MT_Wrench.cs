MT_AddMode("Wrench", $TypeMasks::FxBrickAlwaysObjectType, "MT_Wrench");

function MT_Wrench(%cl, %col)
{
	%hitObj = getWord(%col, 0);
	%hitPos = getWords(%col, 1, 3);
	%hitNor = getWords(%col, 4, 6);
	WrenchImage.onHitObject(%cl.player, 0, %hitObj, %hitPos, %hitNor);
}

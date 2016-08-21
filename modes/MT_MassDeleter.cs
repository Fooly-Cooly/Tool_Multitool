MT_AddMode("Mass-Deleter", $TypeMasks::FxBrickAlwaysObjectType, "MT_MassDeleter");

function MT_MassDeleter(%cl, %col)
{
	if(!%cl.isLan() || !isObject(%col)) return;
	%colDB = %col.getDataBlock();
	%rot = round(getword(%col.rotaton, 3));
	%area = %rot == 90 ? %colDB.bricksizey SPC %colDB.bricksizex : %colDB.bricksizex SPC %colDB.bricksizey;
	%area = %area SPC %colDB.bricksizez;
	%area = vectorScale(%area, "0.55 0.55 0.25");
	InitContainerBoxSearch(%col.getposition(), %area, $TypeMasks::FxBrickAlwaysObjectType);
	while((%brk = containerSearchNext()) > 0)
		if(%brk.isPlanted() && %col != %brk)
			schedule(200, 0, "MT_MassDeleter", %cl, %brk);
	%col.delete();
}




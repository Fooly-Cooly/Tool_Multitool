MT_AddMode("Publication", $TypeMasks::FxBrickAlwaysObjectType, "MT_Publication");

function MT_Publication(%cl,%col)
{
	switch(%cl.isSuperAdmin)
	{
		case 0: centerPrint(%cl, %col.getGroup().name @ " does not trust you enough to do that.", 1);
		default:brickGroup_888888.add(%col);
				%col.stackBL_ID = -1;
				%col.client = -1;
	}
}


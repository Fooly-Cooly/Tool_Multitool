MT_AddMode("Measurer", $TypeMasks::FxBrickAlwaysObjectType, "MT_Measurer");

function MT_Measurer(%cl, %col)
{
	if(!%cl.mes1)
	{
		%cl.mes1 = %col.getPosition();
		centerPrint(%cl, "\c3First brick hit. Please hit second brick.", 3);
		return;
	}
	%cl.mes2 = %col.getPosition();
	%vec = vectorSub(%cl.mes1, %cl.mes2);
	%len = vectorLen(%vec);
	%len = %len / 0.5;
	%len = %len - 1;
	if(%len)
	{
		centerPrint(%cl, "\c3Distance Between Bricks: \c0" @ %len, 3);
		%cl.mes1 = 0;
		%cl.mes2 = 0;
	}
	else centerPrint(%cl, "Please hit a different brick!", 3);
}

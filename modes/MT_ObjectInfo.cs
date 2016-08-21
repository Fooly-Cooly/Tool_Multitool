MT_AddMode("Object-Info", $TypeMasks::All, "MT_ObjectInfo");

function MT_ObjectInfo(%cl, %col)
{
	if(!%cl.isSuperAdmin)
		return;
	%class = %col.getClassName();
	%title = %class @ " - " @ getWord(%col, 0);
	switch$(%class)
	{
		case "fxDTSBrick":
			commandToClient(%cl, 'MessageBoxOK', %title, %col.getName()
			@ "\n" @ %col.getGroup().name @ "[" @ %col.getGroup().bl_id @ "]"
			@ "\n" @ %col.getPosition()
			@ "\n" @ %col.getDataBlock().getName() @ "[" @ %col.getDataBlock() @ "]");
		case "Player":
			commandToClient(%cl, 'MessageBoxOK', %title, %col.client
			@ "\n" @ %col.client.getPlayerName() SPC "[" @ %col.client.bl_id @ "]"
			@ "\n" @ %col.getPosition()
			@ "\n" @ %col.getDataBlock().getName() SPC "[" @ %col.getDataBlock() @ "]");
		default:
			commandToClient(%cl, 'MessageBoxOK', %title, %col.getPosition()
			@ "\n" @ %col.getDataBlock().getName() SPC "[" @ %col.getDataBlock() @ "]");
	}
}
